using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using SysViewHyTurb.Driver.Utility;
using System.Windows.Forms;

namespace SysViewHyTurb.App
{
    class ItemDef
    {
        public string Code;
        public string Value;
        public ushort VarID;
        public ushort Decimal;
    }

    class TcpHttpCient
    {
        /// <summary>
        /// private serial port
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// private destination (ip or dsn)
        /// </summary>
        private string dest;

        private int port;

        /// <summary>
        /// private specific page
        /// </summary>
        private string path;

        private int interval;

        private List<ItemDef> itemList = new List<ItemDef>();

        private System.Threading.Timer timer;

        private System.Threading.Timer checkTimer;

        private System.Threading.Timer delayDialup;

        private DataRepo repo;

        private bool enabled;

        private string message;

        private string lastSentString;

        private object lockDtu = new object();

        private int readTimeOut = 5000;

        private int writeTimeOut = 5000;

        private bool connected;

        private int read(byte[] buffer, int offset, int size)
        {
            int bytesRead = 0;
            AutoResetEvent readCompleted = new AutoResetEvent(false);

            var asyncResult = this.tcpClient.GetStream().BeginRead(buffer, offset, size, (ar) =>
            {
                bytesRead = ((NetworkStream)ar.AsyncState).EndRead(ar);
                readCompleted.Set();

            }, 
            tcpClient.GetStream());

            bool completed = readCompleted.WaitOne(this.readTimeOut, false);
            if (!completed)
            {
                throw new TimeoutException();
            }
            return bytesRead;
            
        }

        private void timerEventProcessor(Object StateInfo)  
        {  
            //this.timer.Enabled = false;
            //this.Enabled = false;
           
            
            lock (this.lockDtu)
            {
                
                try
                {
                    this.tcpClient = new TcpClient();
                    this.tcpClient.Connect(dest, port);

                    this.Message = "连接到" + this.dest + ":" + this.port.ToString() + "成功"; 

                    string sendString = this.buildSendString();
                    int tosend = (int)StateInfo;
                    if (tosend > 0)
                    {
                        //this.timer.Change(Timeout.Infinite, this.interval * 1000);
                        this.Message = "定时上传数据";
                        this.SendData(sendString);
                    }
                    else
                    {
                        if (sendString != this.lastSentString)
                        {
                            this.Enabled = false;
                            this.Message = "上传新的数据";
                            this.SendData(sendString);
                            this.Enabled = true;
                        }
                        else
                            this.Message = "数据未改变，无需上传数据";
                    }
                    this.tcpClient.GetStream().Close();
                    this.tcpClient.Close();
                }

                catch (Exception e)
                {
                    this.Message = "连接到" + this.dest + ":" + this.port.ToString() + "失败  " + e.Message;
                    try
                    {
                        //RasManager.Instance.DialUp("WCDMA", null, null);
                    }
                    catch (Exception ex)
                    {
                        this.Message = "拨号连接失败： " + ex.Message;
                    }
                }

                this.tcpClient.Close();
            }
            //this.Enabled = true;
        }

        private string buildSendString()
        {
            string sendString;
            sendString = "GET " + this.path + "?";
            foreach (ItemDef itm in this.itemList)
            {
                if (itm.Value != string.Empty)
                {
                    sendString += itm.Code + "=" + itm.Value;
                }
                else
                {
                    string formatString = "##0";
                    if(itm.Decimal >  0)
                    {
                        formatString += "." + new string('0', itm.Decimal);
                    }
                    sendString += itm.Code + "=" + this.repo.InputRegisters[itm.VarID].ToString(formatString);
                }
                sendString += "&";
                //MessageBox.Show(sendString);
            }
            sendString = sendString.TrimEnd('&');
            sendString += " HTTP/1.1 \r\n"
                + "Host: " + this.dest +"\r\n"
                //+ "Connection: keep-alive\r\n"
                //+ "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n"
                //+ "User-Agent: Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36\r\n"
                + "\r\n";
            return sendString;
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set 
            { 
                this.enabled = value;
                if (value)
                {
                    this.timer.Change(this.interval * 1000, this.interval * 1000);
                    this.checkTimer.Change(60000, 60000);
                }
                else
                {
                    this.timer.Change(Timeout.Infinite, this.interval * 1000);
                    this.checkTimer.Change(Timeout.Infinite, 60000);
                }
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (value == string.Empty)
                    this.message = string.Empty;
                else
                    this.message += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + value + Environment.NewLine;      
            }
        }

        public TcpHttpCient(string dest, int port, string path, int interval,DataRepo repo)
        {
            this.dest = dest;
            this.port = port;
            this.path = path;
            this.interval = interval;
            this.repo = repo;
            this.timer = new System.Threading.Timer(this.timerEventProcessor, 1, Timeout.Infinite, this.interval * 1000);
            this.checkTimer = new System.Threading.Timer(this.timerEventProcessor, 0, Timeout.Infinite, 60000);
            //this.delayDialup = new System.Threading.Timer(this.Dialup, 0, 3000, Timeout.Infinite);

        }

        ~TcpHttpCient()
        {
            try
            {
                //RasManager.Instance.HangUp();
            }
            catch (Exception)
            {
            }
        }

        void OnDisconnectedEvent()
        {
            this.connected = false;
            this.Message = "拨号网络已断开";
        }

        void OnConnectedEvent()
        {
            this.connected = true;
            this.Message = "拨号网络已连接";
        }

        public void AddItem(ItemDef item)
        {
            this.itemList.Add(item);
        }

        public void SendData(string sendString)
        {
            var result = new StringBuilder();
            var singleByteBuffer = new byte[1];
            //MessageBox.Show(sendString);
            try
            {              
                byte[] sendBytes = Encoding.ASCII.GetBytes(sendString);
                this.tcpClient.GetStream().Write(sendBytes, 0, sendBytes.Count());
                this.Message = sendString;
            }
            catch(Exception e)
            {
                this.Message = "发送数据出错： " + e.Message;
            }

            //read and remove the header
            try
            {
                do
                {
                    this.read(singleByteBuffer, 0, 1);
                    result.Append(Encoding.ASCII.GetChars(singleByteBuffer).First());
                } while (!result.ToString().EndsWith("\r\n\r\n"));
                //MessageBox.Show(result.ToString());
                result.Remove(0, result.Length);
                this.Message = "收到 http 返回头";
            }
            catch (Exception)
            {
                this.Message = "服务器无返回，请检查网络连接";
            }
                    
            try
            {
                //read the first line of the content(only the first line is used)
                do
                {
                        this.read(singleByteBuffer, 0, 1);
                        result.Append(Encoding.ASCII.GetChars(singleByteBuffer).First());
                        if (result.Length > 250)
                            break;
                } while (!result.ToString().EndsWith("\r\n"));

                this.Message = "收到:" + result.ToString();

                //MessageBox.Show("content is " + result.ToString());
                string[] resultParts = result.ToString().TrimEnd(new char[] { '\r', '\n' }).Split(';');
                if (resultParts[0].Trim() == "1")
                {
                     this.Message = "数据上传成功: " + result.ToString();
                     this.lastSentString = sendString;
                }
                else
                {
                     this.Message = "服务器返回结果格式错误";
                            //MessageBox.Show(result.ToString());
                }

             }
             catch(Exception e)
             {
                //do nothing 
                this.Message = "读取服务器返回错误: " + e.Message;
             }
        }
    }
}

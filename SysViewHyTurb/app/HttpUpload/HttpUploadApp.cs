namespace SysViewHyTurb.app.TcpHttpClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms.VisualStyles;
    using System.Xml;
    using System.Xml.Linq;

    using SysViewHyTurb.app.TcpHttpUpload;
    using SysViewHyTurb.data;

    using Timer = System.Threading.Timer;


    class HttpUploadApp
    {
        /// <summary>
        /// private tcp client
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// private destination (ip or dsn)
        /// </summary>
        private readonly string dest;

        /// <summary>
        /// private destination port
        /// </summary>
        private readonly int port;

        /// <summary>
        /// private specific page
        /// </summary>
        private readonly string path;

        private readonly int interval;

        private readonly List<VarItem> itemList = new List<VarItem>();

        private readonly Timer timer;

        private readonly Timer checkTimer;

        private Timer delayDialup;

        private readonly DataRepo repo;

        private bool enabled;

        private string message;

        private string lastSentString;

        private readonly object lockDtu = new object();

        private const int ReadTimeOut = 5000;

        private int WriteTimeOut = 5000;

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
            this.tcpClient.GetStream());

            bool completed = readCompleted.WaitOne(ReadTimeOut, false);
            if (!completed)
            {
                throw new TimeoutException();
            }
            return bytesRead;
            
        }

        private void DataChangedUpload(KeyValuePair<string, object>[] keyValues)
        {
            
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
                    this.tcpClient.Connect(this.dest, this.port);

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

        private string BuildSendString()
        {
            var sendString = "GET " + this.path + "?";
            foreach (var itm in this.itemList)
            {
                if (itm.VarName == "##Reserved")
                {
                    sendString += itm.Code + "=" + itm.Value;
                }
                else
                {
                    var objValue = this.repo.ReadValue(itm.VarName);
                    if (objValue != null)
                    {
                        var dblValue = (double)objValue;
                        var formatString = "##0";
                        if (itm.Decimal > 0)
                        {
                            formatString += "." + new string('0', itm.Decimal);
                        }
                        sendString += itm.Code + "=" + dblValue.ToString(formatString);
                    }
                   
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

        public HttpUploadApp(XElement httpClientElement, DataRepo repo)
        {
            this.dest = httpClientElement.Attribute("dest").Value;
            this.port = int.Parse(httpClientElement.Attribute("dest").Value);
            this.path = httpClientElement.Attribute("path").Value;
            this.interval = int.Parse(httpClientElement.Attribute("interval").Value);
            this.repo = repo;
            this.repo.ValueChanged += this.DataChangedUpload;

            this.checkTimer = new Timer(this.timerEventProcessor, 0, Timeout.Infinite, this.interval * 1000);

            foreach (var itemElement in httpClientElement.Elements("Item"))
            {
                var varItem = new VarItem();
                if (itemElement.Attribute("value") != null)
                {
                    varItem.Value = itemElement.Attribute("value").Value;
                    varItem.VarName = "##Reserved";

                }
                else
                {
                    varItem.Value = string.Empty;
                    varItem.VarName = itemElement.Attribute("name").Value;
                    varItem.Decimal = ushort.Parse(itemElement.Attribute("decimal").Value);
                }
                this.itemList.Add(varItem);
            }  
        }

        ~HttpUploadApp()
        {
            try
            {
                //RasManager.Instance.HangUp();
            }
            catch (Exception)
            {
            }
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

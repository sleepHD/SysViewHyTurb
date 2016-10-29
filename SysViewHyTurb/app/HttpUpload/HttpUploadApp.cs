namespace SysViewHyTurb.app.TcpHttpClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Xml.Linq;

    using SysViewHyTurb.app.TcpHttpUpload;
    using SysViewHyTurb.data;

    using Timer = System.Threading.Timer;


    class HttpUploadApp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

        /// <summary>
        /// private interval to send data
        /// </summary>
        private readonly int interval;

        private readonly List<VarItem> itemList = new List<VarItem>();

        private readonly Timer timer;

        private readonly DataRepo repo;

        private readonly object lockDtu = new object();

        private const int ReadTimeOut = 5000;

        private int WriteTimeOut = 5000;

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
                + "Host: " + this.dest + "\r\n"
                //+ "Connection: keep-alive\r\n"
                //+ "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n"
                //+ "User-Agent: Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36\r\n"
                + "\r\n";
            return sendString;
        }

        public void SendData()
        {
            lock (this.lockDtu)
            {   
                try
                {
                    this.tcpClient = new TcpClient();
                    this.tcpClient.Connect(this.dest, this.port);
                    this.tcpClient.ReceiveTimeout = ReadTimeOut;
                    this.tcpClient.SendTimeout = WriteTimeOut;

                    log.Info("���ӵ�" + this.dest + ":" + this.port.ToString() + "�ɹ�");

                    var sendString = this.BuildSendString();
                    var result = new StringBuilder();
                    var singleByteBuffer = new byte[1];

                    try
                    {
                        byte[] sendBytes = Encoding.ASCII.GetBytes(sendString);
                        this.tcpClient.GetStream().Write(sendBytes, 0, sendBytes.Count());
                        log.Info("�������ݳɹ�");
                        try
                        {
                            do
                            {
                                this.tcpClient.GetStream().Read(singleByteBuffer, 0, 1);
                                result.Append(Encoding.ASCII.GetChars(singleByteBuffer).First());
                            } while (!result.ToString().EndsWith("\r\n\r\n"));
                            result.Remove(0, result.Length);
                            log.Info("�յ� http ����ͷ");
                            try
                            {
                                //read the first line of the content(only the first line is used)
                                do
                                {
                                    this.tcpClient.GetStream().Read(singleByteBuffer, 0, 1);
                                    result.Append(Encoding.ASCII.GetChars(singleByteBuffer).First());
                                    if (result.Length > 250)
                                        break;
                                } while (!result.ToString().EndsWith("\r\n"));

                                log.Info("�յ��������������ݽ��մ���");

                                string[] resultParts = result.ToString().TrimEnd(new char[] { '\r', '\n' }).Split(';');
                                if (resultParts[0].Trim() == "1")
                                {
                                    log.Info("�������ѳɹ������ϴ�����");
                                }
                                else
                                {
                                    log.Error("�������ܾ������ϴ�����");
                                }

                            }
                            catch (Exception ex)
                            {
                                log.Error("δ�յ��������������ݽ��մ���" + ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("������δ����httpͷ" + ex.Message);
                        }

                    }
                    catch (Exception e)
                    {
                        log.Error("�������ݳ��� " + e.Message);
                    }
                     
                    this.tcpClient.GetStream().Close();
                }

                catch (Exception e)
                {
                    log.Error("���ӵ�" + this.dest + ":" + this.port.ToString() + "ʧ��  " + e.Message);
                }

                this.tcpClient.Close();
            }
        }

        private void DataChangedUpload(KeyValuePair<string, object>[] keyValues)
        {
            this.timer.Change(Timeout.Infinite, this.interval * 1000);
            log.Info("�����иı䣬�ϴ��µ����ݵ�������");
            this.SendData();
            this.timer.Change(this.interval * 1000, this.interval * 1000);
        }

        private void timerEventProcessor(Object StateInfo)  
        {
            this.timer.Change(Timeout.Infinite, this.interval * 1000);
            log.Info("��ʱ�ϴ����ݵ�������");
            this.timer.Change(this.interval * 1000, this.interval * 1000);
        }

        public HttpUploadApp(XElement httpClientElement, DataRepo repo)
        {
            this.dest = httpClientElement.Attribute("dest").Value;
            this.port = int.Parse(httpClientElement.Attribute("dest").Value);
            this.path = httpClientElement.Attribute("path").Value;
            this.interval = int.Parse(httpClientElement.Attribute("interval").Value);
            this.repo = repo;
            this.repo.ValueChanged += this.DataChangedUpload;

            this.timer = new Timer(this.timerEventProcessor, 0, Timeout.Infinite, this.interval * 1000);

            foreach (var itemElement in httpClientElement.Elements("Item"))
            {
                var varItem = new VarItem();
                if (itemElement.Attribute("value") != null)
                {
                    varItem.Value = itemElement.Attribute("value").Value;
                    varItem.VarName = "##Reserved";
                    varItem.Code = itemElement.Attribute("Code").Value;

                }
                else
                {
                    varItem.Value = string.Empty;
                    varItem.Decimal = ushort.Parse(itemElement.Attribute("decimal").Value);
                    varItem.VarName = itemElement.Attribute("name").Value;            
                    varItem.Code = itemElement.Attribute("Code").Value;
                }
                this.itemList.Add(varItem);
            }  
        }
    }
}

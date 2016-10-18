
namespace SysViewCp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.IO.Ports;
    using SysViewCp.App;
    using SysViewCp.Driver;
    using System.Windows.Forms;

    class AppDriverContainer
    {
        private readonly DataRepo repo;

        private readonly ModbusPoller devPoller;

        private readonly TcpHttpCient tcpClient;

        public DataRepo Repo { get { return this.repo; } }

        public ModbusPoller DevPoller { get { return this.devPoller; } }

        public TcpHttpCient TcpClient { get { return this.tcpClient; } }

        public AppDriverContainer(string configFile)
        {
            this.repo = new DataRepo();
            //this.repo.InputRegisters[0] = -23;
            //this.repo.InputRegisters[1] = 14.14F;
            //this.repo.InputRegisters[2] = 15.14F;
            //this.repo.InputRegisters[3] = 16.14F;
            
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            //MessageBox.Show(configFile);
            XmlElement rootElem = doc.DocumentElement;

            XmlNode driverNode= rootElem.SelectSingleNode("Driver");
            string portName = driverNode.Attributes["port"].Value;
            string setting = driverNode.Attributes["setting"].Value;
            //MessageBox.Show("DriverPort: " + portName + "  Setting:  " + setting);
            this.devPoller = new ModbusPoller(creatSerialPort(portName, setting), this.repo);
            //MessageBox.Show("Serial port created");
            XmlNodeList moduleNodeList = driverNode.SelectNodes("Module");
            foreach ( XmlNode moduleNode in moduleNodeList)
            {
                ModbusSlave slave = new ModbusSlave(ushort.Parse(moduleNode.Attributes["id"].Value));
                foreach ( XmlNode registerNode in moduleNode.SelectNodes("Register"))
                {
                    ModbusReg reg = new ModbusReg();
                    reg.VarID = ushort.Parse(registerNode.Attributes["varID"].Value);
                    reg.RawMax = double.Parse(registerNode.Attributes["rawMax"].Value);
                    reg.RawMin = double.Parse(registerNode.Attributes["rawMin"].Value);
                    reg.VarMax = double.Parse(registerNode.Attributes["varMax"].Value);
                    reg.VarMin = double.Parse(registerNode.Attributes["varMin"].Value);
                    reg.Convert = ushort.Parse(registerNode.Attributes["convert"].Value);
                    reg.RegID = ushort.Parse(registerNode.Attributes["regID"].Value);
                    reg.RegType = TypeCode.Single;
                    reg.RegSwap = 0;
                    slave.AddVar(reg);
                }
                this.devPoller.AddSlave(slave);
            }
            this.devPoller.Enabled = true;
            //MessageBox.Show("poller started");
            
            XmlNode UploadNode = rootElem.SelectSingleNode("Upload");
            string dest = UploadNode.Attributes["dest"].Value;
            string port = UploadNode.Attributes["port"].Value;
            string path = UploadNode.Attributes["path"].Value;
            //string gduPort = UploadNode.Attributes["gduPort"].Value;
            //string gduSetting = UploadNode.Attributes["gduSetting"].Value;
            string interval = UploadNode.Attributes["interval"].Value;
            
            this.tcpClient = new TcpHttpCient(dest, int.Parse(port),path,int.Parse(interval),this.repo);
            //MessageBox.Show(dest + path + gduPort + gduSetting + interval);
            XmlNodeList itemNodeList = UploadNode.SelectNodes("Item");
            foreach ( XmlNode itemNode in itemNodeList)
            {
                ItemDef item = new ItemDef();
                item.Code = itemNode.Attributes["code"].Value;
                //MessageBox.Show(item.Code);
                if(itemNode.Attributes["value"] != null)
                {
                    item.Value = itemNode.Attributes["value"].Value;
                    item.VarID = 65535;
                }
                else
                {
                    item.Value = string.Empty;
                    item.VarID = ushort.Parse(itemNode.Attributes["varID"].Value);
                    item.Decimal = ushort.Parse(itemNode.Attributes["decimal"].Value);
                }
                tcpClient.AddItem(item);
            }
            tcpClient.Enabled = true;
            //MessageBox.Show("dtuCient started");
        }

        private SerialPort creatSerialPort(string portName, string setting)
        {
            int i;
            int[] settingValues = { 9600, 8, 1, 0 };
            string[] subStrings = setting.Split(',');

            for (i = 0; i < subStrings.Length; i++)
            {
                try
                {
                    settingValues[i] = int.Parse(subStrings[i]);
                }
                catch (Exception)
                {
                }
            }

            int baudRate = settingValues[0];
            int databits = settingValues[1];

            StopBits stopBits;
            Parity parity;

            if (settingValues[2] == 0)
            {
                stopBits = StopBits.None;
            }
            else if (settingValues[2] == 1)
            {
                stopBits = StopBits.One;
            }
            else if (settingValues[2] == 2)
            {
                stopBits = StopBits.Two;
            }
            else
            {
                stopBits = StopBits.OnePointFive;
            }

            if (settingValues[3] == 0)
            {
                parity = Parity.None;
            }
            else if (settingValues[3] == 1)
            {
                parity = Parity.Odd;
            }
            else if (settingValues[3] == 2)
            {
                parity = Parity.Even;
            }
            else if (settingValues[3] == 3)
            {
                parity = Parity.Mark;
            }
            else
            {
                parity = Parity.Space;
            }
            SerialPort serialPort = new SerialPort(portName, baudRate, parity, databits, stopBits);
            serialPort.Open();
            return serialPort;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb.driver
{
    using System.IO.Ports;
    using System.Xml.Linq;

    using SysViewHyTurb.data;
    using SysViewHyTurb.Driver;
    using SysViewHyTurb.Driver.Device;
    using SysViewHyTurb.Driver.IO;
    using SysViewHyTurb.Driver.Utility;

    public class ModbusDriver
    {
        public ModbusDriver( XElement channElement)
        {
            var port = channElement.Attribute("port").Value;
            var setting = channElement.Attribute("setting").Value;
            var serialPort = this.CreatSerialPort(port, setting);
            var rtuTransport = new ModbusRtuTransport(new SerialPortAdapter(serialPort))
                                   {
                                       ReadTimeout = 3000,
                                       WriteTimeout = 3000
                                   };
            this.modbusMaster = new ModbusMaster(rtuTransport);

            foreach (var deviceElement in channElement.Elements("Device"))
            {
                this.modbusSlaves.Add(new ModbusSlave(deviceElement));
            }
        }

        public void ProcessRegisterGroup(ModbusSlave slave, ModbusRegGroup grp)
        {
            if (grp.CommStatus == 0 && grp.WaitTimes > 0)
            {
                grp.WaitTimes--;
            }
            else
            {
                try
                {
                    var results = this.modbusMaster.ReadHoldingRegisters(
                        (byte)slave.IDno,
                        (ushort)(grp.RegStart - Modbus.HoldingRegisterPLCStartAddress),
                        grp.RegNum);
                    grp.CommStatus = 0;
                    grp.WaitTimes = 0;
                    foreach (RegisterVariable reg in grp.RegVars)
                    {
                        if (reg.RegValueType == TypeCode.Single)
                        {
                            //MessageBox.Show(NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart], results[reg.RegID - grp.RegStart + 1]).ToString());
                            switch (reg.Swap)
                            {
                                case 1:
                                    reg.Value = NetworkUtility.GetSingle(
                                        results[reg.RegNo - grp.RegStart + 1],
                                        results[reg.RegNo - grp.RegStart]);
                                    break;
                                case 0:
                                    reg.Value = NetworkUtility.GetSingle(
                                        results[reg.RegNo - grp.RegStart],
                                        results[reg.RegNo - grp.RegStart + 1]);
                                    break;
                            }
                        }
                        else
                        {
                            reg.Value = results[reg.RegNo - grp.RegStart];
                        }
                    }
                }
                catch (Exception)
                {
                    grp.CommStatus = 1;
                    grp.WaitTimes = 4;
                }
            }
        }

        private SerialPort CreatSerialPort(string portName, string setting)
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

        public ModbusSlave[] ModbusSlaves { get { return this.modbusSlaves.ToArray(); } }

        private readonly List<ModbusSlave> modbusSlaves = new List<ModbusSlave>();

        private readonly ModbusMaster modbusMaster;
    }
}

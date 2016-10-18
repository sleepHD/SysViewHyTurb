using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using SysViewHyTurb.Driver.Utility;
using SysViewHyTurb.Driver.Device;
using SysViewHyTurb.Driver.IO;
using System.Windows.Forms;

namespace SysViewHyTurb.Driver
{
    public class ModbusReg
    {
        public ushort VarID 
        {
            get { return this.varID; }
            set { this.varID = value; }
        }

        public double RawMax
        {
            get { return this.rawMax; }
            set { this.rawMax = value; }
        }
        public double RawMin
        {
            get { return this.rawMin; }
            set { this.rawMin = value; }
        }
        public double VarMax
        {
            get { return this.varMax; }
            set { this.varMax = value; }
        }
        public double VarMin
        {
            get { return this.varMin; }
            set { this.varMin = value; }
        }
        public ushort Convert
        {
            get { return this.convert; }
            set { this.convert = value; }
        }
        public ushort RegID
        {
            get { return this.regID; }
            set { this.regID = value; }
        }
        public TypeCode RegType
        {
            get { return this.regType; }
            set { this.regType = value; }
        }
        public ushort RegSwap
        {
            get { return this.regSwap; }
            set { this.regSwap = value; }
        }

        private ushort varID;
        private double rawMax;
        private double rawMin;
        private double varMax;
        private double varMin;
        private ushort convert;
        private ushort regID;
        private TypeCode regType;
        private ushort regSwap;
    }

    public class ModbusRegGroup
    {    
        public int RegStart
        {
            get { return this.regStart;}
            private set { this.regStart = value; }
        }

        public ushort RegNum 
        {
            get { return this.regNum; }
            private set { this.regNum = value; }
        }

        public ushort CommStatus
        {
            get
            {
                return this.commStatus;
            }
            set
            {
                this.commStatus = value;
            }
        }

        public ushort WaitTimes
        {
            get { return this.waitTimes; }
            set { this.waitTimes = value; }
        }

        public ModbusReg[] ModbusRegs
        {
            get
            {
                return this.modbusRegList.ToArray();
            }
        }
        public ModbusRegGroup(ModbusReg var)
        {
            this.RegStart = var.RegID;
            this.AddReg(var);
        }

        public int AddReg(ModbusReg reg)
        {
            if (reg.RegID / 10000 == this.regStart / 10000)
            {
                if (reg.RegID < this.RegStart)
                {
                    this.RegNum = (ushort)(this.RegNum + (this.RegStart - reg.RegID));
                    this.RegStart = reg.RegID;
                }

                switch (reg.RegType)
                {
                    case TypeCode.Boolean:
                    case TypeCode.UInt16:
                    case TypeCode.Int16:
                        this.RegNum = (ushort)(this.RegStart + this.RegNum < reg.RegID + 1 ? reg.RegID - RegStart + 1 : RegNum);
                        break;
                    case TypeCode.UInt32:
                    case TypeCode.Int32:
                    case TypeCode.Single:
                        this.RegNum = (ushort)(this.RegStart + this.RegNum < reg.RegID + 2 ? reg.RegID - RegStart + 2 : RegNum);
                        break;
                }
                this.modbusRegList.Add(reg);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private int regStart;
        private ushort regNum;
        private ushort commStatus = 1;
        private ushort waitTimes;
        private List<ModbusReg> modbusRegList = new List<ModbusReg>();
    }
    
    public class ModbusSlave
    {
        public ushort IDno 
        {
            get { return this.iDno; }
            set { this.iDno = value; }
        }

        public ushort Status 
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public ModbusRegGroup[] VarGroups 
        {
            get { return this.varGroupList.ToArray();}
        }

        public ModbusSlave(ushort idNO)
        {
            this.IDno = idNO;
        }

        public void AddVar(ModbusReg var)
        {
            bool bAdd = false;
            int i;
            for (i = 0; i < this.varGroupList.Count; i++)
            {
                if (this.varGroupList[i].AddReg(var) == 1)
                {
                    bAdd = true;
                    break;
                }
            }
            if (!bAdd)
            {
                this.varGroupList.Add(new ModbusRegGroup(var));
            }
        }

        
        private ushort iDno;
        private ushort status;
        private List<ModbusRegGroup> varGroupList = new List<ModbusRegGroup>();
    }

    class ModbusPoller
    {
        public ModbusSlave[] ModbusSlaves { get { return this.modbusSlaves.ToArray(); } }
        
        public bool Enabled
        {
            get
            {
                return this.enabled;
            } 
            set
            {
                this.enabled = value;
                this.timer.Change(this.interval * 1000, this.interval * 1000);
            }
        }
        
        public ModbusPoller(SerialPort serialPort, DataRepo repo)
        {
            this.repo = repo;
            ModbusRtuTransport rtuTransport = new ModbusRtuTransport(new SerialPortAdapter(serialPort));
            rtuTransport.ReadTimeout = 3000;
            rtuTransport.WriteTimeout = 3000;
            this.modbusMaster = new ModbusMaster(rtuTransport);
            this.timer = new System.Threading.Timer(this.timerCallBack, null, Timeout.Infinite, this.interval * 1000);
        }

        public void AddSlave(ModbusSlave slave)
        {
            this.modbusSlaves.Add(slave);
        }

        public void timerCallBack(Object stateInfo)
        {
            this.timer.Change(Timeout.Infinite, this.interval * 1000);
            this.pollAll();
            this.timer.Change(this.interval * 1000, this.interval * 1000);
        }

        private void pollAll()
        {
           //MessageBox.Show("Poll All");
           foreach (ModbusSlave slv in this.modbusSlaves)
           {
               //MessageBox.Show("Poll Slave");
               foreach (ModbusRegGroup grp in slv.VarGroups)
               {
                   //MessageBox.Show("Poll group");
                   ushort[] results;

                   if (grp.CommStatus == 0 && grp.WaitTimes < 4)
                   {
                       ++(grp.WaitTimes);
                   }
                   else
                   {
                       grp.WaitTimes = 0;
                       if (grp.RegStart >= Modbus.HoldingRegisterPLCStartAddress && grp.RegStart <= Modbus.HoldingRegisterPLCEndAddress)
                       {
                           try
                           {

                               results = this.modbusMaster.ReadHoldingRegisters((byte)slv.IDno, (ushort)(grp.RegStart - Modbus.HoldingRegisterPLCStartAddress), grp.RegNum);
                               grp.CommStatus = 1;
                               grp.WaitTimes = 0;
                               foreach (ModbusReg reg in grp.ModbusRegs)
                               {
                                   if (reg.RegType == TypeCode.Single)
                                   {
                                       //MessageBox.Show(NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart], results[reg.RegID - grp.RegStart + 1]).ToString());
                                       if (reg.RegSwap == 1)
                                           this.repo.InputRegisters[reg.VarID] = NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart + 1], results[reg.RegID - grp.RegStart]);
                                       else if (reg.RegSwap == 0)
                                           this.repo.InputRegisters[reg.VarID] = NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart], results[reg.RegID - grp.RegStart + 1]);
                                   }
                                   else
                                   {
                                       this.repo.InputRegisters[reg.VarID] = results[reg.RegID];
                                   }

                                   if (reg.Convert > 0)
                                   {
                                       this.repo.InputRegisters[reg.VarID] = (float)((reg.VarMax - reg.VarMin) / (reg.RawMax - reg.RawMin) * (this.repo.InputRegisters[reg.VarID] - reg.RawMin) + reg.VarMin);
                                       this.repo.InputRegisters[reg.VarID] = (float)(this.repo.InputRegisters[reg.VarID] > reg.VarMax ? reg.VarMax : this.repo.InputRegisters[reg.VarID]);
                                       this.repo.InputRegisters[reg.VarID] = (float)(this.repo.InputRegisters[reg.VarID] < reg.VarMin ? reg.VarMin : this.repo.InputRegisters[reg.VarID]);
                                   }
                                   //MessageBox.Show(this.repo.InputRegisters[reg.VarID].ToString());
                               }
                           }
                           catch (Exception)
                           {
                               grp.CommStatus = 0;
                               grp.WaitTimes = 0;
                           }

                       }
                       else if (grp.RegStart >= Modbus.InputPLCStartAddress && grp.RegStart <= Modbus.InputPLCEndAddress)
                       {

                           try
                           {
                               results = this.modbusMaster.ReadInputRegisters((byte)slv.IDno, (ushort)(grp.RegStart - Modbus.InputPLCStartAddress + 1), grp.RegNum);
                               grp.CommStatus = 1;
                               grp.WaitTimes = 0;
                               foreach (ModbusReg reg in grp.ModbusRegs)
                               {
                                   if (reg.RegType == TypeCode.Single)
                                   {
                                       if (reg.RegSwap == 1)
                                           this.repo.InputRegisters[reg.VarID] = NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart], results[reg.RegID - grp.RegStart + 1]);
                                       else if (reg.RegSwap == 0)
                                           this.repo.InputRegisters[reg.VarID] = NetworkUtility.GetSingle(results[reg.RegID - grp.RegStart + 1], results[reg.RegID - grp.RegStart]);
                                   }
                                   else
                                   {
                                       this.repo.InputRegisters[reg.VarID] = results[reg.RegID];
                                   }
                                   if (reg.Convert > 0)
                                   {
                                       this.repo.InputRegisters[reg.VarID] = (float)((reg.VarMax - reg.VarMin) / (reg.RawMax - reg.RawMin) * (this.repo.InputRegisters[reg.VarID] - reg.RawMin) + reg.VarMin);
                                       this.repo.InputRegisters[reg.VarID] = (float)(this.repo.InputRegisters[reg.VarID] > reg.VarMax ? reg.VarMax : this.repo.InputRegisters[reg.VarID]);
                                       this.repo.InputRegisters[reg.VarID] = (float)(this.repo.InputRegisters[reg.VarID] < reg.VarMin ? reg.VarMin : this.repo.InputRegisters[reg.VarID]);
                                   }
                               }
                           }
                           catch (Exception)
                           {
                               grp.CommStatus = 0;
                               grp.WaitTimes = 0;
                           }
                       }
                   }
               }
           }
        }

        private int interval = 2;
        private DataRepo repo;
        private System.Threading.Timer timer;
        private bool enabled;
        private ModbusMaster modbusMaster;
        private List<ModbusSlave> modbusSlaves = new List<ModbusSlave>();

    }
}

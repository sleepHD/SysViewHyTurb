using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb.driver
{
    using SysViewHyTurb.data;
    using SysViewHyTurb.Driver;
    using SysViewHyTurb.Driver.Data;

    public class ModbusRegGroup
    {
        /// <summary>
        /// Gets or sets the register type of the process variable
        /// </summary>
        public ushort RegType { get; set; }

        public int RegStart
        {
            get { return this.regStart; }
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

        public bool WriteRead { get; set; }

        public ushort Freq { get; set; }

        public ushort WaitTimes { get; set; }

        public RegisterVariable[] RegVars
        {
            get
            {
                return this.modbusRegList.ToArray();
            }
        }
        public ModbusRegGroup(RegisterVariable reg)
        {
            if (ConvertRegister(reg) > 0)
            {
                this.RegStart = reg.RegNo;
                this.RegType = reg.RegType;
                this.regNum = 1;
                this.AddReg(reg);
            }
        }

        public int AddReg(RegisterVariable reg)
        {
            if (ConvertRegister(reg) > 0 && reg.RegType == this.RegType)
            {
                if (reg.RegNo < this.RegStart)
                {
                    this.RegNum = (ushort)(this.RegNum + (this.RegStart - reg.RegNo));
                    this.RegStart = reg.RegNo;
                }

                switch (reg.RegValueType)
                {
                    case TypeCode.Boolean:
                    case TypeCode.UInt16:
                    case TypeCode.Int16:
                        this.RegNum = (ushort)(this.RegStart + this.RegNum < reg.RegNo + 1 ? reg.RegNo - RegStart + 1 : RegNum);
                        break;
                    case TypeCode.UInt32:
                    case TypeCode.Int32:
                    case TypeCode.Single:
                        this.RegNum = (ushort)(this.RegStart + this.RegNum < reg.RegNo + 2 ? reg.RegNo - RegStart + 2 : RegNum);
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

        public int ConvertRegister(RegisterVariable reg)
        {
            var regAddr = ushort.Parse(reg.RegName);
            reg.RegNo = (ushort)regAddr;
            reg.RegType = (ushort)(regAddr / 10000);
            return 1;
        }

        private int regStart;
        private ushort regNum;
        private ushort commStatus = 1;

        private readonly List<RegisterVariable> modbusRegList = new List<RegisterVariable>();
    }
    
  

}

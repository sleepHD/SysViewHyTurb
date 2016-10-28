using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb.driver
{
    using System.Xml.Linq;

    using SysViewHyTurb.data;
    using SysViewHyTurb.Driver;

    public class ModbusSlave
    {

        public string Addr { get; set; }

        public ushort IDno { get; set; }

        public ushort Status { get; set; }

        public ModbusRegGroup[] VarGroups
        {
            get { return this.varGroupList.ToArray(); }
        }

        public ModbusSlave(XElement slaveElement)
        {
            this.Addr = slaveElement.Attribute("Address").Value;
            this.IDno = ushort.Parse(this.Addr);

            foreach (var varElement in slaveElement.Elements("PV"))
            {
                this.AddVar(new RegisterVariable(varElement));
            }
        }

        public void AddVar(RegisterVariable var)
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

        private readonly List<ModbusRegGroup> varGroupList = new List<ModbusRegGroup>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb.data
{
    using System.Xml.Linq;

    public class RegisterVariable
    {
        /// <summary>
        /// Gets or sets the name of the process variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the register name of the process variable
        /// </summary>
        public string RegName { get; set; }

        /// <summary>
        /// Gets or sets the register type of the process variable
        /// </summary>
        public ushort RegType { get; set; }

        /// <summary>
        /// Gets or sets the register type of the process variable
        /// </summary>
        public TypeCode RegValueType { get; set; }

        /// <summary>
        /// Gets or sets the register number of the process variable
        /// </summary>
        public ushort RegNo { get; set; }

        /// <summary>
        /// Gets or sets the read rate of the process variable
        /// </summary>
        public int UpdateRate { get; set; }

        /// <summary>
        /// Gets or sets the data type of the process variable
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets swap behavior taken by the of the process variable
        /// </summary>
        public int Swap { get; set; }

        /// <summary>
        /// Gets or sets the value of the register variable
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the length of the process variable
        /// </summary>
        public int Length { get; set; }

        public RegisterVariable(XElement varElement)
        {
            Name = varElement.Attribute("name").Value;
            RegName = varElement.Attribute("regName").Value;
            this.RegValueType = (TypeCode)Enum.Parse(typeof(TypeCode), varElement.Attribute("regValueType").Value);
            //Length = int.Parse(varElement.Attribute("Length").Value);
            UpdateRate = int.Parse(varElement.Attribute("updateRate").Value);
            //Swap = int.Parse(varElement.Attribute("Swap").Value);
        }
    }
}

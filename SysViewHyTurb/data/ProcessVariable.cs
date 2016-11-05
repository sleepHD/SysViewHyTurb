
namespace SysViewHyTurb.data
{
    using System;
    using System.Xml.Linq;

    public class ProcessVariable
    {
        /// <summary>
        /// Process variable of the device
        /// </summary>
        private readonly string name;

        /// <summary>
        /// process variable value
        /// </summary>
        private object value;

        /// <summary>
        /// process variable raw value
        /// </summary>
        private object rawValue;

        /// <summary>
        /// process variable value type
        /// </summary>
        private TypeCode valueType;

        /// <summary>
        /// conversion indicator of the process variable
        /// </summary>
        private bool convert;

        /// <summary>
        /// low raw value of the process variable
        /// </summary>
        private double minRawValue;

        /// <summary>
        /// high raw value
        /// </summary>
        private double maxRawValue;

        /// <summary>
        /// low value of the process variable
        /// </summary>
        private double minValue;

        /// <summary>
        /// high value
        /// </summary>
        private double maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessVariable"/> class
        /// </summary>
        /// <param name="processVarElement">process variable configuration XElement</param>
        public ProcessVariable(XElement processVarElement)
        {
            this.name = processVarElement.Attribute("name").Value;
            this.valueType = (TypeCode)Enum.Parse(typeof(TypeCode), processVarElement.Attribute("valueType").Value);
            this.value = System.Convert.ChangeType(processVarElement.Attribute("initValue").Value, this.valueType);
            this.convert = bool.Parse(processVarElement.Attribute("convert").Value);
            this.maxRawValue = double.Parse(processVarElement.Attribute("maxRawValue").Value);
            this.minRawValue = double.Parse(processVarElement.Attribute("minRawValue").Value);
            this.maxValue = double.Parse(processVarElement.Attribute("maxValue").Value);
            this.minValue = double.Parse(processVarElement.Attribute("minValue").Value);
        }

        /// <summary>
        /// Gets process value name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets or sets process variable value
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.rawValue = this.CalcRawValue();
            }
        }

        /// <summary>
        /// Gets or sets process variable value
        /// </summary>
        public object RawValue
        {
            get
            {
                return this.rawValue;
            }

            set
            {
                this.rawValue = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Gets or sets the value type of the process variable 
        /// </summary>
        public TypeCode ValueType
        {
            get
            {
                return this.valueType;
            }

            set
            {
                this.valueType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a value conversion is performed on the process variable
        /// </summary>
        public bool Convert
        {
            get
            {
                return this.convert;
            }

            set
            {
                this.convert = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Gets or sets the low raw value of the process variable
        /// </summary>
        public double MinRawValue
        {
            get
            {
                return this.minRawValue;
            }

            set
            {
                this.minRawValue = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Gets or sets the high raw value of the process variable
        /// </summary>
        public double MaxRawValue
        {
            get
            {
                return this.maxRawValue;
            }

            set
            {
                this.maxRawValue = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Gets or sets the low value of the process variable
        /// </summary>
        public double MinValue
        {
            get
            {
                return this.minValue;
            }

            set
            {
                this.minValue = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Gets or sets the high value of the process variable
        /// </summary>
        public double MaxValue
        {
            get
            {
                return this.maxValue;
            }

            set
            {
                this.maxValue = value;
                this.value = this.CalcValue();
            }
        }

        /// <summary>
        /// Calculates the value of the process variable from raw value
        /// </summary>
        /// <returns>the calculated value</returns>
        private object CalcValue()
        {
            object result = this.rawValue;
            if (this.Convert)
            {
                result = (((float)this.rawValue - this.MinRawValue) * (this.MaxValue - this.MinValue) / (this.MaxRawValue - this.MinRawValue))
                     + this.MinValue;
            }

            return result;
        }

        /// <summary>
        /// Calculates the raw value of the process variable from value
        /// </summary>
        /// <returns>The calculated raw value</returns>
        private object CalcRawValue()
        {
            var result = this.value;
            if (this.Convert)
            {
                result = (((double)this.Value - this.MinValue) * (this.MaxRawValue - this.MinRawValue)
                         / (this.MaxValue - this.MinValue)) + this.maxRawValue;
            }

            return result;
        }
        
    }
}

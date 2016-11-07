//-----------------------------------------------------------------------
// <copyright file="ReadHoldingInputRegistersResponse.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Message
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using SysViewHyTurb.Driver.Data;

    /// <summary>
    /// Read holding or input register response message
    /// </summary>
    public class ReadHoldingInputRegistersResponse : ModbusMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingInputRegistersResponse"/> class
        /// </summary>
        public ReadHoldingInputRegistersResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingInputRegistersResponse"/> class
        /// </summary>
        /// <param name="functionCode">Takes function code</param>
        /// <param name="slaveAddress">Takes the address of the slave device</param>
        /// <param name="data">Response message data</param>
        public ReadHoldingInputRegistersResponse(byte functionCode, byte slaveAddress, RegisterCollection data)
            : base(slaveAddress, functionCode)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.ByteCount = data.ByteCount;
            this.Data = data;
        }

        /// <summary>
        /// Gets the minimum frame size of the read coils or inputs response message
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets or sets the register collection data of the read coils or inputs response message
        /// </summary>
        public new RegisterCollection Data
        {
            get
            {
                return (RegisterCollection)base.Data;
            }

            set
            {
                base.Data = value;
            }
        }

        /// <summary>
        /// Message function information
        /// </summary>
        /// <returns>Message information string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Read {0} {1} registers.", this.Data.Count, this.FunctionCode == Modbus.ReadHoldingRegisters ? "holding" : "input");
        }

        /// <summary>
        /// Initializes the message specific part of a  a message object
        /// </summary>
        /// <param name="frame">Take a byte array received from underlying transport</param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < this.MinimumFrameSize + frame[2])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }
            
            this.ByteCount = frame[2];
            byte[] dataFrame = new byte[this.ByteCount.Value];

            for (int i = 0; i < ByteCount.Value; i++)
            {
                dataFrame[i] = frame[i + 3];
            }
            
            this.Data = new RegisterCollection(dataFrame);
        }
    }
}

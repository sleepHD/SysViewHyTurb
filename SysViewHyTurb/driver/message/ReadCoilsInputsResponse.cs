//-----------------------------------------------------------------------
// <copyright file="ReadCoilsInputsResponse.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Message
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using SysViewHyTurb.Driver.Data;

    /// <summary>
    /// Read coils or inputs response message
    /// </summary>
    public class ReadCoilsInputsResponse : ModbusMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsInputsResponse"/> class
        /// </summary>
        public ReadCoilsInputsResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsInputsResponse"/> class
        /// </summary>
        /// <param name="functionCode">Takes function code</param>
        /// <param name="slaveAddress">Takes the address of the slave device</param>
        /// <param name="byteCount">Takes the byte count of the response message data</param>
        /// <param name="data">Response message data</param>
        public ReadCoilsInputsResponse(byte functionCode, byte slaveAddress, byte byteCount,  DiscreteCollection data)
            : base(slaveAddress, functionCode)
        {
            this.ByteCount = byteCount;
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
        /// Gets or sets the discrete collection data of the read coils or inputs response message
        /// </summary>
        public new DiscreteCollection Data
        {
            get
            {
                return (DiscreteCollection)base.Data;
            }

            set
            {
                base.Data = value;
            }
        }

        /// <summary>
        /// Message function information
        /// </summary>
        /// <returns>Message function information string</returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Read {0} {1} - {2}.",
                this.Data.Count(),
                this.FunctionCode == Modbus.ReadInputs ? "inputs" : "coils",
                this.Data);
        }

        /// <summary>
        /// Initializes the message specific part of a  a message object
        /// </summary>
        /// <param name="frame">Take a byte array received from underlying transport</param>
        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < 3 + frame[2])
            {
                throw new FormatException("Message frame data segment does not contain enough bytes.");
            }

            this.ByteCount = frame[2];
            byte[] dataFrame = new byte[ByteCount.Value];

            for (int i = 0; i < ByteCount.Value; i++)
            {
                dataFrame[i] = frame[i + 3];
            }

            this.Data = new DiscreteCollection(dataFrame);
        }
    }
}

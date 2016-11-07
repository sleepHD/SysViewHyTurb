//-----------------------------------------------------------------------
// <copyright file="ReadCoilsInputsRequest.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Message
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    
    /// <summary>
    /// Read coils or inputs request message
    /// </summary>
    public class ReadCoilsInputsRequest : ModbusMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsInputsRequest"/> class
        /// </summary>
        public ReadCoilsInputsRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsInputsRequest"/> class
        /// </summary>
        /// <param name="functionCode">Takes function code</param>
        /// <param name="slaveAddress">Takes the address of the slave device to read</param>
        /// <param name="startAddress">Takes start address to read</param>
        /// <param name="numberOfPoints">Takes number of points to read</param>
        public ReadCoilsInputsRequest(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(slaveAddress, functionCode)
        {
            this.StartAddress = startAddress;
            this.NumberOfPoints = numberOfPoints;
        }

        /// <summary>
        /// Gets the minimum frame size
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 6; }
        }

        /// <summary>
        /// Message function information
        /// </summary>
        /// <returns>Message information string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Read {0} {1} starting at address {2}.", this.NumberOfPoints, this.FunctionCode == Modbus.ReadCoils ? "coils" : "inputs", this.StartAddress);
        }

        /// <summary>
        /// Initializes the message specific part of a  a message object
        /// </summary>
        /// <param name="frame">Take a byte array received from underlying transport</param>
        protected override void InitializeUnique(byte[] frame)
        {
            this.StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            this.NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        }  
    }
}

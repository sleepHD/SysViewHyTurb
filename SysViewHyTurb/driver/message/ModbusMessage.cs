//-----------------------------------------------------------------------
// <copyright file="ModbusMessage.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Message
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Modbus message class
    /// </summary>
    public abstract class ModbusMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusMessage"/> class
        /// </summary>
        public ModbusMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusMessage"/> class
        /// </summary>
        /// <param name="slaveAddress">Takes slave address</param>
        /// <param name="functionCode">Takes function code</param>
        public ModbusMessage(byte slaveAddress, byte functionCode)
        {
            this.SlaveAddress = slaveAddress;
            this.FunctionCode = functionCode; 
        }

        /// <summary>
        /// Gets the minimum frame size of a specific modbus message
        /// </summary>
        public abstract int MinimumFrameSize { get; }

        /// <summary>
        /// Initializes a message object (common part for all modbus message)
        /// </summary>
        /// <param name="frame">Take a byte array received from underlying transport</param>
        public override void Initialize(byte[] frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame", "Argument frame cannot be null.");
            }

            if (frame.Length < Modbus.MinimumFrameSize)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Message frame must contain at least {0} bytes of data.", Modbus.MinimumFrameSize));
            }

            this.SlaveAddress = frame[0];
            this.FunctionCode = frame[1];
            this.InitializeUnique(frame);
        }

        /// <summary>
        /// Initializes a message object (message specific part, implemented by derived classes)
        /// </summary>
        /// <param name="frame">Take a byte array received from underlying transport</param>
        protected abstract void InitializeUnique(byte[] frame);
    }
}

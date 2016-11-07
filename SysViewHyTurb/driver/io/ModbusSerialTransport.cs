//-----------------------------------------------------------------------
// <copyright file="SerialMasterTransport.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using SysViewHyTurb.Driver.Message;

    /// <summary>
    /// Abstract serial port master transport
    /// </summary>
    public abstract class ModbusSerialTransport : ModbusTransport
    {
        private bool _checkFrame = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialMasterTransport"/> class
        /// </summary>
        /// <param name="serialPortAdapter">SerialPortAdapter implemented IStreamResource</param>
        public ModbusSerialTransport(IStreamResource serialPortAdapter)
            : base(serialPortAdapter)
        {
            Debug.Assert(serialPortAdapter != null, "Argument serialPortAdapter cannot be null.");
            
        }

        /// <summary>
        /// Gets or sets a value indicating whether LRC/CRC frame checking is performed on messages.
        /// </summary>
        public bool CheckFrame
        {
            get { return _checkFrame; }
            set { _checkFrame = value; }
        }

        /// <summary>
        ///  SerialPort.Read exits immediately though there are less bytes than as specified by the count parameter,this method  handles that
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
                numBytesRead += StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);

            return frameBytes;
        }

        /// <summary>
        /// Override this method to check sums match 
        /// </summary>
        /// <param name="message">Message received</param>
        /// <param name="messageFrame">Byte array received</param>
        /// <returns>Check result</returns>
        public virtual bool ChecksumsMatch(Message message, byte[] messageFrame)
        {
            return true;
        }

    }
}

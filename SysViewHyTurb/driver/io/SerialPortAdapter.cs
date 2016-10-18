//-----------------------------------------------------------------------
// <copyright file="SerialPortAdapter.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Text; 

    /// <summary>
    /// Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class SerialPortAdapter : IStreamResource
    {
        /// <summary>
        /// private serial port
        /// </summary>
        private SerialPort serialPort;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortAdapter"/> class
        /// </summary>
        /// <param name="serialPort">a construct takes a serial port </param>
        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            this.serialPort = serialPort;
            this.ReadTimeout = 1000;
            this.WriteTimeout = 1000;
            // _serialPort.NewLine = Modbus.NewLine;
        }

        /// <summary>
        /// Gets what ? Indicates that no timeout should occur.
        /// </summary>
        public int InfiniteTimeout
        {
            get { return SerialPort.InfiniteTimeout; }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a read operation does not finish.
        /// </summary>
        public int ReadTimeout
        {
            get { return this.serialPort.ReadTimeout; }
            set { this.serialPort.ReadTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a write operation does not finish.
        /// </summary>
        public int WriteTimeout
        {
            get { return this.serialPort.WriteTimeout; }
            set { this.serialPort.WriteTimeout = value; }
        }

        /// <summary>
        /// Purges the receive buffer.
        /// </summary>
        public void DiscardInBuffer()
        {
            this.serialPort.DiscardInBuffer();
        }

        /// <summary>
        /// Reads a number of bytes from the input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="offset">The offset in the buffer array to begin writing.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.serialPort.Read(buffer, offset, count);
        }

        /// <summary>
        /// Writes a specified number of bytes to the port from an output buffer, starting at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The offset in the buffer array to begin writing.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            this.serialPort.Write(buffer, offset, count);
        }

        public void Connect()
        {
            this.serialPort.Open();
        }

        public bool Connected
        {
            get
            {
                return this.serialPort.IsOpen;
            }
        }

        public string Port
        {
            get
            {
                return this.serialPort.PortName;
            }
        }
    }
}

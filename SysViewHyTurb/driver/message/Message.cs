//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Message
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using SysViewHyTurb.Driver.Data;

    /// <summary>
    /// Base message class indicating common message construction
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Gets or sets address of the slave device (if any)
        /// </summary>
        public byte? SlaveAddress 
        {
            get { return this.slaveAddress; }
            set { this.slaveAddress = value; }
        }

        /// <summary>
        /// Gets or sets function code of the specific message (if any)
        /// </summary>
        public byte? FunctionCode
        {
            get { return this.functionCode; }
            set { this.functionCode = value; }
        }

        /// <summary>
        /// Gets or sets byte count of the specific message (if any)
        /// </summary>
        public byte? ByteCount
        {
            get { return this.byteCount; }
            set { this.byteCount = value; }
        }

        /// <summary>
        /// In TCP MbapHeader
        /// </summary>
        public ushort? TransactionId
        {
            get { return this.transactionId; }
            set { this.transactionId = value; }
        }

        /// <summary>
        /// Gets or sets exception code of the specific message (if any)
        /// </summary>
        public byte? ExceptionCode
        {
            get { return this.exceptionCode; }
            set { this.exceptionCode = value; }
        }

        /// <summary>
        /// Gets or sets number of points of the specific message (if any)
        /// </summary>
        public ushort? NumberOfPoints
        {
            get { return this.numberOfPoints; }
            set { this.numberOfPoints = value; }
        }

        /// <summary>
        /// Gets or sets start address of the specific message (if any)
        /// </summary>
        public ushort? StartAddress
        {
            get { return this.startAddress; }
            set { this.startAddress = value; }
        }

        /// <summary>
        /// Gets or sets subFunctionCode of the specific message (if any)
        /// </summary>
        public ushort? SubFunctionCode
        {
            get { return this.subFunctionCode; }
            set { this.subFunctionCode = value; }
        }

        /// <summary>
        /// Gets or sets the data part of the specific message (if any)
        /// </summary>
        public IDataCollection Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        /// <summary>
        /// Gets the whole message in plain byte array form including slave address (if any) and protocol data unit
        /// Builds message for the underlying transport layer
        /// </summary>
        public byte[] MessageFrame 
        {
            get
            {
                List<byte> frame = new List<byte>();
                if (this.SlaveAddress.HasValue)
                {
                    frame.Add(this.SlaveAddress.Value);
                }

                frame.AddRange(this.ProtocolDataUnit);
                return frame.ToArray();
            }
        }

        /// <summary>
        /// Gets the protocol data unit part of the message including function code (if any) and data (if any) 
        /// And other protocol specific part
        /// Builds the protocol data part of message
        /// Delegates actual building to derived classes
        /// </summary>
        public byte[] ProtocolDataUnit 
        {
            get
            {
                List<byte> pdu = new List<byte>();

                if (this.FunctionCode.HasValue)
                {
                    pdu.Add(this.FunctionCode.Value);
                }

                if (this.ExceptionCode.HasValue)
                {
                    pdu.Add(this.ExceptionCode.Value);
                }

                if (this.SubFunctionCode.HasValue)
                {
                    pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.SubFunctionCode.Value)));
                }

                if (this.StartAddress.HasValue)
                {
                    pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.StartAddress.Value)));
                }

                if (this.NumberOfPoints.HasValue)
                {
                    pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.NumberOfPoints.Value)));
                }

                if (this.ByteCount.HasValue)
                {
                    pdu.Add(this.ByteCount.Value);
                }

                if (this.Data != null)
                {
                    pdu.AddRange(this.Data.NetworkBytes);
                }

                return pdu.ToArray();
            }
         }
               
        /// <summary>
        /// Initializes message object from plain byte array received by the underlying transport layer
        /// Parses message 
        /// </summary>
        /// <param name="frame">byte array received by the underlying transport layer </param>
        public abstract void Initialize(byte[] frame);

        private byte? slaveAddress;

        private byte? functionCode;


        private byte? byteCount;


        private ushort? transactionId;


        private byte? exceptionCode;


        private ushort? numberOfPoints;


        private ushort? startAddress;


        private ushort? subFunctionCode;

        private IDataCollection data;

    }
}

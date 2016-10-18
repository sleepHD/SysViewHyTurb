//-----------------------------------------------------------------------
// <copyright file="SlaveExceptionResponse.cs" company="SysTek">
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

    class SlaveExceptionResponse : ModbusMessage
    {
        private static readonly Dictionary<byte, string> _exceptionMessages = CreateExceptionMessages();

        public SlaveExceptionResponse()
        {
        }

        public SlaveExceptionResponse(byte slaveAddress, byte functionCode, byte exceptionCode)
            : base(slaveAddress, functionCode)
        {
            SlaveExceptionCode = exceptionCode;
        }

        public override int MinimumFrameSize
        {
            get { return 3; }
        }

        public byte SlaveExceptionCode 
        { 
            get 
            { 
                return this.slaveExceptionCode; 
            } 
            set 
            { 
                this.slaveExceptionCode = value; 
            } 
        }
        private byte slaveExceptionCode;

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            string message = _exceptionMessages.ContainsKey(SlaveExceptionCode) ? _exceptionMessages[SlaveExceptionCode] : "Unknown";
            return String.Format(CultureInfo.InvariantCulture, "Function Code: {1}{0} Exception Code:{2}-{3}", "\n\r", FunctionCode, SlaveExceptionCode, message);
        }

        internal static Dictionary<byte, string> CreateExceptionMessages()
        {
            Dictionary<byte, string> messages = new Dictionary<byte, string>(9);

            messages.Add(1, "IllegalFunction");
            messages.Add(2, "IllegalDataAddress");
            messages.Add(3, "IllegalDataValue");
            messages.Add(4, "SlaveDeviceFailure");
            messages.Add(5, "Acknowlege");
            messages.Add(6, "SlaveDeviceBusy");
            messages.Add(8, "MemoryParityError");
            messages.Add(10, "GatewayPathUnavailable");
            messages.Add(11, "GatewayTargetDeviceFailedToRespond");

            return messages;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (FunctionCode <= Modbus.ExceptionOffset)
                throw new FormatException("SlaveExceptionResponseInvalidFunctionCode");

            SlaveExceptionCode = frame[2];
        }
    }
}

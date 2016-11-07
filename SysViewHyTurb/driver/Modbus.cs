//-----------------------------------------------------------------------
// <copyright file="Modbus.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver
{
    /// <summary>
    ///  Defines constants related to the Modbus protocol.
    /// </summary>
    internal static class Modbus
    {
        /// <summary>
        /// Read coils function code
        /// </summary>
        public const byte ReadCoils = 1;

        /// <summary>
        /// Read inputs function code
        /// </summary>
        public const byte ReadInputs = 2;

        /// <summary>
        /// Read holding registers function code
        /// </summary>
        public const byte ReadHoldingRegisters = 3;

        /// <summary>
        /// Read input registers function code
        /// </summary>
        public const byte ReadInputRegisters = 4;

        /// <summary>
        /// Write single coil function code
        /// </summary>
        public const byte WriteSingleCoil = 5;

        /// <summary>
        /// Write single register function code
        /// </summary>
        public const byte WriteSingleRegister = 6;

        /// <summary>
        /// Diagnostics function code
        /// </summary>
        public const byte Diagnostics = 8;

        /// <summary>
        /// Diagnostics return query data function code
        /// </summary>
        public const ushort DiagnosticsReturnQueryData = 0;

        /// <summary>
        /// Write multiple coils function code
        /// </summary>
        public const byte WriteMultipleCoils = 15;

        /// <summary>
        /// Write multiple registers function code
        /// </summary>
        public const byte WriteMultipleRegisters = 16;

        /// <summary>
        /// Read write multiple registers function code
        /// </summary>
        public const byte ReadWriteMultipleRegisters = 23;

        /// <summary>
        /// Maximum discrete request and response size
        /// </summary>
        public const int MaximumDiscreteRequestResponseSize = 2040;

        /// <summary>
        /// Maximum register request and response size
        /// </summary>
        public const int MaximumRegisterRequestResponseSize = 127;

        /// <summary>
        /// modbus slave exception offset that is added to the function code, to flag an exception
        /// </summary>
        public const byte ExceptionOffset = 128;

        /// <summary>
        /// modbus slave exception codes
        /// </summary>
        public const byte Acknowledge = 5;

        /// <summary>
        /// slave device busy code
        /// </summary>
        public const byte SlaveDeviceBusy = 6;

        /// <summary>
        /// default setting for number of retries for IO operations
        /// </summary>
        public const int DefaultRetries = 3;

        /// <summary>
        /// default number of milliseconds to wait after encountering an ACKNOWLEGE or SLAVE DEVIC BUSY slave exception response.
        /// </summary>
        public const int DefaultWaitToRetryMilliseconds = 250;

        /// <summary>
        /// default setting for IO timeouts in milliseconds
        /// </summary>
        public const int DefaultTimeout = 1000;

        /// <summary>
        /// smallest supported message frame size (sans checksum)
        /// </summary>
        public const int MinimumFrameSize = 2;

        /// <summary>
        /// indicates coil on (modbus protocol)
        /// </summary>
        public const ushort CoilOn = 0xFF00;

        /// <summary>
        /// indicates coil off (modbus protocol)
        /// </summary>
        public const ushort CoilOff = 0x0000;

        /// <summary>
        /// IP slaves should be addressed by IP
        /// </summary>
        public const byte DefaultIpSlaveUnitId = 0;

        /// <summary>
        /// An existing connection was forcibly closed by the remote host
        /// </summary>
        public const int ConnectionResetByPeer = 10054;

        /// <summary>
        ///  Existing socket connection is being closed
        /// </summary>
        public const int WSACancelBlockingCall = 10004;

        /// <summary>
        /// used by the ASCII transport to indicate end of message
        /// </summary>
        public const string NewLine = "\r\n";

        public const int CoilPLCStartAddress = 1;

        public const int CoilPLCEndAddress = 9999;

        public const int InputPLCStartAddress = 10001;

        public const int InputPLCEndAddress = 19999;

        public const int HoldingRegisterPLCStartAddress = 40001;

        public const int HoldingRegisterPLCEndAddress = 49999;

        public const int InputRegisterPLCStartAddress = 30001;

        public const int InputRegisterPLCEndAddress = 39999;

        public const int WriteMultipleCoilsPLCStartAddress = 80001;

        public const int WriteMultipleCoilsPLCEndAddress = 89999;

        public const int WriteMultipleRegistersPLCStartAddress = 90001;

        public const int WriteMultipleRegistersPLCEndAddress = 99999;

    }
}

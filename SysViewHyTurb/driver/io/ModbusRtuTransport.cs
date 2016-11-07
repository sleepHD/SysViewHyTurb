namespace SysViewHyTurb.Driver.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using SysViewHyTurb.Driver.Message;
    using SysViewHyTurb.Driver.Utility;
    
    /// <summary>
    /// Modbus RTU master transport (serial port) class
    /// </summary>
    public class ModbusRtuTransport : ModbusSerialTransport 
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(ModbusRtuMasterTransport));
        
        public const int ResponseFrameStartLength = 4;

        public ModbusRtuTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public static int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            // exception response
            if (functionCode > Modbus.ExceptionOffset)
                return 1;

            int numBytes;
            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                    numBytes = frameStart[2] + 1;
                    break;
                case Modbus.WriteSingleCoil:
                case Modbus.WriteSingleRegister:
                case Modbus.WriteMultipleCoils:
                case Modbus.WriteMultipleRegisters:
                case Modbus.Diagnostics:
                    numBytes = 4;
                    break;
                default:
                    string errorMessage = String.Format(CultureInfo.InvariantCulture, "Function code {0} not supported.", functionCode);
                    throw new NotImplementedException(errorMessage);
            }

            return numBytes;
        }

        internal virtual Message CreateResponse<T>(byte[] frame) where T : Message, new()
        {
            byte functionCode = frame[1];
            Message response;

            // check for slave exception response else create message from frame
            if (functionCode > Modbus.ExceptionOffset)
            {
                response = new SlaveExceptionResponse();
                response.Initialize(frame);
            }
            else
            {
                response = new T();
                response.Initialize(frame);    
            }

            if (this.CheckFrame && !ChecksumsMatch(response, frame))
            {
                string errorMessage = String.Format(CultureInfo.InvariantCulture, "Checksums failed to match {0} != {1}", BitConverter.ToString(response.MessageFrame), BitConverter.ToString(frame));
                throw new IOException(errorMessage);
            }
            return (T)response;
        }

        public override byte[] BuildMessageFrame(Message message)
		{
			List<byte> messageBody = new List<byte>();
			messageBody.Add(message.SlaveAddress.Value);
			messageBody.AddRange(message.ProtocolDataUnit);
			messageBody.AddRange(NetworkUtility.CalculateCrc(message.MessageFrame));
            //_logger.InfoFormat("TX: {0}", BitConverter.ToString(messageBody.ToArray()));
			return messageBody.ToArray();
		}

		public override bool ChecksumsMatch(Message message, byte[] messageFrame)
		{
            return BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2) == BitConverter.ToUInt16(NetworkUtility.CalculateCrc(message.MessageFrame), 0);
		}

		public override Message ReadResponse<T>() 
		{
            try
            {
                byte[] frameStart = Read(ResponseFrameStartLength);
                byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
                byte[] frame = frameStart.Concat(frameEnd).ToArray();
                //Transport.logger.Debug(this.StreamResource.Port + "-RX: " + BitConverter.ToString(frame));
                return CreateResponse<T>(frame);
            }
            catch (Exception e)
            {
                //Transport.logger.Debug(e.Message);
                throw e;
            }
		}
    }
}

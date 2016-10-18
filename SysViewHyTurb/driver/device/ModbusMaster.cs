//-----------------------------------------------------------------------
// <copyright file="ModbusMaster.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Device
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using SysViewHyTurb.Driver.Data;
    using SysViewHyTurb.Driver.IO;
    using SysViewHyTurb.Driver.Message;
    using SysViewHyTurb.Driver.Utility;
    using System.Windows.Forms;

    /// <summary>
    /// Modbus master device.
    /// </summary>
    public class ModbusMaster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusMaster"/> class
        /// </summary>
        /// <param name="transport">Takes a modbus transport</param>
        public ModbusMaster(ModbusTransport transport)
        {
            this.transport = transport;
        }

        /// <summary>
        /// Gets the transport.
        /// </summary>
        /// <value>The transport.</value>
        public ModbusTransport Transport
        {
            get
            {
                return this.transport;
            }
        }


        /// <summary>
        /// Read from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status</returns>
        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            return this.ReadDiscretes(Modbus.ReadCoils, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Read from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>Discrete inputs status</returns>
        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            return this.ReadDiscretes(Modbus.ReadInputs, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Read contiguous block of 16 bit holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status</returns>
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            return this.ReadRegisters(Modbus.ReadHoldingRegisters, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Read contiguous block of 16 bit input registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status</returns>
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            return this.ReadRegisters(Modbus.ReadInputRegisters, slaveAddress, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Validates number of points  of a read
        /// </summary>
        /// <param name="argumentName">argument name</param>
        /// <param name="numberOfPoints">number of points</param>
        /// <param name="maxNumberOfPoints">max number of  points</param>
        private static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                 throw new ArgumentException(
                     string.Format(
                                  CultureInfo.InvariantCulture,
                                  "Argument {0} must be between 1 and {1} inclusive.",
                                  argumentName,
                                  maxNumberOfPoints));               
            }
        }

        /// <summary>
        /// Reads input and holding registers
        /// </summary>
        /// <param name="functionCode">Function code</param>
        /// <param name="slaveAddress">Slave address</param>
        /// <param name="startAddress">Start address to read</param>
        /// <param name="numberOfPoints">Number of points to read</param>
        /// <returns>Registers read</returns>
        private ushort[] ReadRegisters(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            //try
            //{
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(functionCode, slaveAddress, startAddress, numberOfPoints);
            //MessageBox.Show(request.ToString());
            //MessageBox.Show(this.Transport.ToString());
            ReadHoldingInputRegistersResponse response = this.Transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);
            return response.Data.ToArray();
            //}
            //catch (Exception e)
            //{
            //    string er = e.Message;
            //    return null;
            //}
            
        }

        /// <summary>
        /// Reads coils or discrete inputs
        /// </summary>
        /// <param name="functionCode">Function Code</param>
        /// <param name="slaveAddress">Slave address to read </param>
        /// <param name="startAddress">Start address to read</param>
        /// <param name="numberOfPoints">Number of points to read</param>
        /// <returns>coils or discrete inputs to read</returns>
        private bool[] ReadDiscretes(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(functionCode, slaveAddress, startAddress, numberOfPoints);
            ReadCoilsInputsResponse response = this.Transport.UnicastMessage<ReadCoilsInputsResponse>(request);         
            return response.Data.Take(request.NumberOfPoints.Value).ToArray();
        }

        private ModbusTransport transport;
    }
}

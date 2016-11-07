//-----------------------------------------------------------------------
// <copyright file="RegisterCollection.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using SysViewHyTurb.Driver.Utility; 

    /// <summary>
    /// Register Collection
    /// </summary>
    public class RegisterCollection : Collection<ushort>, IDataCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCollection"/> class.
        /// </summary>
        public RegisterCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCollection"/> class.
        /// </summary>
        /// <param name="bytes">Takes a byte array</param>
        public RegisterCollection(byte[] bytes)
            : this((IList<ushort>)NetworkUtility.NetworkBytesToHostUInt16(bytes))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCollection"/> class.
        /// </summary>
        /// <param name="registers">Takes a unshort array</param>
        public RegisterCollection(params ushort[] registers)
            : this((IList<ushort>)registers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCollection"/> class.
        /// </summary>
        /// <param name="registers">Takes a unshort list</param>
        public RegisterCollection(IList<ushort> registers)
            : base(registers.IsReadOnly ? new List<ushort>(registers) : registers)
        {
        }

        /// <summary>
        /// Gets the network bytes.
        /// </summary>
        public byte[] NetworkBytes
        {
            get
            {
                List<byte> bytes = new List<byte>();

                foreach (ushort register in this)
                {
                    bytes.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)register)));
                }

                return bytes.ToArray();
            }
        }

        /// <summary>
        /// Gets the byte count.
        /// </summary>
        public byte ByteCount
        {
            get 
            { 
                return (byte)(Count * 2);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        //public override string ToString()
        //{
        //    return string.Concat("{", string.Join(", ", this.Select(v => v.ToString()).ToArray()), "}");
        //}
    }
}

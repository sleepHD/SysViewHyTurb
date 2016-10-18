//-----------------------------------------------------------------------
// <copyright file="DiscreteCollection.cs" company="SysTek">
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
    using System.Linq;

    /// <summary>
    /// Discrete Collection
    /// </summary>
    public class DiscreteCollection : Collection<bool>, IDataCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteCollection"/> class.
        /// </summary>
        public DiscreteCollection()
        {
        }

       /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteCollection"/> class.
       /// </summary>
       /// <param name="bits">Takes a bit array</param>
        public DiscreteCollection(params bool[] bits)
            : this((IList<bool>)bits)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteCollection"/> class.
        /// </summary>
        /// <param name="bytes">Takes a byte array</param>
        public DiscreteCollection(params byte[] bytes)
            : this((IList<bool>)(new BitArray(bytes)).Cast<bool>().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteCollection"/> class.
        /// </summary>
        /// <param name="bits">Takes a bit list</param>
        public DiscreteCollection(IList<bool> bits)
            : base(bits.IsReadOnly ? new List<bool>(bits) : bits)
        {
        }

        /// <summary>
        /// Gets the network bytes.
        /// </summary>
        public byte[] NetworkBytes
        {
            get
            {
                bool[] bits = new bool[Count];
                this.CopyTo(bits, 0);

                BitArray bitArray = new BitArray(bits);

                byte[] bytes = new byte[this.ByteCount];
                bitArray.CopyTo(bytes, 0);

                return bytes;
            }
        }

        /// <summary>
        /// Gets the byte count.
        /// </summary>
        public byte ByteCount
        {
            get
            {
                return (byte)((Count + 7) / 8);
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
        //    return string.Concat("{", string.Join(", ", this.Select(discrete => discrete ? "1" : "0").ToArray()), "}");
        //}
    }
}

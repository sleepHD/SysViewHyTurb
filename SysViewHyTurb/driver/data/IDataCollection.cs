//-----------------------------------------------------------------------
// <copyright file="IDataCollection.cs" company="SysTek">
//     Copyright (c) SysTek. All rights reserved.
// </copyright>
// <author>Hule</author>
//-----------------------------------------------------------------------

namespace SysViewHyTurb.Driver.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Data part in a message
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// Gets the network bytes.
        /// </summary>
        byte[] NetworkBytes { get; }

        /// <summary>
        /// Gets the byte count.
        /// </summary>
        byte ByteCount { get; }
    }
}

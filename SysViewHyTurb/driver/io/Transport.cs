//-----------------------------------------------------------------------
// <copyright file="Transport.cs" company="SysTek">
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
    /// Base transport class for both master and slave transport.
    /// Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public abstract class Transport
    {
        /// <summary>
        /// private retries for property Retries
        /// </summary>
        private int retries = 3;

        /// <summary>
        /// Millisecond  waited to retry after a failure
        /// </summary>
        private int waitToRetryMilliseconds = 3000;

        /// <summary>
        /// private streamResource
        /// </summary>
        private IStreamResource streamResource;
                     
        /// <summary>
        /// Initializes a new instance of the <see cref="Transport"/> class
        /// </summary>
        /// <param name="streamResource">takes object implementing IStreamResource</param>
        public Transport(IStreamResource streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
            this.streamResource = streamResource;
        }

        /// <summary>
        /// Gets or sets number of times to retry sending message after encountering a failure such as an IOException, 
        /// TimeoutException, or a corrupt message,applicable for both master and slave transport.
        /// </summary>
        public int Retries
        {
            get { return this.retries; }
            set { this.retries = value; }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds the  master transport will wait before retrying a message after receiving 
        /// a slave exception response.
        /// </summary>
        public int WaitToRetryMilliseconds
        {
            get
            {
                return this.waitToRetryMilliseconds;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("WaitToRetryMilliseconds must be greater than 0.");
                }

                this.waitToRetryMilliseconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a read operation does not finish.
        /// </summary>
        public int ReadTimeout
        {
            get
            {
                return this.StreamResource.ReadTimeout;
            }

            set
            {
                this.StreamResource.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a write operation does not finish.
        /// </summary>
        public int WriteTimeout
        {
            get
            {
                return this.StreamResource.WriteTimeout;
            }

            set
            {
                this.StreamResource.WriteTimeout = value;
            }
        }

        /// <summary>
        /// Gets the stream resource.
        /// </summary>
        public IStreamResource StreamResource
        {
            get
            {
                return this.streamResource;
            }
        }     

        /// <summary>
        /// Builds byte array from message object for underlying IStreamResource transport
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>byte array to transport</returns>
        public abstract byte[] BuildMessageFrame(Message message);
        
        /// <summary>
        /// Builds byte array and Writes to the underlying IStreamResource transport
        /// </summary>
        /// <param name="message">Message object</param>
        public void WriteMessage(Message message)
        {
            this.StreamResource.DiscardInBuffer();
            byte[] frame = this.BuildMessageFrame(message);
            //logger.Debug(this.StreamResource.Port + "-TX: " + BitConverter.ToString(frame));
            this.StreamResource.Write(frame, 0, frame.Length);
           
        }
    }
}

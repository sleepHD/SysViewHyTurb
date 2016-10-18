//-----------------------------------------------------------------------
// <copyright file="ModbusTransport.cs" company="SysTek">
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
    using System.Windows.Forms;

    /// <summary>
    /// base abstract master transport class 
    /// </summary>
    public abstract class ModbusTransport : Transport 
    {        
        /// <summary>
        /// Used to sync read and write
        /// </summary>
        private object syncLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusTransport"/> class
        /// </summary>
        /// <param name="streamResource">a construct takes a IStreamResource</param>
        public ModbusTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        /// <summary>
        /// Message needs not be responded,general process
        /// If you want protocol specific handling,you have to override this method
        /// </summary>
        /// <param name="message">Message to send</param>
        public virtual void UnicastMessage(Message message)
        {
            lock (this.syncLock)
            {
                this.WriteMessage(message);
            }
        }
               
        /// <summary>
        /// Message received without request,general process
        /// If you want protocol specific handling,you have to override this method
        /// </summary>
        /// <typeparam name="T">Specific message type wanted to receive</typeparam>
        /// <returns>A specific message</returns>
        public virtual T UnicastMessage<T>() where T : Message, new()
        {
            Message response = null; 
            lock (this.syncLock)
            {
                response = this.ReadResponse<T>();
            }               
            return (T)response;   
        }

        /// <summary>
        /// Write and read message,general process
        /// f you want protocol specific handling,you have to override this method
        /// </summary>
        /// <typeparam name="T">Received message type</typeparam>
        /// <param name="message">Message to write</param>
        /// <returns>Message received</returns>
        public virtual T UnicastMessage<T>(Message message) where T : Message, new()
        {
            Message response = null;
            int attempt = 1;
            bool success = false;
            //MessageBox.Show("to write");
            do
            {
                try
                {
                    lock (this.syncLock)
                    {
                        //MessageBox.Show(attempt.ToString());
                        this.WriteMessage(message);
                        response = this.ReadResponse<T>();
                        success = true;
                    }
                }
                catch (Exception e)
                {
                    if (++attempt > this.Retries)
                        throw e;
                }
            } while (!success);

            return (T)response;
        }

        /// <summary>
        /// Derived class choose to implement or not
        /// </summary>
        /// <param name="request">request sent</param>
        /// <param name="response">response received</param>
        public virtual void ValidateResponse(Message request, Message response)
        {
        } 
     
        /// <summary>
        /// Read a specific response,distinguish between ReadResponse and ReadRequest(declared in SlaveTransport)
        /// Write is implemented in base Transport class,no need to distinguish between WriteRequest and WriteResponse
        /// </summary>
        /// <typeparam name="T">Specific message type wanted to read</typeparam>
        /// <returns>Message received</returns>
        public abstract Message ReadResponse<T>() where T : Message, new();     
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb
{
    using System.IO;
    using System.Net.WebSockets;

    using AustinHarris.JsonRpc;

    using Unosquare.Labs.EmbedIO;
    using Unosquare.Labs.EmbedIO.Modules;

    /// <summary>
    /// Defines a scada server
    /// </summary>
    public class WebSocketsScadaServer : WebSocketsServer
    {

        public WebSocketsScadaServer()
            : base(true, 0)
        {
            // placeholder
        }

        /// <summary>
        /// Called when this WebSockets Server receives a full message (EndOfMessage) form a WebSockets client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rxBuffer">The rx buffer.</param>
        /// <param name="rxResult">The rx result.</param>
        protected override void OnMessageReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
        {
            var rpcResultHandler = new AsyncCallback(
               _ => this.Send(context, ((JsonRpcStateAsync)_).Result));

            var async = new JsonRpcStateAsync(rpcResultHandler, null) { JsonRpc = Encoding.UTF8.GetString(rxBuffer) };
            JsonRpcProcessor.Process(async);
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public override string ServerName
        {
            get { return "Chat Server"; }
        }

        /// <summary>
        /// Called when this WebSockets Server accepts a new WebSockets client.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnClientConnected(WebSocketContext context)
        {
            this.Send(context, "Welcome to the chat room!");
            foreach (var ws in this.WebSockets)
            {
                if (ws != context)
                    this.Send(ws, "Someone joined the chat room.");
            }
        }

        /// <summary>
        /// Called when this WebSockets Server receives a message frame regardless if the frame represents the EndOfMessage.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rxBuffer">The rx buffer.</param>
        /// <param name="rxResult">The rx result.</param>
        protected override void OnFrameReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
        {
            
        }

        /// <summary>
        /// Called when the server has removed a WebSockets connected client for any reason.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnClientDisconnected(WebSocketContext context)
        {
            this.Broadcast(string.Format("Someone left the chat room."));
        }

        private static object svc = new ScadaService();
    }
}

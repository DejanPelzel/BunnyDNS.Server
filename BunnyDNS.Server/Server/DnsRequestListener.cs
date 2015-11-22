using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Specialized;
using BunnyDNS.Server.RequestProcessor;

namespace BunnyDNS.Server
{
    public abstract class DnsRequestListener
    {
        /// <summary>
        /// The pool of SocketAsyncEventArgs objects that will be used for the connection
        /// </summary>
        protected Stack<SocketAsyncEventArgs> _socketAsyncEventArgsPool;

        /// <summary>
        /// The port that will be used for listening
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The DNS record provider that will be used to provide answer records for the received requests
        /// </summary>
        public DnsRecordProvider DnsRecordProvider { get; private set; }

        /// <summary>
        /// Initialize and create a new DnsRequestListener object
        /// </summary>
        public DnsRequestListener(DnsRecordProvider dnsRecordProvider, int port)
        {
            // Initialize the socket event pool
            this._socketAsyncEventArgsPool = new Stack<SocketAsyncEventArgs>();
            this.Port = 53;
            this.DnsRecordProvider = dnsRecordProvider;
        }

        /// <summary>
        /// Send the response back to the client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bytesWritten"></param>
        public abstract void SendResponse(DnsRequestListenerSession request, int bytesWritten);
    }
}

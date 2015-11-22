using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using BunnyDNS.Server.RequestProcessor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Specialized;

namespace BunnyDNS.Server
{
    public class UdpDnsRequestListener : DnsRequestListener
    {
        private Socket listenerSocket;
        public static int packetsReceived = 0;
        public static long bytesReceived = 0;

        /// <summary>
        /// Create a new DnsServer object
        /// </summary>
        public UdpDnsRequestListener(DnsRecordProvider dnsRecordProvider, int defaultPoolSize = 5000, int port = 53) : base(dnsRecordProvider, port)
        {
            this.listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.listenerSocket.ReceiveBufferSize = 20000000; //~20 megabytes

            // Initialize the socket event pool
            this._socketAsyncEventArgsPool = new Stack<SocketAsyncEventArgs>();

            // Initialize the SocketAsyncEventArgs pool
            for (int i = 0; i < defaultPoolSize; i++)
            {
                var socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.Completed += UdpSocketEventArg_Completed;
                socketEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Any, this.Port);
                socketEventArg.SetBuffer(new byte[4096], 0, 4096);

                // Connect with a request
                var request = new DnsRequestListenerSession(this, socketEventArg);
                socketEventArg.UserToken = request;

                // Add to the pool
                this._socketAsyncEventArgsPool.Push(socketEventArg);
            }
        }


        /// <summary>
        /// Start the DNS server and begin accepting new queries
        /// </summary>
        public void BeginAcceptQueries()
        {
            this.listenerSocket.Bind(new IPEndPoint(IPAddress.Any, this.Port));

            // Start the appropriate task according to the connection type
            foreach (var args in this._socketAsyncEventArgsPool)
            {
                this.listenerSocket.ReceiveFromAsync(args);
            }
        }

        /// <summary>
        /// Called when a ReceiveAsyncFrom operation completes
        /// </summary>
        private void ProcessReceiveFrom(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    packetsReceived++;
                    bytesReceived += e.BytesTransferred;

                    var request = e.UserToken as DnsRequestListenerSession;
                    // Handle this request
                    var responseBytes = DnsRequestProcessor.ProcessReceivedData(request);
                    if (responseBytes > 0)
                    {
                        request.SocketAsyncEventArgs.SetBuffer(0, responseBytes);
                        this.listenerSocket.SendToAsync(e);
                    }
                }
                else
                {
                    // Start listening again
                    e.SetBuffer(0, e.Buffer.Length);
                    this.listenerSocket.ReceiveFromAsync(e);
                }
            }
            catch {
                // In case of an exception, start listening again
                e.SetBuffer(0, e.Buffer.Length);
                this.listenerSocket.ReceiveFromAsync(e);
            }
        }

        /// <summary>
        /// Called when a SendToAsync operation completes
        /// </summary>
        private void ProcessSendTo(SocketAsyncEventArgs e)
        {
            e.SetBuffer(0, e.Buffer.Length);
            this.listenerSocket.ReceiveFromAsync(e);
        }

        public override void SendResponse(DnsRequestListenerSession request, int bytesWritten)
        {
            request.SocketAsyncEventArgs.SetBuffer(0, bytesWritten);
            this.listenerSocket.SendToAsync(request.SocketAsyncEventArgs);
        }

        
        /// <summary>
        /// Handle the UDP socket event completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UdpSocketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFrom(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSendTo(e);
                    break;
                default:
                    throw new Exception("Invalid operation completed: " + e.LastOperation.ToString());
            }
        }
    }
}


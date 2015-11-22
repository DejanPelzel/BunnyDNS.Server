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
    public class TcpDnsRequestListener : DnsRequestListener
    {
        /// <summary>
        /// The semaphore used for limiting the number of simultanious connections
        /// </summary>
        private Semaphore connectionsSemaphore;
        public static int packetsReceived = 0;
        public static long bytesReceived = 0;
        
        /// <summary>
        /// The socket used for listening for new connections
        /// </summary>
        protected Socket listenerSocket;

        /// <summary>
        /// Create a new DnsServer object
        /// </summary>
        public TcpDnsRequestListener(DnsRecordProvider dnsRecordProvider, int defaultPoolSize = 5000, int port = 53) : base(dnsRecordProvider, port)
        {
            this.listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.connectionsSemaphore = new Semaphore(defaultPoolSize, defaultPoolSize);
            this.listenerSocket.ReceiveBufferSize = 20000000; //~20 megabytes

            // Initialize the SocketAsyncEventArgs pool
            for (int i = 0; i < defaultPoolSize; i++)
            {
                var socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.Completed += TcpSocketEventArg_Completed;
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

            var acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            this.listenerSocket.Listen(10000);
            connectionsSemaphore.WaitOne();

            // Start listening
            bool willRaiseEvent = this.listenerSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client 
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing the accept operation on the 
        /// server's listening socket</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            //socket must be cleared since the context object is being reused
            acceptEventArg.AcceptSocket = null;
            connectionsSemaphore.WaitOne();

            // Accept a new client
            bool willRaiseEvent = this.listenerSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
            else
            {
                // TODO: ??
            }
        }

        /// <summary>
        /// Process accept a new connection
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                var readEventArgs = _socketAsyncEventArgsPool.Pop();
                ((DnsRequestListenerSession)readEventArgs.UserToken).Socket = e.AcceptSocket;

                // As soon as the client is connected, post a receive to the connection
                readEventArgs.SetBuffer(0, readEventArgs.Buffer.Length);
                bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch(Exception ex) {
                // Ignore exceptions
            }

            StartAccept(e);
        }

        /// <summary>
        /// This method is the callback method associated with Socket.AcceptAsync operations and is invoked
        /// when an accept operation is complete
        /// </summary>
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// Called when a ReceiveAsyncFrom operation completes
        /// </summary>
        private void ProcessReceive(SocketAsyncEventArgs e)
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
                        request.Socket.SendAsync(e);
                        return;
                    }
                }
            }
            catch (Exception ex) {
                // In case of an exception, close the socket
            }

            // Start listening again
            this.CloseClientSocket(e);
        }

        /// <summary>
        /// Close the client socket
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            var request = e.UserToken as DnsRequestListenerSession;
            
            // close the socket associated with the client
            try
            {
                request.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            request.Socket.Close();

  
            // Reset the buffer and release back the event args
            _socketAsyncEventArgsPool.Push(e);

            connectionsSemaphore.Release();
        }

        /// <summary>
        /// Handle the TCP socket event completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpSocketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new Exception("Invalid operation completed: " + e.LastOperation.ToString());
            }
        }


        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  The method issues another receive
        /// on the socket to read any additional data sent from the client
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                CloseClientSocket(e);
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        public override void SendResponse(DnsRequestListenerSession request, int bytesWritten)
        {
            request.SocketAsyncEventArgs.SetBuffer(0, bytesWritten);
            request.Socket.SendAsync(request.SocketAsyncEventArgs);
        }
    }
}


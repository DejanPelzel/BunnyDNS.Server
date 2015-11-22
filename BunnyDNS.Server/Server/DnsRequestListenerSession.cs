using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BunnyDNS.Server
{
    public class DnsRequestListenerSession
    {
        /// <summary>
        /// Create a new DnsRequestListenerSession object linked to the given listener
        /// </summary>
        /// <param name="requesListener"></param>
        /// <param name="eventArgs"></param>
        public DnsRequestListenerSession(DnsRequestListener requesListener, SocketAsyncEventArgs eventArgs)
        {
            this.DnsRequestListener = requesListener;
            this.SocketAsyncEventArgs = eventArgs;
        }

        /// <summary>
        /// Gets the buffer of the session
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this.SocketAsyncEventArgs.Buffer;
            }
        }

        /// <summary>
        /// Gets or sets the socket event args
        /// </summary>
        public SocketAsyncEventArgs SocketAsyncEventArgs { get; set; }

        /// <summary>
        /// /The parent DnsRequestListener object
        /// </summary>
        public DnsRequestListener DnsRequestListener { get; private set; }

        /// <summary>
        /// Used for TCP connections only, this holds the currently socket of the session
        /// </summary>
        public Socket Socket { get; set; }
    }
}

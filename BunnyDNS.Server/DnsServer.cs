using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server.RequestProcessor;

namespace BunnyDNS.Server
{
    /// <summary>
    /// Provides a full wrapper around the BunnyDNS services
    /// </summary>
    public class DnsServer
    {
        /// <summary>
        /// The TCP listener
        /// </summary>
        public TcpDnsRequestListener TcpRequestListener { get; private set; }

        /// <summary>
        /// The UDP listener
        /// </summary>
        public UdpDnsRequestListener UdpRequestListener { get; private set; }

        /// <summary>
        /// Create a new DnsServer object
        /// </summary>
        public DnsServer(DnsRecordProvider dnsRecordProvider, int udpArgPoolSize = 5000, int tcpArgPoolSize = 5000, int port = 53)
        {
            this.TcpRequestListener = new TcpDnsRequestListener(dnsRecordProvider, tcpArgPoolSize, port);
            this.UdpRequestListener = new UdpDnsRequestListener(dnsRecordProvider, udpArgPoolSize, port);
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            this.TcpRequestListener.BeginAcceptQueries();
            this.UdpRequestListener.BeginAcceptQueries();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException(); // YOLO, we run forever!
        }
    }
}

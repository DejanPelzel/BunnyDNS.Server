using System;
using System.Net;
using System.Text;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class NsDnsRecord : DnsRecord
    {
        /// <summary>
        /// The hostname that will be returned in the NS record
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// Create a new NS record object
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="hostname"></param>
        public NsDnsRecord(int ttl, string hostname) : base(DnsZoneType.NS, ttl)
        {
            this.Hostname = hostname;
        }

        /// <summary>
        /// Write the response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override int WriteResponseData(byte[] array, int offset)
        {
            var hostnameLength = DnsUtil.WriteHostnameLabel(array, offset, this.Hostname);
            return hostnameLength;
        }
    }
}


using System;
using System.Net;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class CnameDnsRecord : DnsRecord
    {
        /// <summary>
        /// The hostname that will be returned
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// Create a new CnameDnsRecord object
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="hostname"></param>
        public CnameDnsRecord(int ttl, string hostname) : base(DnsZoneType.CNAME, ttl)
        {
            this.Hostname = hostname;
        }


        /// <summary>
        /// Write the record response data
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


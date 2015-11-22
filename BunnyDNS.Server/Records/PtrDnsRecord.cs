using System;
using System.Net;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class PtrDnsRecord : DnsRecord
    {
        /// <summary>
        /// The hostname that will be returned
        /// </summary>
        public string Hostname { get; set; }

        public PtrDnsRecord(int ttl, string hostname) : base(DnsZoneType.PTR, ttl)
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


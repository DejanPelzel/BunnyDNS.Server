using System;
using System.Net;
using System.Text;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class TxtDnsRecord : DnsRecord
    {
        /// <summary>
        /// The value of the TXT record
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Create a new TXT record
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="value"></param>
        public TxtDnsRecord(int ttl, string value) : base(DnsZoneType.TXT, ttl)
        {
            if(value.Length > 255)
            {
                throw new ArgumentException("Maximum value length can be 255 characters", "value");
            }
            
            this.Value = value;
        }

        /// <summary>
        /// Write the response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override int WriteResponseData(byte[] array, int offset)
        {
            return DnsEncoder.WriteTextBlock(array, offset, this.Value);
        }
    }
}


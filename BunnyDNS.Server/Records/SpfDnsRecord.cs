using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class SpfDnsRecord : DnsRecord
    {
        /// <summary>
        /// The value of the SPF record
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Create a new SPF record
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="value"></param>
        public SpfDnsRecord(int ttl, string value) : base(DnsZoneType.SPF, ttl)
        {
            if (value.Length > 255)
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

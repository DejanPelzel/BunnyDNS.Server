using System;
using System.Net;
using BunnyDNS.Server.Util;
using System.Text;

namespace BunnyDNS.Server
{
    public class MxRecord : DnsRecord
    {
        /// <summary>
        /// The priority of the MX record
        /// </summary>
        public uint Priority { get; set; }

        /// <summary>
        /// The value of the MX record that will be returned
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Create a new MX record with the given data
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="priority"></param>
        /// <param name="value"></param>
        public MxRecord(int ttl, uint priority, string value) : base(DnsZoneType.MX, ttl)
        {
            this.Priority = priority;
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
            int bytesWritten = DnsEncoder.WriteUint16(array, this.Priority, offset);
            bytesWritten += DnsUtil.WriteHostnameLabel(array, offset + bytesWritten, this.Value);

            return bytesWritten;
        }
    }
}


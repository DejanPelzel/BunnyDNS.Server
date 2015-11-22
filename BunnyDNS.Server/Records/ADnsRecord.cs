using System;
using System.Net;

namespace BunnyDNS.Server
{
	public class ADnsRecord : DnsRecord
	{
        /// <summary>
        /// The IP that will be returned
        /// </summary>
        public IPAddress IP { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="ip"></param>
        public ADnsRecord(int ttl, IPAddress ip) : base(DnsZoneType.A, ttl)
        {
            this.IP = ip;
        }

        /// <summary>
        /// Write the record response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override int WriteResponseData(byte[] array, int offset)
        {
            byte[] buffer = this.IP.GetAddressBytes();
            Buffer.BlockCopy(buffer, 0, array, offset, buffer.Length);

            return buffer.Length;
        }
    }
}


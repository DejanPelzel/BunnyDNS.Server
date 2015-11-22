using System;
using System.Net;
using BunnyDNS.Server.Util;
using System.Text;

namespace BunnyDNS.Server
{
    public class SrvDnsRecord : DnsRecord
    {
        /// <summary>
        /// The priority SRV record 
        /// </summary>
        public uint Priority { get; set; }

        /// <summary>
        /// The weight of the SRV record
        /// </summary>
        public uint Weight { get; set; }

        /// <summary>
        /// The port of the SRV record
        /// </summary>
        public uint Port { get; set; }

        /// <summary>
        /// The hostname value of the SRV record
        /// </summary>
        public string Hostname { get; set; }
        
        /// <summary>
        /// Create a new SRV record
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="priority"></param>
        /// <param name="weight"></param>
        /// <param name="port"></param>
        /// <param name="value"></param>
        public SrvDnsRecord(int ttl, uint priority, uint weight, uint port, string hostname) : base(DnsZoneType.SRV, ttl)
        {
            this.Priority = priority;
            this.Weight = weight;
            this.Port = port;
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
            var bytesWritten = DnsEncoder.WriteUint16(array, this.Priority, offset);
            bytesWritten += DnsEncoder.WriteUint16(array, this.Weight, offset + bytesWritten);
            bytesWritten += DnsEncoder.WriteUint16(array, this.Port, offset + bytesWritten);

            bytesWritten += DnsUtil.WriteHostnameLabel(array, offset + bytesWritten, this.Hostname);
            return bytesWritten;
        }
    }
}
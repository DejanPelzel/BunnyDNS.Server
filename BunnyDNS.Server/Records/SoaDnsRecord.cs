using System;
using System.Net;
using BunnyDNS.Server.Util;
using System.Text;

namespace BunnyDNS.Server
{
    public class SoaDnsRecord : DnsRecord
    {
        /// <summary>master hostname value of the SOA record
        /// </summary>
        public string MasterHostname { get; set; }

        /// <summary>
        /// The responsible hostname value of the SOA record
        /// </summary>
        public string ResponsibleMailboxHostname { get; set; }

        /// <summary>
        /// The serial number of the SOA record
        /// </summary>
        public uint SerialNumber { get; set; }

        /// <summary>
        /// The serial number of the SOA record
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The retry interval of the SOA record
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// The expiration limit of the SOA record
        /// </summary>
        public int ExpirationLimit { get; set; }

        /// <summary>
        /// The minimum TTL of the SOA record
        /// </summary>
        public int MinimumTTL { get; set; }

        /// <summary>
        /// Create a new SOA record with the given data
        /// </summary>
        /// <param name="ttl"></param>
        /// <param name="priority"></param>
        /// <param name="value"></param>
        public SoaDnsRecord(int ttl, string masterHostname, string responsibleMailboxHostname, uint serialNumber, int refreshInterval, int retryInterval, int expirationLimit, int minimumTTL) : base(DnsZoneType.SOA, ttl)
        {
            this.MasterHostname = masterHostname;
            this.ResponsibleMailboxHostname = responsibleMailboxHostname;
            this.SerialNumber = serialNumber;
            this.RefreshInterval = refreshInterval;
            this.RetryInterval = retryInterval;
            this.ExpirationLimit = expirationLimit;
            this.MinimumTTL = minimumTTL;
        }

        /// <summary>
        /// Write the response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override int WriteResponseData(byte[] array, int offset)
        {
            int bytesWritten = DnsUtil.WriteHostnameLabel(array, offset, this.MasterHostname);
            bytesWritten += DnsUtil.WriteHostnameLabel(array, offset + bytesWritten, this.ResponsibleMailboxHostname);

            bytesWritten += DnsEncoder.WriteUint32(array, this.SerialNumber, offset + bytesWritten);
            bytesWritten += DnsEncoder.WriteInt32(array, this.RefreshInterval, offset + bytesWritten);
            bytesWritten += DnsEncoder.WriteInt32(array, this.RetryInterval, offset + bytesWritten);
            bytesWritten += DnsEncoder.WriteInt32(array, this.ExpirationLimit, offset + bytesWritten);
            bytesWritten += DnsEncoder.WriteInt32(array, this.MinimumTTL, offset + bytesWritten);

            return bytesWritten;
        }
    }
}
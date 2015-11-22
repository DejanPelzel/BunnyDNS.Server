using System;
using System.Net;
using System.Text;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class OptDnsRecord : DnsRecord
    {
		public uint UdpPayloadSize = 0;

		/// <summary>
		/// Create a new OPTDnsRecord object
		/// </summary>
        public OptDnsRecord() : base(DnsZoneType.OPT, 0)
        {
            string header = "; OPT PSEUDOSECTION:\n; EDNS: version 0;";
            var headerBytes = Encoding.UTF8.GetBytes(header);
        }

        /// <summary>
		/// Write the record data into the given byte array and return the number of bytes written
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="offset">Offset.</param>
		public override int WriteRecord(byte[] array, int offset)
        {
            offset--;
            int startingOffset = offset;
            // 2 bytes - TYPE
            offset += DnsEncoder.WriteUint16(array, (uint)this.Type, offset);
            // 2 bytes - CLASS We just use the default one here
            offset += DnsEncoder.WriteUint16(array, (uint)4096, offset);
            // 2 bytes - Extended TTL: EXTENDED-RCODE
            offset += DnsEncoder.WriteUint16(array, (uint)0, offset);
            // 2 bytes - Extended TTL: VERSION
            offset += DnsEncoder.WriteUint16(array, (uint)0, offset);
            // 2 bytes - RDLENGTH the length of the response data for this response
            // This is actually written later, after we write the data with a negative offset

            // X bytes - Response data
            var dataLength = this.WriteResponseData(array, offset + 2);

            // Write the RDLENGTH
            offset += DnsEncoder.WriteUint16(array, (uint)dataLength, offset);
            offset += dataLength;

            //offset += this.ResponseData.Length;
            return offset - startingOffset;
        }

        /// <summary>
        /// Write the response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override int WriteResponseData(byte[] array, int offset)
        {
            return 0;
        }
    }
}


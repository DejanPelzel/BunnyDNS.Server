using System;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
	public abstract class DnsRecord
	{
		/// <summary>
		/// The type of the DnsRecord
		/// </summary>
		/// <value>The type.</value>
		public DnsZoneType Type { get; private set; }
		public int TTL { get; set; }
        /// <summary>
        /// True if the record is an authority record
        /// </summary>
        public bool IsAuthorityRecord { get; set; }

        /// <summary>
        /// Create a new DNS record
        /// </summary>
        /// <param name="ttl"></param>
		public DnsRecord (DnsZoneType type, int ttl)
		{
            this.Type = type;
            this.TTL = ttl;
		}

		/// <summary>
		/// Write the record data into the given byte array and return the number of bytes written
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="offset">Offset.</param>
		public virtual int WriteRecord(byte[] array, int offset)
		{
			int startingOffset = offset;

			// 2 bytes - TYPE
			offset += DnsEncoder.WriteUint16 (array, (uint)this.Type, offset);
			// 2 bytes - CLASS We just use the default one here
			offset += DnsEncoder.WriteUint16 (array, (uint)1, offset);
			// 4 bytes - TTL
			offset += DnsEncoder.WriteInt32 (array, this.TTL, offset);
			// 2 bytes - RDLENGTH the length of the response data for this response
			// This is actually written later, after we write the data with a negative offset

			// X bytes - Response data
            var dataLength = this.WriteResponseData(array, offset + 2);

            // Write the RDLENGTH
            offset += DnsEncoder.WriteUint16(array, (uint)dataLength, offset);
            offset += dataLength;

            return offset - startingOffset;
		}

        /// <summary>
        /// Write the response data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected abstract int WriteResponseData(byte[] array, int offset);
	}
}


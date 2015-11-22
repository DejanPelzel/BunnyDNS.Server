using System;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
	public class DnsMessageHeader
	{
		/// <summary>
		/// The size in bytes of the DNS message header
		/// </summary>
		public const int SizeInBytes = 12;

		/// <summary>
		/// The request type of the query
		/// </summary>
		/// <value>The type of the request.</value>
		public DnsQueryRequestType RequestType { get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether the message is a response
		/// </summary>
		/// <value><c>true</c> if recursin desired; otherwise, <c>false</c>.</value>
		public bool IsResponse { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether recursion is desired
		/// </summary>
		/// <value><c>true</c> if recursin desired; otherwise, <c>false</c>.</value>
		public bool RecursionDesired { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether recursion is desired
		/// </summary>
		/// <value><c>true</c> if recursin desired; otherwise, <c>false</c>.</value>
		public bool RecursionSupported { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the message is truncated
		/// </summary>
		/// <value><c>true</c> if recursin desired; otherwise, <c>false</c>.</value>
		public bool IsTruncated { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the message is an authorative answer
		/// </summary>
		/// <value><c>true</c> if recursin desired; otherwise, <c>false</c>.</value>
		public bool IsAuthorativeMessage { get; set; }
		/// <summary>
		/// The number of NS type answers in the message
		/// </summary>
		/// <value>The name server answers count.</value>
		public uint NameServerAnswerCount { get; set; }
		/// <summary>
		/// Gets the number of total normal answers in the message
		/// </summary>
		/// <value>The normal answers count.</value>
		public uint NormalAnswerCount { get; set; }
		/// <summary>
		/// Get the number of total additional answers in the message
		/// </summary>
		/// <value>The additional answers count.</value>
		public uint AdditionalAnswerCount { get; set; }
        /// <summary>
        /// Get the number of total questions in the message
        /// </summary>
        /// <value>The additional answers count.</value>
        public uint QuestionCount { get; set; }
        /// <summary>
        /// Get the number of addiotional questions
        /// </summary>
        /// <value>The additional answers count.</value>
        public uint AdditionalQuestionCount { get; set; }
        /// <summary>
        /// The status code that will be returned in the request
        /// </summary>
        /// <value>The status code.</value>
        public DnsQueryStatusCode StatusCode { get; set; }

		/// <summary>
		/// The ID of the message
		/// </summary>
		/// <value>The message identifier.</value>
		public byte[] MessageId { get; private set; }

		/// <summary>
		/// Create an empty DnsMessageHeader object
		/// </summary>
		public DnsMessageHeader (byte[] messageId)
		{
			this.MessageId = messageId;
		}

		/// <summary>
		/// Write the header data into the given array
		/// </summary>
		/// <returns>The header.</returns>
		/// <param name="array">Array.</param>
		/// <param name="offset">Offset.</param>
		public int WriteHeader(byte[] array, int offset)
		{
			// HEADER PART 1
			// 2 bytes - Message ID
			array [offset++] = this.MessageId[0];
			array [offset++] = this.MessageId[1];

			// 1 byte - QR, OPCODE, AA, TC, RD, RA
			byte header1 = 0;
			// 1 bit
			header1 += (byte)((this.IsResponse ? 1 : 0) << 7); // Add QR - Query response bit, since this is a response, it's a 1
			// 4 bits - Request type
			DnsEncoder.Save4BitNumeric(ref header1, (byte)this.RequestType, 1);
			// 1 bit, this is an athorative answer
			header1 += (byte)((this.IsAuthorativeMessage ? 1 : 0) << 2);
			// 1 bit, this message is not truncated
			header1 += (byte)((this.IsTruncated ? 1 : 0) << 1);
			// 1 bit, recursion not desired
			header1 += (byte)((this.RecursionDesired ? 1 : 0) << 0);
            array[offset++] = header1;

            // Write into the second byte
            // 1 bit, recursion not supported
            array[offset] = (byte)((this.RecursionSupported ? 1 : 0) << 0);

            // HEADER PART 2
            // 3 bits - res1, res2, res3
            // Skipped ~

            // 4 bits RCODE - Response status code
            DnsEncoder.Save4BitNumeric(ref array[offset++], (byte)this.StatusCode, 0);

			// HEADER COUNTS
			// 2 bytes - question count
			DnsEncoder.WriteUint16 (array, (uint)this.QuestionCount, offset);
			// 2 bytes - answer count
			DnsEncoder.WriteUint16 (array, this.NormalAnswerCount, offset + 2);
			// 2 bytes - NS type answer count
			DnsEncoder.WriteUint16 (array, this.NameServerAnswerCount, offset + 4);
			// 2 bytes - AR type answer count
			DnsEncoder.WriteUint16 (array, this.AdditionalAnswerCount, offset + 6);

			// A header is always the same size
			return DnsMessageHeader.SizeInBytes;
		}

		/// <summary>
		/// Parse a DnsMessageHeader object from the given array
		/// </summary>
		/// <param name="array">Array.</param>
		public static DnsMessageHeader Parse(byte[] array, int offset)
		{
			// TODO: check length
			// HEADER: A total of 12 bytes
			// 2 bytes - Message ID
			byte[] messageId = new byte[2];
			messageId[0] = array[offset++];
			messageId[1] = array[offset++];

			var dnsMessageHeader = new DnsMessageHeader (messageId);

			// 1 bit - QR query response, should be set to 0
			var queryHeaderByte1 = array[offset++];
			var queryHeaderByte2 = array[offset++];
			var isQuestion = ((queryHeaderByte1 >> 7) & 1) == 0;
			if (!isQuestion) {
				// INVALID DNS query
				return null;
			}
			// 4 bits - Request type
			dnsMessageHeader.RequestType = (DnsQueryRequestType)DnsEncoder.Parse4BitNumeric(queryHeaderByte1, 1);
			// 1 bit - authorative answer
			dnsMessageHeader.IsAuthorativeMessage = 1 == ((queryHeaderByte1 >> 2) & 1);
			dnsMessageHeader.IsTruncated = 1 == ((queryHeaderByte1 >> 1) & 1);
			dnsMessageHeader.RecursionDesired = 1 == ((queryHeaderByte1 >> 0) & 1);
			dnsMessageHeader.RecursionSupported = 1 == ((queryHeaderByte2 >> 7) & 1);

            // 2 bytes - QDCOUNT
            dnsMessageHeader.QuestionCount = DnsEncoder.ParseUint16(array, offset);
            // 4 bytes, skip
            offset += 6;
            // 2 bytes - Additional questions
            dnsMessageHeader.AdditionalQuestionCount = DnsEncoder.ParseUint16(array, offset);


            // TODO: what to do if number of questions is 0?
            // TODO: what to do if number of questions does not match

            return dnsMessageHeader;
		}
	}
}


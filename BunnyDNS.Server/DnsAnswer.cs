using System;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
    public class DnsAnswer
    {
		/// <summary>
		/// The DNS record that will be returned in this answer
		/// </summary>
		/// <value>The record.</value>
        public DnsRecord Record { get; set; }

		/// <summary>
		/// The DnsQuestion linked to this answer
		/// </summary>
		/// <value>The question.</value>
        public DnsQuestion Question { get; private set; }

		/// <summary>
		/// The hostname linked to this answer. If the answer is linked
		/// to a question or to nothing this will return null.
		/// </summary>
		/// <value>The hostname.</value>
        public string Hostname { get; private set; }

		/// <summary>
		/// Create an empty DnsAnswer
		/// </summary>
		public DnsAnswer()
		{
			this.Hostname = null;
		}

		/// <summary>
		/// Create a DnsAnswer linked to the given hostname
		/// </summary>
		/// <param name="hostname">Hostname.</param>
        public DnsAnswer(string hostname)
        {
            this.Hostname = hostname;
            this.Question = null;
        }

		/// <summary>
		/// Create a DnsAnswer linked to the given question
		/// </summary>
		/// <param name="dnsQuestion">DnsQuestion.</param>
        public DnsAnswer (DnsQuestion dnsQuestion)
		{
            this.Question = dnsQuestion;
			this.Hostname = null;
        }

		/// <summary>
		/// Write the answer data into the given byte array and return the number of bytes written
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="offset">Offset.</param>
		public int WriteAnswer(byte[] array, int offset)
		{
			int startingOffset = offset;

			// 2 bytes - NAME POINTER
			// 16 bit pointer, with two first bits 11 indiciating we are sending a pointer
			// If the answer is linked to an answer with a pointer, return a pointer, otherwise return a label
			if(this.Question != null && this.Question.DataPointer > 0)
			{
				var pointerBytes = BitConverter.GetBytes(this.Question.DataPointer);
				ByteArray.SwapFields(pointerBytes, 0, 1);

				// First two bits indicate a pointer
				pointerBytes[0] += (1 << 7);
				pointerBytes[0] += (1 << 6);

				array[offset++] = pointerBytes[0];
				array[offset++] = pointerBytes[1];
			}
			// If not linked to an answer, check if we can output the hostname label
			else if(this.Hostname != null && this.Hostname.Length > 0)
			{
				offset += DnsUtil.WriteHostnameLabel(array, offset, this.Hostname);
			}
			// Finally, if we can't find another hostname, we output an empty pointer
			else
			{
				array[offset++] = 0;
				array[offset++] = 0;
			}

			// Write the record and finish
			offset += this.Record.WriteRecord (array, offset);
			return offset - startingOffset;
		}
	}
}


using System;
using System.Text;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server
{
	public class DnsQuestion
	{
		/// <summary>
		/// The zone type the question is asking for
		/// </summary>
		/// <value>The type.</value>
		public DnsZoneType ZoneType { get; private set; }

		/// <summary>
		/// The class the question is asking for
		/// </summary>
		/// <value>The class.</value>
		public DnsRecordClass Class { get; private set; }

		/// <summary>
		/// The hostname the question is asking for
		/// </summary>
		/// <value>The hostname.</value>
		public string Hostname { get; set; }

		/// <summary>
		/// The location offset pointer of the question of the original request message
		/// </summary>
		/// <value>The data pointer.</value>
		public int DataPointer { get; set; }

		/// <summary>
		/// Create an empty DnsQuestion object
		/// </summary>
		private DnsQuestion()
		{
		}

		public static int Parse(out DnsQuestion question, byte[] array, int offset)
		{
			question = new DnsQuestion ();
			question.DataPointer = offset;
			int startingOffset = offset;

			// Parse the domain name
			byte partLength = 0;
			var hostnameBuilder = new StringBuilder ();
			while ((partLength = array[offset++]) > 0) 
			{
				var domainPart = Encoding.UTF8.GetString (array, offset, partLength);
				// Add separating dots
				if (hostnameBuilder.Length > 0) {
					hostnameBuilder.Append ('.');
				}
				hostnameBuilder.Append (domainPart);
				offset += partLength;
				// TODO: handle bytes larger than 255 chracters (LIMIT!)
			}

			// Parse the zone request type
			// 2 bytes
			var zoneType = (DnsZoneType)DnsEncoder.ParseUint16(array, offset);
			offset += 2;

			// Skip 2 bytes of QCLASS, we don't need them
			var questionClass = (DnsRecordClass)DnsEncoder.ParseUint16(array, offset);
			offset += 2;

			// Wire up the data
			question.Hostname = hostnameBuilder.ToString ();
			question.ZoneType = zoneType;
			question.Class = questionClass;

			return offset - startingOffset;
		}
	}
}


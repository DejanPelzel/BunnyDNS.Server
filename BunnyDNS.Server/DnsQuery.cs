using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using BunnyDNS.Server.Geo;
using System.Net.Sockets;
using System.Net;
using BunnyDNS.Server.Edns;

namespace BunnyDNS.Server
{
    public class DnsQuery
    {
		/// <summary>
		/// The ID of the DNS query message
		/// </summary>
		/// <value>The message identifier.</value>
		public DnsMessageHeader Header { get; set; }

        private GeoLocation _GeoLocation;
        /// <summary>
        /// The geo location where the query orinates from.
        /// If send, this will use the IP data provided via the edns subnet record. 
        /// </summary>
        public GeoLocation GeoLocation
        {
            get
            {
                if(this._GeoLocation == null)
                {
                    this._GeoLocation = GeoLocationMapper.MapIP(this.ClientIPAddress);
                }

                return this._GeoLocation;
            }
        }

        /// <summary>
        /// The IP address of the client that requested the query
        /// </summary>
        public IPAddress ClientIPAddress { get; private set; }

        /// <summary>
        /// Gets the EdnsQuestion that was sent with the query using the OPT record.
        /// If no query was sent this returns null.
        /// </summary>
        public EdnsQuestion EdnsQuestion { get; private set; }

		/// <summary>
		/// Gets the parsing status of the query
		/// </summary>
		/// <value>The status.</value>
		public DnsQueryStatus Status { get; private set; }

        /// <summary>
        /// The original question data that was passed to the request
        /// </summary>
		public byte[] OriginalQuestionData { get; private set; }

		private List<DnsQuestion> _questions = new List<DnsQuestion>();
		/// <summary>
		/// The list of questions requested in this query
		/// </summary>
		/// <value>The questions.</value>
		public List<DnsQuestion> Questions {
			get {
				return this._questions;
			}
		}
        
        /// <summary>
        /// Parse the DnsQuery from the given UdpReceiveResult
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DnsQuery Parse(byte[] buffer, IPAddress remoteEndpoint)
        {
            // Create the query
            var query = new DnsQuery();
			query.Status = DnsQueryStatus.OK;
            query.ClientIPAddress = remoteEndpoint;

            // Parse the dns request header
            query.Header = DnsMessageHeader.Parse (buffer, 0);

            // DNS QUESTIONS:
            int offset = 12;
			for (int i = 0; i < query.Header.QuestionCount; i++) 
			{
				try
				{
					// Save the question
					DnsQuestion question;
					offset += DnsQuestion.Parse(out question, buffer, offset);
					if(question != null)
					{
						query.Questions.Add (question);
					}
				}
				catch (Exception e) {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
					query.Status = DnsQueryStatus.ParsingError;
					break;
				}
			}

            // Save the original question data to include it in the answer
            query.OriginalQuestionData = new byte[offset];
            Buffer.BlockCopy(buffer, 0, query.OriginalQuestionData, 0, offset);

            // Parse the additional EDNS question
            // For now, we only support a single additional question
            if (query.Header.AdditionalQuestionCount > 0)
            {
                //Console.WriteLine("Reading OPT");
                try
                {
                    int bytesProcessed = 0;
                    query.EdnsQuestion = EdnsQuestion.Parse(buffer, offset, ref bytesProcessed);
                    offset += bytesProcessed;

                    // If the question was validely parsed, try using the subnet to assign the IP based on the user's location instead of the
                    // dns proxy server
                    if (query.EdnsQuestion != null)
                    {
                        var clientSubnetOption = (EdnsClientSubnetOption)query.EdnsQuestion.GetOption(EdnsOptionCode.ClientSubnet);
                        if(clientSubnetOption != null)
                        {
                            query.ClientIPAddress = clientSubnetOption.IPAddress;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    // If this fails, we still try to proceed normally
                    // TODO: log
                }
            }

            return query;
        }
    }
}

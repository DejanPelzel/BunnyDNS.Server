using System;
using System.Collections.Generic;

namespace BunnyDNS.Server
{
	public class DnsResponse
	{
		/// <summary>
		/// The number of NS type answers in this response
		/// </summary>
		/// <value>The name server answers count.</value>
		public uint NameServerAnswerCount { get; private set; }
		/// <summary>
		/// Gets the number of total normal answers in this response
		/// </summary>
		/// <value>The normal answers count.</value>
		public uint NormalAnswerCount { get; private set; }
		/// <summary>
		/// Get the number of total additional answers in this response
		/// </summary>
		/// <value>The additional answers count.</value>
		public uint AdditionalAnswerCount { get; private set; }
		/// <summary>
		/// The status code that will be returned in the request
		/// </summary>
		/// <value>The status code.</value>
		public DnsQueryStatusCode StatusCode { get; set; }
		/// <summary>
		/// The original DNS query that this is a response to
		/// </summary>
		/// <value>The query.</value>
		public DnsQuery Query { get; private set; }

		/// <summary>
		/// The list of answers that will be returned
		/// </summary>
		/// <value>The answers.</value>
		public List<DnsAnswer> Answers { get; private set; }

		/// <summary>
		/// Create a new DnsResponse
		/// </summary>
		public DnsResponse (DnsQuery query, List<DnsAnswer> answers)
		{
			this.Query = query;
			this.Answers = answers;
			this.StatusCode = DnsQueryStatusCode.OK;

            // Add the OPT record
            this.Answers.Add(new DnsAnswer()
            {
                Record = new OptDnsRecord()
            });

            this.CountAnswers();
        }

        /// <summary>
        /// Get the response data
        /// </summary>
        /// <returns>The response.</returns>
        public int GetData(byte[] responseBuffer)
		{
			// Create and write the response header
			var dnsMessageHeader = new DnsMessageHeader (this.Query.Header.MessageId) {
				StatusCode = DnsQueryStatusCode.OK,
				AdditionalAnswerCount =  this.AdditionalAnswerCount,
				NameServerAnswerCount = this.NameServerAnswerCount,
				NormalAnswerCount = this.NormalAnswerCount,
				QuestionCount =  (uint)this.Query.Questions.Count,
				IsAuthorativeMessage = true,
				IsResponse = true,
				IsTruncated = false,
				RecursionDesired = false,
				RecursionSupported = false
			};
			dnsMessageHeader.WriteHeader (responseBuffer, 0);

			// After the header, copy the original request
			Buffer.BlockCopy (
				src: this.Query.OriginalQuestionData, 
				srcOffset: DnsMessageHeader.SizeInBytes, 
				dst: responseBuffer, 
				dstOffset: DnsMessageHeader.SizeInBytes, 
				count: this.Query.OriginalQuestionData.Length - DnsMessageHeader.SizeInBytes);

			// Now write the response
			var byteIndex = this.Query.OriginalQuestionData.Length;
            foreach (var answer in this.Answers)
            {
                byteIndex += answer.WriteAnswer(responseBuffer, byteIndex);
            }
            
            // Return the number of bytes written
            return byteIndex;
        }

		/// <summary>
		/// Count the total number of different answer types in the response
		/// </summary>
		private void CountAnswers()
		{
			uint answerCount = 0;
			uint nsAnswerCount = 0;
			uint arCount = 0;

			foreach (var answer in this.Answers)
			{
				if (answer.Record.Type == DnsZoneType.OPT)
				{
					arCount++;
				}
                // We respond the NS records as non-authoritive
				else if (answer.Record.Type == DnsZoneType.NS && answer.Record.IsAuthorityRecord)
				{
					nsAnswerCount++;
                }
				else {
					answerCount++;
				}
			}

			this.NormalAnswerCount = answerCount;
			this.NameServerAnswerCount = nsAnswerCount;
			this.AdditionalAnswerCount = arCount;
		}
	}
}


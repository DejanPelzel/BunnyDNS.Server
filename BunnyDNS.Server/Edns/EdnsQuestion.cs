using System;
using System.Text;
using BunnyDNS.Server.Util;
using System.Collections.Generic;

namespace BunnyDNS.Server.Edns
{
	public class EdnsQuestion
	{
        private List<EdnsOption> _Options = new List<EdnsOption>();
        /// <summary>
        /// 
        /// </summary>
        public List<EdnsOption> Options
        {
            get
            {
                return this._Options;
            }
        }

        /// <summary>
        /// The UDP size the request supports (default = 512)
        /// </summary>
        public int UdpSize { get; private set; } = 512;

        /// <summary>
        /// Create an empty DnsEdnsRequestMessage object
        /// </summary>
        private EdnsQuestion()
		{
		}

        /// <summary>
        /// If found, returns the option of the given type in this question.
        /// If it is not found, null is returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EdnsOption GetOption(EdnsOptionCode code)
        {
            foreach(var option in this.Options)
            {
                if(option.Code == code)
                {
                    return option;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse the DnsEdnsRequestMessage from the given byte array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
		public static EdnsQuestion Parse(byte[] array, int offset, ref int bytesProcessed)
		{
            // Check the length
            if(offset + 10 >= array.Length)
            {
                return null;
            }

            // domain
            var domain = array[offset++];

            // 2 bytes - Question type
            var type = (DnsZoneType)DnsEncoder.ParseUint16(array, offset);
            offset += 2;

            // 2 bytes - requestor UDP size
            // TODO: save this, should be useful
            var requestorUdpSize = DnsEncoder.ParseUint16(array, offset);
            if (requestorUdpSize < 512) {
                requestorUdpSize = 512; // Don't allow smaller than 512 bytes
            }
            else if (requestorUdpSize > 4096) {
                requestorUdpSize = 4096; // Don't allow smaller than 4096 bytes
            }
            offset += 2;

            // extended RCODE and flags ~ skip
            offset += 4;

            // Domain must be 0, type must be OPT
            if (domain != 0 || type != DnsZoneType.OPT)
            {
                return null;
            }

            // Read the RDATA length
            var rdataLength = DnsEncoder.ParseUint16(array, offset);
            offset += 2;

            // Validate length
            if (offset + rdataLength > array.Length)
            {
                return null;
            }

            // Create a new question
            var ednsQuestion = new EdnsQuestion();

            // Read the options
            int dataBytesProcessed = 0;
            while (dataBytesProcessed < rdataLength)
            {
                // Load the option based on type
                var option = EdnsOption.ParseOption(array, offset + dataBytesProcessed, ref dataBytesProcessed);
                if (option != null)
                {
                    ednsQuestion.Options.Add(option);
                }
            }

            // Save the bytes processed
            bytesProcessed = 10 + dataBytesProcessed;

            return ednsQuestion;
        }
	}
}


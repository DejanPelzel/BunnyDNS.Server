using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server.Util;
using System.Net;

namespace BunnyDNS.Server.Edns
{
    public class EdnsClientSubnetOption : EdnsOption
    {
        /// <summary>
        /// The source netmask of the IP
        /// </summary>
        public byte SourceNetmask { get; set; }

        /// <summary>
        /// The scope netmask of the IP
        /// </summary>
        public byte ScopeNetmask { get; set; }

        /// <summary>
        /// The IP address of the requesting client
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Create a new EdnsClientSubnetOption object
        /// </summary>
        public EdnsClientSubnetOption() : base(EdnsOptionCode.ClientSubnet)
        {

        }

        /// <summary>
        /// Parse a EdnsClientSubnetOption object from the given data
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="bytesProcessed"></param>
        /// <returns></returns>
        public static EdnsClientSubnetOption Parse(byte[] array, int offset, ref int bytesProcessed)
        {
            // 2 bytes - Check if an IPv4 or IPv6 ip will be given
            bool isIPv4 = DnsEncoder.ParseUint16(array, offset) == 1;
            offset += 2;
            // 1 byte - source netmask
            byte sourceNetmask = array[offset++];
            // 1 byte - scope netmask
            byte scopeNetmask = array[offset++];

            bytesProcessed += 4;

            // Create an array to hold the IP address data
            byte[] addressBytes = new byte[isIPv4 ? 4 : 16];

            // Validate array size
            int addressLength = (int)Math.Ceiling(sourceNetmask / 8d);
            if(addressLength > addressBytes.Length)
            {
                return null;
            }

            Buffer.BlockCopy(array, offset, addressBytes, 0, addressLength);
            bytesProcessed += addressLength;
            offset += addressLength;

            // Copy the data into a new EdnsClientSubnetOption object
            return new EdnsClientSubnetOption()
            {
                IPAddress = new IPAddress(addressBytes),
                ScopeNetmask = scopeNetmask,
                SourceNetmask = sourceNetmask
            };
        }
    }
}

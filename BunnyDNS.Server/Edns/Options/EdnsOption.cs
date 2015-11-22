using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server.Util;

namespace BunnyDNS.Server.Edns
{
    public class EdnsOption
    {
        /// <summary>
        /// The type code of this option
        /// </summary>
        public EdnsOptionCode Code { get; private set; }

        /// <summary>
        /// Create a new EdnsOption based on the given code
        /// </summary>
        public EdnsOption(EdnsOptionCode code)
        {
            this.Code = code;
        }

        /// <summary>
        /// Parse an EdnsOption based on the given type and array offset
        /// </summary>
        /// <param name="optionType"></param>
        /// <param name="optionLength"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static EdnsOption ParseOption(byte[] array, int offset, ref int bytesProcessed)
        {
            // 2 bytes ~ option code
            uint optionCode = DnsEncoder.ParseUint16(array, offset);
            offset += 2;

            // 2 bytes ~ option length
            uint optionLength = DnsEncoder.ParseUint16(array, offset);
            offset += 2;

            bytesProcessed += 4;

            EdnsOption option = null;
            // Load based on the option code
            switch ((EdnsOptionCode)optionCode)
            {
                case EdnsOptionCode.ClientSubnet:
                    option = EdnsClientSubnetOption.Parse(array, offset, ref bytesProcessed);
                    break;
            }

            return option;

        }
    }
}

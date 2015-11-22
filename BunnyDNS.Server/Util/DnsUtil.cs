using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyDNS.Server.Util
{
    internal class DnsUtil
    {
        /// <summary>
        /// Generate the byte[] array hostname label in a format to be used with DNS requests
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns>Bytes written</returns>
        public static int WriteHostnameLabel(byte[] targetByteArray, int offset, string hostname)
        {
            /*if(hostname.Length > 255)
            {
                // TODO: do something
            }*/
            byte[] buffer = new byte[256];

            int index = 0;
            var splitParts = hostname.Split('.');
            foreach (var part in splitParts)
            {
                var partBytes = Encoding.UTF8.GetBytes(part);
                buffer[index++] = (byte)partBytes.Length;

                // Write the byte parts
                Buffer.BlockCopy(partBytes, 0, buffer, index, partBytes.Length);
                index += partBytes.Length;
            }

            // Trim for the result
            Buffer.BlockCopy(buffer, 0, targetByteArray, offset, index);

            return index + 1; // We increase the index to 1, to include one empty byte indicating the end of the string
        }
    }
}

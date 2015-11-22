using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyDNS.Server.Util
{
    public class DnsEncoder
    {
		/// <summary>
		/// Parse a 4 bit numeric number and return it as a byte value
		/// </summary>
		/// <returns>The uint.</returns>
		/// <param name="array">Array.</param>
		/// <param name="index">Index.</param>
		public static byte Parse4BitNumeric(byte dataByte, int position)
		{
			return (byte)((dataByte >> position) & 15);
		}

		/// <summary>
		/// Parse the Uint value at the given index in the array
		/// </summary>
		/// <returns>The uint.</returns>
		/// <param name="array">Array.</param>
		/// <param name="index">Index.</param>
		public static uint ParseUint16(byte[] array, int index)
		{
			// Swap the fields to little endian
			ByteArray.SwapFields (array, index, index + 1);

			var value = BitConverter.ToUInt16 (array, index);

			// Swap back to maintain the data
			ByteArray.SwapFields(array, index, index + 1);

			return value;
		}

		/// <summary>
		/// Saves the given value into the array at the given index
		/// </summary>
		/// <returns>The uint16.</returns>
		/// <param name="array">Array.</param>
		/// <param name="index">Index.</param>
		public static int WriteUint16(byte[] array, uint value, int index)
		{
			var valueBytes = BitConverter.GetBytes (value);
			array [index] = valueBytes [1];
			array [index + 1] = valueBytes [0];

			return 2;
		}

        /// <summary>
        /// Saves the given value into the array at the given index
        /// </summary>
        /// <returns>The uint32.</returns>
        /// <param name="array">Array.</param>
        /// <param name="index">Index.</param>
        public static int WriteUint32(byte[] array, uint value, int index)
        {
            var valueBytes = BitConverter.GetBytes(value);
            array[index++] = valueBytes[3];
            array[index++] = valueBytes[2];
            array[index++] = valueBytes[1];
            array[index++] = valueBytes[0];

            return 4;
        }

        /// <summary>
        /// Saves the given value into the array at the given index
        /// </summary>
        /// <returns>The uint16.</returns>
        /// <param name="array">Array.</param>
        /// <param name="index">Index.</param>
        public static int WriteInt32(byte[] array, int value, int index)
		{
			var valueBytes = BitConverter.GetBytes (value);
			array[index++] = valueBytes[3];
			array[index++] = valueBytes[2];
			array[index++] = valueBytes[1];
			array[index++] = valueBytes[0];

			return 4;
		}

		/// <summary>
		/// Saves the given value into the array at the given index
		/// </summary>
		/// <returns>The uint16.</returns>
		/// <param name="array">Array.</param>
		/// <param name="index">Index.</param>
		public static void Save4BitNumeric(ref byte dataByte, byte value, int offset)
		{
			dataByte += (byte)(((value >> 0) & 1) << (4 - offset));
			dataByte += (byte)(((value >> 1) & 1) << (5 - offset));
			dataByte += (byte)(((value >> 2) & 1) << (6 - offset));
			dataByte += (byte)(((value >> 3) & 1) << (7 - offset));
		}

        /// <summary>
        /// Write an encoded text block with a length and value to the array at the given offset
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int WriteTextBlock(byte[] array, int offset, string text)
        {
            var valueBytes = Encoding.ASCII.GetBytes(text);
            array[offset] = (byte)valueBytes.Length;
            Buffer.BlockCopy(valueBytes, 0, array, offset + 1, valueBytes.Length);

            return valueBytes.Length + 1;
        }

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

            return index;
        }

        /// <summary>
        /// Parse the bit at the given position in the byte data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitNumber"></param>
        /// <returns></returns>
        public static int GetData(byte data, int position, int numberOfBits)
        {
            switch(numberOfBits)
            {
                case 1:
                    return (data >> position) & 1;
                case 2:
                    return (data >> position) & 3;
                case 3:
                    return (data >> position) & 7;
                case 4:
                    return (data >> position) & 15;
                case 5:
                    return (data >> position) & 31;
                case 6:
                    return (data >> position) & 63;
                case 7:
                    return (data >> position) & 127;
                case 8:
                    return (data >> position) & 255;
            }

            return 0;
        }  

    }
}

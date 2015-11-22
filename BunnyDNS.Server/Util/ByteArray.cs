using System;

namespace BunnyDNS.Server.Util
{
    public class ByteArray
    {
        public static void SwapFields(byte[] array, int index1, int index2)
        {
            var byte1 = array[index1];
            var byte2 = array[index2];
            array[index1] = byte2;
            array[index2] = byte1;
        }
    }
}


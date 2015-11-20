using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class Utility
    {
        public static short GetHighBits(int x)
        {
            uint u = (uint)(x);
            return (short) (u >> 16);
        }

        public static short GetLowBits(int x)
        {
            return (short)(x & 0xFFFF);
        }

        public static int toIntUnsigned(short x)
        {
            return x & 0xFFFF;
        }

        public static int unsignedBinarySearch(short[] array, int begin, int end, short k) {
       
            //optimizes for the case where the value is inserted at the end
            //if ((end > 0) && (toIntUnsigned(array[end - 1]) < ikey))
            //{

            //}
            return 0;
        }

    }
}

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

        public static uint toIntUnsigned(short x)
        {
            return (uint) x;
        }

        public static int unsignedBinarySearch(short[] array, int begin, int end, short k) {

            uint ikey = toIntUnsigned(k);
            //optimizes for the case where the value is inserted at the end
            if ((end > 0) && (toIntUnsigned(array[end - 1]) < ikey))
            {
                return -end - 1;
            }
            int low = begin;
            int high = end - 1;
            while (low <= high) {

                //convert to uint to shift unsigned by one, then convert back
                int middleIndex = (int)((uint)(low + high) >> 1);
                uint middleValue = toIntUnsigned(array[middleIndex]);

                if (middleIndex < ikey) {
                    low = middleIndex + 1;
                } else if (middleValue > ikey) {
                    high = middleIndex - 1;
                } else {
                    return middleIndex;
                }
                return -(low + 1);
            }
            return 0;
        }

    }
}

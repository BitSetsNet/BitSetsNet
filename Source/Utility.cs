using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class Utility
    {
        public static ushort GetHighBits(int x)
        {
            uint u = (uint)(x);
            return (ushort) (u >> 16);
        }

        public static ushort GetLowBits(int x)
        {
            return (ushort)(x & 0xFFFF);
        }

        public static uint toIntUnsigned(ushort x)
        {
            return (uint) x;
        }

        public static int unsignedBinarySearch(ushort[] array, int begin, int end, ushort key) {

            //optimizes for the case where the value is inserted at the end
            if ((end > 0) && (array[end - 1] < key))
            {
                return -end - 1;
            }

            int low = begin;
            int high = end - 1;
            while (low <= high) {

                // divide by 2 to find the middle index
                int middleIndex = (low + high) >> 1;
                ushort middleValue = array[middleIndex];

                if (middleValue < key) {
                    low = middleIndex + 1;
                } else if (middleValue > key) {
                    high = middleIndex - 1;
                } else {
                    return middleIndex;
                }
            }
            return -(low + 1);
        }
		
        /// <summary>
        /// Naive implementation to count the number of true bits in a word.
        /// </summary>
        /// <param name="w">
        /// word
        /// </param>
        /// <returns>
        /// The number of true bits in the word
        /// </returns>
        public static int longBitCount(long w)
        {
            //TODO - Implement a faster counting method
            int rtnVal = 0;

            for (int i = 0; i < 64; i++)
            {
                rtnVal += (int)((uint)(w >> i) & 1);
            }

            return rtnVal;
        }

        /// <summary>
        /// compute the bitwise AND between two long arrays and write the set
        /// bits in the container
        /// </summary>
        /// <param name="container">where we write</param>
        /// <param name="bitmap1">first bitmap</param>
        /// <param name="bitmap2">second bitmap</param>
        public static void fillArrayAND(
            ref ushort[] container, 
            long[] bitmap1,
            long[] bitmap2
        ) {
            int pos = 0;

            if (bitmap1.Length != bitmap2.Length)
                throw new ArgumentOutOfRangeException("not supported");

            for (int k = 0; k < bitmap1.Length; ++k) {
                long bitset = bitmap1[k] & bitmap2[k];

                while (bitset != 0) {
                    long t = bitset & -bitset;
                    container[pos++] = (ushort) (k * 64 + longBitCount(t - 1));
                    bitset ^= t;
                }
            }
        }
    }
}

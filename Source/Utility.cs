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
            int rtnVal = 0;
            ulong word = (ulong)w;

            for (; word > 0; rtnVal++)
            {
                word &= word - 1;
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
        
        
        /**
 * Unite two sorted lists and write the result to the provided
 * output array
 *
 * @param set1    first array
 * @param length1 length of first array
 * @param set2    second array
 * @param length2 length of second array
 * @param buffer  output array
 * @return cardinality of the union
 */
        public static int unsignedUnion2by2(ushort[] set1,
                                            int length1, 
                                            ushort[] set2,
                                            int length2,
                                            ushort[] buffer)
        {
            int pos = 0;
            int k1 = 0, k2 = 0;
            if (0 == length2)
            {
                Array.Copy(set1, 0, buffer, 0, length1);
                return length1;
            }
            if (0 == length1)
            {
                Array.Copy(set2, 0, buffer, 0, length2);
                return length2;
            }
            ushort s1 = set1[k1];
            ushort s2 = set2[k2];
            while (true)
            {
                int v1 = s1;
                int v2 = s2;
                if (v1 < v2)
                {
                    buffer[pos++] = s1;
                    ++k1;
                    if (k1 >= length1)
                    {
                        Array.Copy(set2, k2, buffer, pos, length2 - k2);
                        return pos + length2 - k2;
                    }
                    s1 = set1[k1];
                }
                else if (v1 == v2)
                {
                    buffer[pos++] = s1;
                    ++k1;
                    ++k2;
                    if (k1 >= length1)
                    {
                        Array.Copy(set2, k2, buffer, pos, length2 - k2);
                        return pos + length2 - k2;
                    }
                    if (k2 >= length2)
                    {
                        Array.Copy(set1, k1, buffer, pos, length1 - k1);
                        return pos + length1 - k1;
                    }
                    s1 = set1[k1];
                    s2 = set2[k2];
                }
                else
                {// if (set1[k1]>set2[k2])
                    buffer[pos++] = s2;
                    ++k2;
                    if (k2 >= length2)
                    {
                        Array.Copy(set1, k1, buffer, pos, length1 - k1);
                        return pos + length1 - k1;
                    }
                    s2 = set2[k2];
                }
            }
            //return pos;
        }


        /**
         * Intersect two sorted lists and write the result to the provided
         * output array
         *
         * @param set1    first array
         * @param length1 length of first array
         * @param set2    second array
         * @param length2 length of second array
         * @param buffer  output array
         * @return cardinality of the intersection
         */
        public static int unsignedIntersect2by2(ushort[] set1,
                                                int length1, ushort[] set2, int length2,
                                                ushort[] buffer) {
            if (set1.Length * 64 < set2.Length) {
                return unsignedOneSidedGallopingIntersect2by2(set1, length1, set2, length2, buffer);
            } else if (set2.Length * 64 < set1.Length) {
                return unsignedOneSidedGallopingIntersect2by2(set2, length2, set1, length1, buffer);
            } else {
                return unsignedLocalIntersect2by2(set1, length1, set2, length2, buffer);
            }
        }

        protected static int unsignedLocalIntersect2by2(ushort[] set1,
                                                        int length1, ushort[] set2, int length2,
                                                        ushort[] buffer) {
            if ((0 == length1) || (0 == length2))
                return 0;
            int k1 = 0;
            int k2 = 0;
            int pos = 0;
            ushort s1 = set1[k1];
            ushort s2 = set2[k2];

            bool breakflag = false;
            while (!breakflag)
            {
                int v1 = s1;
                int v2 = s2;
                if (v2 < v1) {
                    do {
                        ++k2;
                        if (k2 == length2)
                        {
                            breakflag = true;
                            break;
                        }

                        s2 = set2[k2];
                        v2 = s2;
                    } while (v2 < v1);
                } else if (v1 < v2) {
                    do {
                        ++k1;
                        if (k1 == length1)
                        {
                            breakflag = true;
                            break;
                        }

                        s1 = set1[k1];
                        v1 = s1;
                    } while (v1 < v2);
                } else {
                    // (set2[k2] == set1[k1])
                    buffer[pos++] = s1;
                    ++k1;
                    if (k1 == length1)
                        break;
                    ++k2;
                    if (k2 == length2)
                        break;
                    s1 = set1[k1];
                    s2 = set2[k2];
                }
            }
            return pos;
        }

        protected static int unsignedOneSidedGallopingIntersect2by2(
                ushort[] smallSet, int smallLength,
                ushort[] largeSet, int largeLength,
                ushort[] buffer) {
            if (0 == smallLength)
                return 0;
            
            int k1 = 0;
            int k2 = 0;
            int pos = 0;
            ushort s1 = largeSet[k1];
            ushort s2 = smallSet[k2];

            while (true) {
                if (s1 < s2) {
                    k1 = advanceUntil(largeSet, k1, largeLength, s2);
                    if (k1 == largeLength)
                        break;
                    s1 = largeSet[k1];
                }
                if (s2 < s1) {
                    ++k2;
                    if (k2 == smallLength)
                        break;
                    s2 = smallSet[k2];
                } else {
                    // (set2[k2] == set1[k1])
                    buffer[pos++] = s2;
                    ++k2;
                    if (k2 == smallLength)
                        break;
                    s2 = smallSet[k2];
                    k1 = advanceUntil(largeSet, k1, largeLength, s2);
                    if (k1 == largeLength)
                        break;
                    s1 = largeSet[k1];
                }

            }
            return pos;

        }

        /// <summary>
        /// Find the smallest integer larger than pos such that array[pos] >= min.
        /// If none can be found, return length. Based on code by O. Kaser.
        /// </summary>
        /// <param name="array">array to search within</param>
        /// <param name="pos">starting position of the search</param>
        /// <param name="length">length of the array to search</param>
        /// <param name="min">minimum value</param>
        /// <returns>x greater than pos such that array[pos] is at least as large
        /// as min, pos is is equal to length if it is not possible.</returns>
        public static int advanceUntil(ushort[] array, int pos, int length, ushort min)
        {
            int lower = pos + 1;

            // special handling for a possibly common sequential case
            if (lower >= length || array[lower] >= min)
            {
                return lower;
            }

            int spansize = 1; // could set larger
            // bootstrap an upper limit

            while (lower + spansize < length && toIntUnsigned(array[lower + spansize]) < toIntUnsigned(min))
                spansize *= 2; // hoping for compiler will reduce to
            // shift
            int upper = (lower + spansize < length) ? lower + spansize : length - 1;

            // maybe we are lucky (could be common case when the seek ahead
            // expected
            // to be small and sequential will otherwise make us look bad)
            if (array[upper] == min)
            {
                return upper;
            }

            if (toIntUnsigned(array[upper]) < toIntUnsigned(min))
            {// means
                // array
                // has no
                // item
                // >= min
                // pos = array.length;
                return length;
            }

            // we know that the next-smallest span was too small
            lower += (spansize / 2);

            // else begin binary search
            // invariant: array[lower]<min && array[upper]>min
            while (lower + 1 != upper)
            {
                int mid = (lower + upper) / 2;
                ushort arraymid = array[mid];
                if (arraymid == min)
                {
                    return mid;
                }
                else if (arraymid < min)
                    lower = mid;
                else
                    upper = mid;
            }
            return upper;

        }

        public static int select(long w, int j) {
            ulong word = (ulong) w;
            int seen = 0;
            // Divide 64bit
            uint part = (uint) word & 0xFFFFFFFF;
            int n = longBitCount(part);
            if (n <= j) {
                part = (uint) (word >> 32);
                seen += 32;
                j -= n;
            }
            uint ww = part;

            // Divide 32bit
            part = ww & 0xFFFF;

            n = longBitCount(part);
            if (n <= j) {

                part = ww >> 16;
                seen += 16;
                j -= n;
            }
            ww = part;

            // Divide 16bit
            part = ww & 0xFF;
            n = longBitCount(part);
            if (n <= j) {
                part = ww >> 8;
                seen += 8;
                j -= n;
            }
            ww = part;

            // Lookup in final byte
            int counter;
            for (counter = 0; counter < 8; counter++) {
                j -= (int)(ww >> counter) & 1;
                if (j < 0) {
                    break;
                }
            }
            return seen + counter;
        }
    }
}

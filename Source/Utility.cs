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
            //https://stackoverflow.com/questions/6097635/checking-cpu-popcount-from-c-sharp
            ulong result = (ulong)w - (((ulong)w >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (byte)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
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
                    container[pos++] = (ushort) ((k << 6) + longBitCount(t - 1));
                    bitset ^= t;
                }
            }
        }

        /// <summary>
        /// compute the difference between two long arrays and write the set
        /// bits in the container
        /// </summary>
        /// <param name="container">where we write</param>
        /// <param name="bitmap1">first bitmap</param>
        /// <param name="bitmap2">second bitmap</param>
        public static void fillArrayDIFF(
            ref ushort[] container, 
            long[] bitmap1,
            long[] bitmap2)
        {
            int pos = 0;

            if (bitmap1.Length != bitmap2.Length)
                throw new ArgumentOutOfRangeException("not supported");

            for (int k = 0; k < bitmap1.Length; ++k)
            {
                long bitset = bitmap1[k] & (~bitmap2[k]);

                while (bitset != 0)
                {
                    long t = bitset & -bitset;
                    container[pos++] = (ushort) ((k << 6) + longBitCount(t - 1));
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
            if ((set1.Length << 6) < set2.Length) {
                return unsignedOneSidedGallopingIntersect2by2(set1, length1, set2, length2, buffer);
            } else if ((set2.Length << 6) < set1.Length) {
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

        /**
         * Gets the elements of the first sorted lists
         * that are not in the second and write the result
         * to the provided output array.
         *
         * @param set1    first array
         * @param length1 length of first array
         * @param set2    second array
         * @param length2 length of second array
         * @param buffer  output array
         * @return cardinality of the difference
         */
        public static int unsignedDifference2by2(ushort[] set1,
                                                int length1, ushort[] set2, int length2,
                                                ushort[] buffer)
        {
            int cardinality = 0;

            int pos1 = 0;
            int pos2 = 0;

            ushort item1;
            ushort item2;

            while (pos1 < length1 && pos2 < length2)
            {
                item1 = set1[pos1];
                item2 = set2[pos2];
                if (item1 == item2)
                {
                    // The element exists in both so not in the difference, just move on
                    pos1++;
                    pos2++;    
                }
                else if (item1 < item2)
                {
                    // item1 is different, add and advance search position in set1
                    buffer[cardinality] = item1;
                    pos1++;
                    cardinality++;
                }
                else
                {
                    // item1 may be different or may not, we need to continue searching
                    // set2 to see if there might be a match later in set2
                    pos2++;
                }
            }

            // Handle the case where we finish searching set2 before set1 or set2 is empty
            if (pos1 < length1) 
            {
                // All remaining elements in set1 are not in set2 so we copy them to the result
                // in the case where set 2 was empty, this means copy everything to the result.
                Array.Copy(set1, pos1, buffer, cardinality, length1-pos1);
                cardinality += length1-pos1;
            }

            return cardinality;

        }

        /**
         * Determines if elements overlap between the sets.
         *
         * @param set1    first array
         * @param length1 length of first array
         * @param set2    second array
         * @param length2 length of second array
         * @return true if values are shared between the sets
         */
        public static bool unsignedOverlaps2by2(ushort[] set1, int length1, 
                                                ushort[] set2, int length2)
        {
            int pos1 = 0;
            int pos2 = 0;

            ushort item1;
            ushort item2;

            while (pos1 < length1 && pos2 < length2)
            {
                item1 = set1[pos1];
                item2 = set2[pos2];
                if (item1 == item2)
                {
                    // The element is shared return true
                    return true;
                }
                else if (item1 < item2)
                {
                    // item1 is not in set2, advance the search position in set1
                    pos1++;
                }
                else
                {
                    // item1 may be in set2 or may not, we need to continue searching
                    // advance the search position in set2
                    pos2++;
                }
            }

            return false;

        }

        /// <summary>
        /// This is an Array extension method analogous to Java's Array.fill().
        /// Fills a certain range of array indices with a specific value.
        /// </summary>
        /// <param name="array">array to modify</param>
        /// <param name="start">the starting index</param>
        /// <param name="end">the ending index</param>
        /// <param name="value">value to set</param>
        public static void Fill<T>(T[] array, int start, int end, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (start < 0 || start > end)
            {
                throw new ArgumentOutOfRangeException("fromIndex");
            }
            if (end > array.Length)
            {
                throw new ArgumentOutOfRangeException("toIndex");
            }
            for (int i = start; i < end; i++)
            {
                array[i] = value;
            }
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
        
        /// <summary>
        /// clear bits at start, start+1,..., end-1
        /// </summary>
        /// <param name="bitmap">bitmap array of words to be modified</param>
        /// <param name="start">start first index to be modified (inclusive)</param>
        /// <param name="end">end last index to be modified (exclusive)</param>
        public static void resetBitmapRange(long[] bitmap, int start, int end)
        {
            if (start == end) return;

            int firstword = start / 64;
            int endword = (end - 1) / 64;

            if (firstword == endword)
            {
                bitmap[firstword] &= ~((~0L << start) & (long)(~0UL >> -end));
                return;
            }

            bitmap[firstword] &= ~(~0L << start);

            for (int i = firstword + 1; i < endword; i++)
                bitmap[i] = 0;

            bitmap[endword] &= (long)~(~0UL >> -end);
        }

        /// <summary>
        /// set bits at start, start+1,..., end-1
        /// </summary>
        /// <param name="bitmap">array of words to be modified</param>
        /// <param name="start">first index to be modified (inclusive)</param>
        /// <param name="end">last index to be modified (exclusive)</param>
        public static void setBitmapRange(long[] bitmap, ushort start, ushort end)
        {
            if (start == end) return;

            int firstword = start / 64;
            int endword = (end - 1) / 64;

            if (firstword == endword)
            {
                bitmap[firstword] |= (~0L << start) & (long)(~0UL >> -end);
                return;
            }

            bitmap[firstword] |= ~0L << start;

            for (int i = firstword + 1; i < endword; i++)
                bitmap[i] = ~0L;

            bitmap[endword] |= (long)(~0UL >> -end);
        }

        #region DeBruijn Sequence BitScan
        /// <summary>
        /// This sequence is used to optimize bit seek operations.
        /// For more info see: https://en.wikipedia.org/wiki/De_Bruijn_sequence
        /// </summary>
        private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

        /// <summary>
        /// This lookup is used to optimize bit seek operations.
        /// For more info see: https://en.wikipedia.org/wiki/De_Bruijn_sequence
        /// </summary>
        private static readonly ushort[] MultiplyDeBruijnBitPosition =
        {
            0, 1, 17, 2, 18, 50, 3, 57,
            47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6,
            61, 44, 12, 35, 60, 11, 10, 9,
        };

        /// <summary>
        /// Search the mask data from least significant bit (LSB) to the most significant bit (MSB) for a set bit (1)
        /// using De Bruijn sequence approach. Warning: Will return zero for b = 0.
        /// </summary>
        /// <param name="w">Word to scan through.</param>
        /// <returns>Zero-based position of LSB (from right to left).</returns>
        public static ushort BitScanForward(ulong w)
        {
            return MultiplyDeBruijnBitPosition[((ulong)((long)w & -(long)w) * DeBruijnSequence) >> 58];
        }
        #endregion
    }
}

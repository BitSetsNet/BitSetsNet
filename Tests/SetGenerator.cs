using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    public class SetGenerator
    {

        private static Random numberGenerator = new Random();
        private SetGenerator()
        {
            //can't instantiate this
        }

        public static int[] GetRandomArray(int length, int maxNumberOfOnes = 0)
        {

            if (maxNumberOfOnes == 0) { maxNumberOfOnes = length; }

            var indexSet = new HashSet<int>();
           
            int numberOfOnes = Math.Max(numberGenerator.Next(maxNumberOfOnes), 1);

            for (int i = 0; i<numberOfOnes; i++)
            {
                indexSet.Add(numberGenerator.Next(length));
            }

            return indexSet.ToArray();
        }

        public static int[] GetContiguousArray(int start, int end)
        {
            int[] set = new int[end - start];
            for (int i = start; i < end; i++)
            {
                set[i - start] = i;
            }

            return set;
        }

        /// <summary>
        /// Knocks out a few selected values from a contiguous array to allow meaningful
        /// difference testing.This would ordinarily be done by just using GetContiguousArray
        /// and flipping the bits off with Set() or Flip(), but those are not currently
        /// Implemented in RLE.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public static int[] GetContiguousArrayWithExceptions(int start, int end, int[] exceptions)
        {
            int irrelevantExceptions = 0;
            foreach (int exception in exceptions)
            {
                if (exception > end || exception < start)
                {
                    // We're not going to come across these exceptions in
                    // set generation.
                    irrelevantExceptions++;
                }
            }

            int relevantExceptions = exceptions.Length - irrelevantExceptions;
            int[] set = new int[end - start - relevantExceptions];
            int skipped = 0;
            for (int i = start; i < end; i++)
            {
                if(exceptions.Contains(i))
                {
                    skipped++;
                }
                else
                {
                    set[i - start - skipped] = i;
                }
            }

            return set;
        }
    }
}

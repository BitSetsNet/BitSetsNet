using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}

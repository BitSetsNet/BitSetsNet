using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitsetsNET.Tests
{
    /// <summary>
    /// Summary description for IBitSet
    /// </summary>
    [TestClass]
    public abstract class BaseBitSetTests
    {

        const int TEST_SET_LENGTH = 10;
        const int TEST_ITERATIONS = 10;
        protected abstract IBitset CreateSetFromIndices(int[] indices, int length);

        [TestMethod()]
        public virtual void AndTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Intersect(second).ToArray();
            IBitset expected = CreateSetFromIndices(result, TEST_SET_LENGTH);
            IBitset actual = CreateSetFromIndices(first, TEST_SET_LENGTH).And(CreateSetFromIndices(second, TEST_SET_LENGTH));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public virtual void AndWithTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Intersect(second).ToArray();
            IBitset testSet = CreateSetFromIndices(first, TEST_SET_LENGTH);
            testSet.AndWith(CreateSetFromIndices(second, TEST_SET_LENGTH));

            Assert.AreEqual(CreateSetFromIndices(result, TEST_SET_LENGTH), testSet);
  
        }

        [TestMethod()]
        public virtual void CloneTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            var clone = testSet.Clone();
            Assert.AreEqual(clone, testSet);
        }

        [TestMethod()]
        public virtual void GetTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            bool expected = set.Contains(2);
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void OrTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Union(second).ToArray();
            IBitset expected = CreateSetFromIndices(result, TEST_SET_LENGTH);
            IBitset actual = CreateSetFromIndices(first, TEST_SET_LENGTH).Or(CreateSetFromIndices(second, TEST_SET_LENGTH));

            Assert.AreEqual(expected, actual, generateMessage("OrWith", first, second, result));

        }

        [TestMethod()]
        public virtual void OrWithTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Union(second).ToArray();

            IBitset testSet = CreateSetFromIndices(first, TEST_SET_LENGTH);
            testSet.OrWith(CreateSetFromIndices(second, TEST_SET_LENGTH));

            Assert.AreEqual(CreateSetFromIndices(result, TEST_SET_LENGTH), testSet, generateMessage("OrWith", first, second, result));

        }

        [TestMethod()]
        public virtual void SetTrueTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            testSet.Set(8, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetFalseTest()
        {
            int[] set = { 1, 2, 3 };
            IBitset testSet = CreateSetFromIndices(set, 4);
            testSet.Set(2, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeTrueTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            testSet.Set(7,9, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeFalseTest()
        {
            int[] set = { 1, 2, 3 };
            IBitset testSet = CreateSetFromIndices(set, 4);
            testSet.Set(1, 3, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipTrueTest()
        {
            int[] set = { 1, 2, 3, 5 };
            IBitset testSet = CreateSetFromIndices(set, 6);
            testSet.Flip(4);
            bool expected = true;
            bool result = testSet.Get(4);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipFalseTest()
        {
            int[] set = { 1, 2, 3, 5 };
            IBitset testSet = CreateSetFromIndices(set, 6);
            testSet.Flip(2);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipRangeTrueTest()
        {
            int[] set = { 1, 2, 3, 7 };
            IBitset testSet = CreateSetFromIndices(set, 8);
            testSet.Flip(4,6);
            bool expected = true;
            bool result = testSet.Get(5);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipRangeFalseTest()
        {
            int[] set = { 1, 2, 3, 7 };
            IBitset testSet = CreateSetFromIndices(set, 8);
            testSet.Flip(2,4);
            bool expected = false;
            bool result = testSet.Get(3);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void DifferenceTest()
        {
            int[] set1 = { 1, 2, 3, 7 };
            IBitset testSet1 = CreateSetFromIndices(set1, 8);

            int[] set2 = { 1, 4, 7 };
            IBitset testSet2 = CreateSetFromIndices(set2, 8);

            // These sparse sets will all use array containers.
            IBitset arrayContainerDiffSet = testSet1.Difference(testSet2);

            Assert.AreEqual(false, arrayContainerDiffSet.Get(1));
            Assert.AreEqual(true, arrayContainerDiffSet.Get(3));

            // Test difference from large contiguous bitset to exercise bitsetcontainers.
            int[] set3 = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet3 = CreateSetFromIndices(set3, 5000);

            int[] setExceptions = { 4 };
            int[] set4 = SetGenerator.GetContiguousArrayWithExceptions(0, 5000, setExceptions);
            IBitset testSet4 = CreateSetFromIndices(set4, 5000);

            // Both sets are using bitset containers
            IBitset bitsetContainerDiffSet = testSet3.Difference(testSet4);

            Assert.AreEqual(false, bitsetContainerDiffSet.Get(1));
            Assert.AreEqual(true, bitsetContainerDiffSet.Get(4));

            // Diff sets using bitset containers with array containers and vice versa
            IBitset mixedDiffSet1 = testSet4.Difference(testSet2);
            IBitset mixedDiffSet2 = testSet2.Difference(testSet4);

            Assert.AreEqual(false, mixedDiffSet1.Get(1));
            Assert.AreEqual(true, mixedDiffSet1.Get(3));

            Assert.AreEqual(false, mixedDiffSet2.Get(1));
            Assert.AreEqual(true, mixedDiffSet2.Get(4));
        }

        [TestMethod]
        public virtual void ToBitArrayTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            BitArray setArray = new BitArray(TEST_SET_LENGTH);

            foreach (int index in set)
            {
                setArray[index] = true;
            }

            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            BitArray testArray = testSet.ToBitArray();

            bool expected = true;
            bool actual = true;

            for (int i = 0; i < setArray.Length; i++)
            {
                if (setArray[i])
                {
                    actual &= setArray[i] == testArray[i];
                }
                else
                {
                    //do nothing
                }
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public virtual void CardinalityTest()
        {
            int[] set = SetGenerator.GetContiguousArray(1, 5000);
            IBitset testSet = CreateSetFromIndices(set, set.Max() + 1);

            int expected = set.Length;
            int actual = testSet.Cardinality();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public virtual void EnumerationTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndices(set, TEST_SET_LENGTH);
            List<int> enumeratedList = new List<int>();
            foreach (int i in testSet)
            {
                enumeratedList.Add(i);
            }
            CollectionAssert.AreEquivalent(enumeratedList.ToArray(), set);
        }

        private string generateMessage(string functionName, int[] setA, int[] setB, int[] expected)
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Testing " + functionName);
            builder.AppendLine("Set A " + generateSetMessage(setA));
            builder.AppendLine("Set B " + generateSetMessage(setB));
            builder.AppendLine("Expected " + generateSetMessage(expected));

            return builder.ToString();
        }

        private string generateSetMessage(int[] indicies)
        {
            var builder = new System.Text.StringBuilder();
            foreach (int i in indicies)
            {
                builder.Append(i); builder.Append(',');
            }

            return builder.ToString();
        }

    }
}

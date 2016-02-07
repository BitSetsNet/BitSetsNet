using System;
using System.Text;
using System.Linq;
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
        protected abstract IBitset CreateSetFromIndicies(int[] indices, int length);

        [TestMethod()]
        public virtual void AndTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Intersect(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, TEST_SET_LENGTH);
            IBitset actual = CreateSetFromIndicies(first, TEST_SET_LENGTH).And(CreateSetFromIndicies(second, TEST_SET_LENGTH));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public virtual void AndWithTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Intersect(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, TEST_SET_LENGTH);
            testSet.AndWith(CreateSetFromIndicies(second, TEST_SET_LENGTH));

            Assert.AreEqual(CreateSetFromIndicies(result, TEST_SET_LENGTH), testSet);
  
        }

        [TestMethod()]
        public virtual void CloneTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndicies(set, TEST_SET_LENGTH);
            var clone = testSet.Clone();
            Assert.AreEqual(clone, testSet);
        }

        [TestMethod()]
        public virtual void GetTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndicies(set, TEST_SET_LENGTH);
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
            IBitset expected = CreateSetFromIndicies(result, TEST_SET_LENGTH);
            IBitset actual = CreateSetFromIndicies(first, TEST_SET_LENGTH).Or(CreateSetFromIndicies(second, TEST_SET_LENGTH));

            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public virtual void OrWithTest()
        {
            int[] first = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] second = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            int[] result = first.Union(second).ToArray();

            IBitset testSet = CreateSetFromIndicies(first, TEST_SET_LENGTH);
            testSet.OrWith(CreateSetFromIndicies(second, TEST_SET_LENGTH));

            Assert.AreEqual(CreateSetFromIndicies(result, TEST_SET_LENGTH), testSet, generateMessage("OrWith", first, second, result));

        }

        [TestMethod()]
        public virtual void SetTrueTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndicies(set, TEST_SET_LENGTH);
            testSet.Set(8, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetFalseTest()
        {
            int[] set = { 1, 2, 3 };
            IBitset testSet = CreateSetFromIndicies(set, 4);
            testSet.Set(2, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeTrueTest()
        {
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            IBitset testSet = CreateSetFromIndicies(set, TEST_SET_LENGTH);
            testSet.Set(7,9, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeFalseTest()
        {
            int[] set = { 1, 2, 3 };
            IBitset testSet = CreateSetFromIndicies(set, 4);
            testSet.Set(1, 3, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetAllTest()
        {

        }

        [TestMethod()]
        public virtual void FlipTrueTest()
        {
            int[] set = { 1, 2, 3, 5 };
            IBitset testSet = CreateSetFromIndicies(set, 6);
            testSet.Flip(4);
            bool expected = true;
            bool result = testSet.Get(4);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipFalseTest()
        {
            int[] set = { 1, 2, 3, 5 };
            IBitset testSet = CreateSetFromIndicies(set, 6);
            testSet.Flip(2);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipRangeTrueTest()
        {
            int[] set = { 1, 2, 3, 7 };
            IBitset testSet = CreateSetFromIndicies(set, 8);
            testSet.Flip(4,6);
            bool expected = true;
            bool result = testSet.Get(5);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void FlipRangeFalseTest()
        {
            int[] set = { 1, 2, 3, 7 };
            IBitset testSet = CreateSetFromIndicies(set, 8);
            testSet.Flip(2,4);
            bool expected = false;
            bool result = testSet.Get(3);
            Assert.AreEqual(expected, result);
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

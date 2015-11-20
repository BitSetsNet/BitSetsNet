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
        protected BitsetsNET.IBitset OrResult { get; set; }
        protected KeyValuePair<int, bool> GetResult { get; set; }
        protected int ExpectedLength = 0;

        protected abstract IBitset CreateSetFromIndicies(int[] indices, int length);

        [TestMethod()]
        public virtual void AndTest()
        {
            int[] first = { 1, 2, 3 };
            int[] second = { 1, 4, 5 };
            int[] result = first.Intersect(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, 10);
            IBitset actual = CreateSetFromIndicies(first, 10).And(CreateSetFromIndicies(second, 10));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public virtual void AndWithTest()
        {
            int[] first = { 1, 2, 3 };
            int[] second = { 1, 4, 5 };
            int[] result = first.Intersect(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, 10);
            testSet.AndWith(CreateSetFromIndicies(second, 10));

            Assert.AreEqual(CreateSetFromIndicies(result, 10), testSet);
  
        }

        [TestMethod()]
        public virtual void CloneTest()
        {
            int[] set = { 1, 5, 7 };
            IBitset testSet = CreateSetFromIndicies(set, 10);
            var clone = testSet.Clone();
            Assert.AreEqual(clone, testSet);
        }

        [TestMethod()]
        public virtual void GetTest()
        {
            int[] set = { 2, 7, 9 };
            IBitset testSet = CreateSetFromIndicies(set, 10);
            bool expected = true;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void LengthTest()
        {
            int[] set = { 1 };
            IBitset testSet = CreateSetFromIndicies(set, 10);
            Assert.AreEqual(10, testSet.Length());
        }

        [TestMethod()]
        public virtual void OrTest()
        {
            int[] first = { 1, 2, 3 };
            int[] second = { 1, 4, 5 };
            int[] result = first.Union(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, 10);
            IBitset actual = CreateSetFromIndicies(first, 10).Or(CreateSetFromIndicies(second, 10));

            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public virtual void OrWithTest()
        {
            int[] first = { 1, 2, 3 };
            int[] second = { 1, 4, 5 };
            int[] result = first.Union(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, 10);
            testSet.OrWith(CreateSetFromIndicies(second, 10));

            Assert.AreEqual(CreateSetFromIndicies(result, 10), testSet);
        }

        [TestMethod()]
        public virtual void SetTest()
        {

        }

        [TestMethod()]
        public virtual void SetAllTest()
        {

        }

    }
}

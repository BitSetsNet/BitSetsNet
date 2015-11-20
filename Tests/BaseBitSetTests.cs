using System;
using System.Text;
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
        protected BitsetsNET.IBitset TestSet { get; set; }
        protected BitsetsNET.IBitset TestSet2 { get; set; }
        protected BitsetsNET.IBitset AndResult { get; set; }
        protected BitsetsNET.IBitset OrResult { get; set; }
        protected KeyValuePair<int, bool> GetResult { get; set; }
        protected int ExpectedLength = 0; 

        [TestMethod()]
        public virtual void AndTest()
        {
            Assert.AreEqual(AndResult, TestSet.And(TestSet2));
        }

        [TestMethod()]
        public virtual void AndWithTest()
        {
            TestSet.AndWith(TestSet2);
            Assert.AreEqual(AndResult, TestSet);
        }

        [TestMethod()]
        public virtual void CloneTest()
        {
            var clone = TestSet.Clone();
            Assert.AreEqual(clone, TestSet);
        }

        [TestMethod()]
        public virtual void GetTest()
        {
            bool result = TestSet.Get(GetResult.Key);
            Assert.AreEqual(result, GetResult.Value);
        }

        [TestMethod()]
        public virtual void LengthTest()
        {
            Assert.AreEqual(ExpectedLength, TestSet.Length());
        }

        [TestMethod()]
        public virtual void OrTest()
        {
            Assert.AreEqual(TestSet, TestSet2);

        }

        [TestMethod()]
        public virtual void OrWithTest()
        {

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

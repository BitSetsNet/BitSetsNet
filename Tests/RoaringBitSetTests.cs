﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    [TestClass()]
    public class RoaringBitSetTests : BaseBitSetTests 
    {
        protected override IBitset CreateSetFromIndices(int[] indices, int length)
        {
            return RoaringBitset.Create(indices);
        }

        [TestMethod()]
        public virtual void SetTrueLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(9, 5009);
            IBitset testSet = CreateSetFromIndices(set, 5000);
            testSet.Set(8, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetFalseLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet = CreateSetFromIndices(set, 5000);
            testSet.Set(2, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeTrueLargeTest()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet = CreateSetFromIndices(set, 5000);
            testSet.Set(5007, 5009, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetRangeFalseLargeTest()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000); ;
            IBitset testSet = CreateSetFromIndices(set, 5000);
            testSet.Set(1, 3, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void DifferenceWithTest()
        {
            int[] set1 = { 1, 2, 3, 7 };
            RoaringBitset testSet1 = RoaringBitset.Create(set1);

            int[] set2 = { 1, 7 };
            RoaringBitset testSet2 = RoaringBitset.Create(set2);

            testSet1.DifferenceWith(testSet2);

            bool expected1 = false;
            bool result1 = testSet1.Get(1);
            Assert.AreEqual(expected1, result1);

            bool expected2 = true;
            bool result2 = testSet1.Get(3);
            Assert.AreEqual(expected2, result2);
        }
    }
}

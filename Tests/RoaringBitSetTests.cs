using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            // Test arrayContainer-based sets
            int[] set1 = { 1, 2, 3, 7 };
            RoaringBitset testSet1 = RoaringBitset.Create(set1);

            int[] set2 = { 1, 4, 7 };
            RoaringBitset testSet2 = RoaringBitset.Create(set2);

            testSet1.DifferenceWith(testSet2);

            Assert.AreEqual(false, testSet1.Get(1));
            Assert.AreEqual(true, testSet1.Get(3));

            // Test bitsetContainer-based sets
            int[] set3 = SetGenerator.GetContiguousArray(0, 5000);
            RoaringBitset testSet3 = RoaringBitset.Create(set3);

            int[] setExceptions = { 4 };
            int[] set4 = SetGenerator.GetContiguousArrayWithExceptions(0, 5000, setExceptions);
            RoaringBitset testSet4 = RoaringBitset.Create(set4);

            // Reduce contiguous array to single value (4) via DifferenceWith
            testSet3.DifferenceWith(testSet4);

            Assert.AreEqual(false, testSet3.Get(2));
            Assert.AreEqual(true, testSet3.Get(4));

            // 
            // Reduce testSet2 to 4 as well
            testSet2.DifferenceWith(testSet4);
            Assert.AreEqual(false, testSet2.Get(1));
            Assert.AreEqual(true, testSet2.Get(4));

            // Remove contents of set1 from set4
            testSet4.DifferenceWith(testSet1);
            Assert.AreEqual(false, testSet4.Get(2));
            Assert.AreEqual(true, testSet4.Get(6));

        }
    }
}

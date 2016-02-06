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
        protected override IBitset CreateSetFromIndicies(int[] indices, int length)
        {
            return RoaringBitset.Create(indices);
        }

        [TestMethod()]
        public virtual void SetTrueLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(9, 5009);
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(8, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public virtual void SetFalseLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(2, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.AreEqual(expected, result);
        }
    }
}

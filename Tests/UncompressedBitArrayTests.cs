using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitsetsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    [TestClass()]
    public class UncompressedBitArrayTests : BaseBitSetTests
    {
        [TestMethod]
        public override void OrTest()
        {
            int[] first = { 1, 2 };
            int[] second = { 2, 3 };
            int[] result = { 1, 2, 3 };
            TestSet = new UncompressedBitArray(first, 3);
            TestSet2 = new UncompressedBitArray(second, 3);
            OrResult = new UncompressedBitArray(result);

            base.OrTest();
        }

        
    }
}
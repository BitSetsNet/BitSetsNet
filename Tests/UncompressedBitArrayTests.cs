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

        protected override IBitset CreateSetFromIndices(int[] indices, int length)
        {
            return new UncompressedBitArray(indices, length);
        }
    }
}
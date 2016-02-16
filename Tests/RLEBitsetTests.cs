using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    [TestClass()]
    public class RLEBitsetTests : BaseBitSetTests
    {
        protected override IBitset CreateSetFromIndices(int[] indices, int length)
        {
            return RLEBitset.CreateFrom(indices, length);
        }
    }
}

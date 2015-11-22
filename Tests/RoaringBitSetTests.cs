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
            return RoaringBitset.Crte(indices);
        }
    }
}

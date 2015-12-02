using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    class RLEBitsetTests : BaseBitSetTests
    {
        protected override IBitset CreateSetFromIndicies(int[] indices, int length)
        {
            return RLE.RLEBitset.CreateFrom(indices);
        }
    }
}

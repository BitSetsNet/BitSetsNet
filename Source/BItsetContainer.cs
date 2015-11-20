using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class BitsetContainer : Container
    {
        protected const int MAX_CAPACITY = 1 << 64;

        int cardinality;
        long[] bitmap; //Should we use a BitArray object?

        public BitsetContainer()
        {
            this.cardinality = 0;
            this.bitmap = new long[MAX_CAPACITY / 64];
        }

    }
}

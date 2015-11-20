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


        public override Container add(short x)
        {
            throw new NotImplementedException();
        }

        public override Container and(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container and(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container clone()
        {
            throw new NotImplementedException();
        }

        public override bool contains(short x)
        {
            throw new NotImplementedException();
        }

        public override void fillLeastSignificant16bits(int[] x, int i, int mask)
        {
            throw new NotImplementedException();
        }

        public override int getCardinality()
        {
            throw new NotImplementedException();
        }

        public override Container iand(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container iand(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        public override bool intersects(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        public override bool intersects(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container ior(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container ior(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container or(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container or(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        public override Container remove(short x)
        {
            throw new NotImplementedException();
        }

        public override short select(int j)
        {
            throw new NotImplementedException();
        }

    }
}

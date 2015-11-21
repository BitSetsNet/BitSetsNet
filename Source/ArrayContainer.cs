using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class ArrayContainer : Container
    {
        private const int DEFAULT_INIT_SIZE = 4;
        private const int DEFAULT_MAX_SIZE = 4096;

        public int cardinality;
        public ushort[] content;

        public ArrayContainer() : this(DEFAULT_INIT_SIZE) {}
        
        public ArrayContainer(int capacity)
        {
            this.cardinality = 0;
            this.content = new ushort[capacity];
        }

        public override Container add(ushort x)
        {
            int loc = Utility.unsignedBinarySearch(content, 0, cardinality, x);

            // if the location is positive, it means the number being added already existed in the
            // array, so no need to do anything.

            // if the location is negative, we did not find the value in the array. The location represents
            // the negative value of the position in the array (not the index) where we want to add the value
            if (loc < 0) {
                // Transform the ArrayContainer to a BitmapContainer
                // when cardinality = DEFAULT_MAX_SIZE
                if (cardinality >= DEFAULT_MAX_SIZE) {
                    BitsetContainer a = this.toBitsetContainer();
                    a.add(x);
                    return a;
                }
                if (cardinality >= this.content.Length)
                    increaseCapacity();

                // insertion : shift the elements > x by one position to the right
                // and put x in its appropriate place
                Array.Copy(content, -loc - 1, content, -loc, cardinality + loc + 1);
                content[-loc - 1] = x;
                ++cardinality;
            }
            return this;
        }

        //TODO: This needs to be optimized. It should increase capacity by more than just 1 each time
        public void increaseCapacity()
        {
            int currCapacity = this.content.Length;
            //TODO: Tori says this may be jank
            Array.Resize(ref this.content, currCapacity + 1);
        }

        public BitsetContainer toBitsetContainer()
        {
            BitsetContainer bc = new BitsetContainer();
            bc.loadData(this);
            return bc;
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

        public override bool contains(ushort x)
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

        public override Container remove(ushort x)
        {
            throw new NotImplementedException();
        }

        public override short select(int j)
        {
            throw new NotImplementedException();
        }

    }
}

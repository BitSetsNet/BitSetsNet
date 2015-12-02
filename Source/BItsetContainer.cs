using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    public class BitsetContainer : Container
    {
        protected static int MAX_CAPACITY = 1 << 16;

        public int cardinality;
        public long[] bitmap;

        public BitsetContainer()
        {
            this.cardinality = 0;
            this.bitmap = new long[MAX_CAPACITY / 64];
        }

        public BitsetContainer(int cardinality, long[] bitmap)
        {
            this.cardinality = cardinality;
            this.bitmap = bitmap;
        }

        public override Container add(ushort x)
        {
            long prevVal = bitmap[x / 64];
            long newVal = prevVal | (1L << x);
            bitmap[x / 64] = newVal;
            if (prevVal != newVal) ++cardinality;
            return this;
        }

        public void loadData(ArrayContainer arrayContainer)
        {
            this.cardinality = arrayContainer.cardinality;
            for (int k = 0; k < arrayContainer.cardinality; ++k)
            {
                ushort x = arrayContainer.content[k];
                bitmap[x / 64] |= (1L << x);
            }
        }

         /**
         * Copies the data to an array container
         *
         * @return the array container
         */
        public ArrayContainer toArrayContainer()
        {
            ArrayContainer ac = new ArrayContainer(cardinality);
            ac.loadData(this);
            return ac;
        }

        /**
         * Fill the array with set bits
         *
         * @param array container (should be sufficiently large)
         */
        public void fillArray(ushort[] array)
        {
            int pos = 0;
            for (int k = 0; k < bitmap.Length; ++k)
            {
                long bitset = bitmap[k];
                while (bitset != 0)
                {
                    long t = bitset & -bitset;
                    array[pos++] = (ushort)(k * 64 + Utility.longBitCount(t - 1));
                    bitset ^= t;
                }
            }
        }

        public override Container and(BitsetContainer value2)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.bitmap.Length; ++k) {
                newCardinality += Utility.longBitCount(
                    this.bitmap[k] & value2.bitmap[k]
                );
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE) {
                BitsetContainer answer = new BitsetContainer();
                for (int k = 0; k < answer.bitmap.Length; ++k) {
                    answer.bitmap[k] = this.bitmap[k]
                            & value2.bitmap[k];
                }
                answer.cardinality = newCardinality;
                return answer;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.fillArrayAND(ref ac.content, this.bitmap, value2.bitmap);
            ac.cardinality = newCardinality;
            return ac;
        }

        public override Container and(ArrayContainer x)
        {
            ArrayContainer answer = new ArrayContainer(x.content.Length);
            int c = x.cardinality;
            for (int i=0; i<c; i++)
            {
                ushort v = x.content[i];
                if (this.contains(v))
                    answer.content[answer.cardinality++] = v;
            }
            return answer;
        }

        public override Container clone()
        {
            long[] newBitmap = new long[this.bitmap.Length];
            this.bitmap.CopyTo(newBitmap, 0);

            return new BitsetContainer(this.cardinality, newBitmap);
        }

        public override bool contains(ushort x)
        {
            return (bitmap[x / 64] & (1L << x)) != 0;
        }

        public override void fillLeastSignificant16bits(int[] x, int i, int mask)
        {
            throw new NotImplementedException();
        }

        public override int getCardinality()
        {
            return cardinality;
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

        public override Container or(BitsetContainer value2)
        {
            BitsetContainer answer = new BitsetContainer();
            answer.cardinality = 0;
            for (int k = 0; k < answer.bitmap.Length; ++k) {
                long w = this.bitmap[k] | value2.bitmap[k];
                answer.bitmap[k] = w;
                answer.cardinality += Utility.longBitCount(w);
            }
            return answer;
        }

        public override Container or(ArrayContainer value2)
        {
            BitsetContainer answer = (BitsetContainer) clone();

            int c = value2.cardinality;
            for (int k = 0; k < c ; ++k) {
                ushort v = value2.content[k];
                int i = v >> 6;
                long w = answer.bitmap[i];
                long aft = w | (1L << v);

                answer.bitmap[i] = aft;
                //if (USE_BRANCHLESS) {
                answer.cardinality += (int)((w - aft) >> 63);
                //} else {
                //    if (w != aft)
                //        answer.cardinality++;
                //}
            }
            return answer;
        }

        public override Container remove(ushort x)
        {
            throw new NotImplementedException();
        }

        public override ushort select(int j)
        {
            int leftover = j;
            for (int k = 0; k < bitmap.Length; ++k)
            {
                int w = Utility.longBitCount(bitmap[k]);
                if (w > leftover)
                {
                    return (ushort)(k * 64 + Utility.select(bitmap[k], leftover));
                }
                leftover -= w;
            }
            throw new ArgumentOutOfRangeException("Insufficient cardinality.");
        }

        public override bool Equals(Object o) {
            if (o is BitsetContainer) {
                BitsetContainer srb = (BitsetContainer)o;
                if (srb.cardinality != this.cardinality)
                    return false;
                return Array.Equals(this.bitmap, srb.bitmap);
            } 
            return false;
        }

    }
}

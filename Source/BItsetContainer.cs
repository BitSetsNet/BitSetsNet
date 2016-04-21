using System;
using System.Collections.Generic;
using System.IO;
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
        
        /// <summary>
        /// Recomputes the cardinality of the bitmap.
        /// </summary>
        protected void computeCardinality()
        {
            this.cardinality = 0;
            for (int k = 0; k < this.bitmap.Length; k++)
            {
                this.cardinality += Utility.longBitCount(this.bitmap[k]);
            }
        }

        public override Container add(ushort x)
        {
            long prevVal = bitmap[x / 64];
            long newVal = prevVal | (1L << x);
            bitmap[x / 64] = newVal;
            if (prevVal != newVal) ++cardinality;
            return this;
        }

        public override Container add(ushort rangeStart, ushort rangeEnd)
        {
            // TODO: may need to convert to a RunContainer
            Utility.setBitmapRange(bitmap, rangeStart, rangeEnd);
            computeCardinality();
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

        public override Container flip(ushort i)
        {
            int x = Utility.toIntUnsigned(i);
            int index = x / 64;
            long bef = bitmap[index];
            long mask = 1L << x;
            if (cardinality == ArrayContainer.DEFAULT_MAX_SIZE + 1)
            {// this is
             // the
             // uncommon
             // path
                if ((bef & mask) != 0)
                {
                    --cardinality;
                    bitmap[index] &= ~mask;
                    return this.toArrayContainer();
                }
            }
            // TODO: check whether a branchy version could be faster
            cardinality += 1 - 2 * (int)((bef & mask) >> x);
            bitmap[index] ^= mask;
            return this;
        }

        public override Container iadd(ushort begin, ushort end)
        {
            // TODO: may need to convert to a RunContainer
            if (end == begin)
            {
                return this;
            }
            if (begin > end)
            {
                throw new ArgumentException("Invalid range [" + begin + "," + end + ")");
            }
            Utility.setBitmapRange(bitmap, begin, end);
            computeCardinality();
            return this;
        }

        public override Container inot(int firstOfRange, int lastOfRange)
        {
            if (lastOfRange - firstOfRange == MAX_CAPACITY)
            {
                Utility.flipBitsetRange(bitmap, firstOfRange, lastOfRange);
                cardinality = MAX_CAPACITY - cardinality;
            }
            else if (lastOfRange - firstOfRange > MAX_CAPACITY / 2)
            {
                Utility.flipBitsetRange(bitmap, firstOfRange, lastOfRange);
                computeCardinality();
            }
            else
            {
                cardinality += Utility.flipBitsetRangeAndCardinalityChange(bitmap, firstOfRange, lastOfRange);
            }
            if (cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return toArrayContainer();
            }
            return this;
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

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// ArrayContainer.
        /// </summary>
        /// <param name="x">the ArrayContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container andNot(ArrayContainer x)
        {
            BitsetContainer answer = (BitsetContainer) clone();
            int c = x.cardinality;
            for (int k = 0; k < c; ++k)
            {
                ushort v = x.content[k];
                uint i = (uint) (Utility.toIntUnsigned(v) >> 6);
                long w = answer.bitmap[i];
                long aft = w & (~(1L << v));
                answer.bitmap[i] = aft;
                answer.cardinality -= (int) ((w ^ aft) >> v);
            }
            if (answer.cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
                return answer.toArrayContainer();
            return answer;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// other BitsetContainer.
        /// </summary>
        /// <param name="x">the other BitsetContainer</param>
        /// <returns>A new container with the differences</returns>
        public override Container andNot(BitsetContainer x)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.bitmap.Length; ++k)
            {
                newCardinality += Utility.longBitCount(this.bitmap[k]
                        & (~x.bitmap[k]));
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                BitsetContainer answer = new BitsetContainer();
                for (int k = 0; k < answer.bitmap.Length; ++k)
                {
                    answer.bitmap[k] = this.bitmap[k]
                            & (~x.bitmap[k]);
                }
                answer.cardinality = newCardinality;
                return answer;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.fillArrayANDNOT(ac.content, this.bitmap, x.bitmap);
            ac.cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// ArrayContainer by modifying the current container in place. 
        /// </summary>
        /// <param name="x">the ArrayContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container iandNot(ArrayContainer x)
        {
            for (int k = 0; k < x.cardinality; ++k)
            {
                this.remove(x.content[k]);
            }
            if (cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
                return this.toArrayContainer();
            return this;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// other BitsetContainer. Depending on the cardinality of the result, 
        /// this will either modify the container in place or return a new ArrayContainer.
        /// </summary>
        /// <param name="x">the other BitsetContainer</param>
        /// <returns>The current container, modified the differences</returns>
        public override Container iandNot(BitsetContainer x)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.bitmap.Length; ++k)
            {
                newCardinality += Utility.longBitCount(this.bitmap[k] & (~x.bitmap[k]));
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                for (int k = 0; k < this.bitmap.Length; ++k)
                {
                    this.bitmap[k] = this.bitmap[k] & (~x.bitmap[k]);
                }
                this.cardinality = newCardinality;
                return this;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.fillArrayANDNOT(ac.content, this.bitmap, x.bitmap);
            ac.cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Performs an intersection with another BitsetContainer. Depending on
        /// the cardinality of the result, this will either modify the container
        /// in place or return a new ArrayContainer.
        /// </summary>
        /// <param name="other">the other BitsetContainer to intersect</param>
        public override Container iand(BitsetContainer other)
        {
            int newCardinality = 0;
            for (int k = 0; k < bitmap.Length; ++k)
            {
                newCardinality += Utility.longBitCount(
                    bitmap[k] & other.bitmap[k]
                );
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                for (int k = 0; k < bitmap.Length; ++k)
                {
                    bitmap[k] = bitmap[k]
                            & other.bitmap[k];
                }
                cardinality = newCardinality;
                return this;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.fillArrayAND(ref ac.content, bitmap, other.bitmap);
            ac.cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Performs an "in-place" intersection with an ArrayContainer. Since
        /// no in-place operation is actually possible, this method defaults to
        /// calling ArrayContainer's and() method with this as input.
        /// </summary>
        /// <param name="other">the ArrayContainer to intersect</param>
        public override Container iand(ArrayContainer other)
        {
            return other.and(this); // No in-place possible
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
            this.cardinality = 0;
            for (int k = 0; k < this.bitmap.Length; ++k)
            {
                long w = this.bitmap[k] | x.bitmap[k];
                this.bitmap[k] = w;
                this.cardinality += Utility.longBitCount(w);
            }
            return this;
        }

        public override Container ior(ArrayContainer x)
        {
            int c = x.cardinality;
            for (int k = 0; k < c; ++k)
            {
                ushort v = x.content[k];
                int i = v >> 6;

                long w = this.bitmap[i];
                long aft = w | (1L << v);
                this.bitmap[i] = aft;

                this.cardinality += (int)((w - aft) >> 63);

            }
            return this;
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
            int index = x / 64;
            long bef = bitmap[index];
            long mask = (1L << x);

            if (cardinality == ArrayContainer.DEFAULT_MAX_SIZE + 1)
            {// this is
             // the
             // uncommon
             // path
                if ((bef & mask) != 0)
                {
                    --cardinality;
                    bitmap[x / 64] = bef & (~mask);
                    return this.toArrayContainer();
                }
            }

            long aft = bef & (~mask);
            cardinality -= (aft - bef) != 0 ? 1 : 0;
            bitmap[index] = aft;
            return this;
        }
        
        public override Container remove(ushort begin, ushort end)
        {
            Utility.resetBitmapRange(bitmap, begin, end);
            computeCardinality();

            if (getCardinality() <= ArrayContainer.DEFAULT_MAX_SIZE)
                return toArrayContainer();

            return this;
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

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = cardinality;
                for (int i = 0; i < bitmap.Length; i++)
                {
                    hash = 17 * hash + (int) bitmap[i];
                }
                return hash;
            }
        }

        /// <summary>
        /// Serialize this container in a binary format.
        /// </summary>
        /// <param name="writer">The writer to which to serialize this container.</param>
        /// <remarks>The format of the serialization is the cardinality of this container as a 32-bit integer, followed by the bit array. The cardinality is used in deserialization to distinguish BitsetContainers from ArrayContainers.</remarks>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(cardinality);
            foreach(long value in bitmap)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Deserialize a container from binary format, as written by the Serialize method minus the first 32 bits giving the cardinality.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The first container represented by reader.</returns>
        public static BitsetContainer Deserialize(BinaryReader reader, int cardinality)
        {
            BitsetContainer container = new BitsetContainer();

            container.cardinality = cardinality;
            for(int i = 0; i < container.bitmap.Length; i++)
            {
                container.bitmap[i] = reader.ReadInt64();
            }

            return container;
        }

        public override IEnumerator<ushort> GetEnumerator()
        {
            ushort index = 0;
            foreach(long value in bitmap)
            {
                if (value != 0)
                {
                    //copy value so that we can modify it
                    ulong val = (ulong)value;
                    for (int i = 0; i < 64; i++)
                    {
                        if ((val & 1) == 1)
                        {
                            yield return index;
                        }
                        val >>= 1;
                        index++;
                    }
                }
                else
                {
                    index += 64;
                }
            }
        }
    }
}

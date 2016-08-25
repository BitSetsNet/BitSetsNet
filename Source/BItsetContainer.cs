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

        public int Cardinality;
        public long[] Bitmap;

        public BitsetContainer()
        {
            this.Cardinality = 0;
            this.Bitmap = new long[MAX_CAPACITY / 64];
        }

        public BitsetContainer(int cardinality, long[] bitmap)
        {
            this.Cardinality = cardinality;
            this.Bitmap = bitmap;
        }
        
        /// <summary>
        /// Recomputes the cardinality of the bitmap.
        /// </summary>
        protected void ComputeCardinality()
        {
            this.Cardinality = 0;
            for (int k = 0; k < this.Bitmap.Length; k++)
            {
                this.Cardinality += Utility.LongBitCount(this.Bitmap[k]);
            }
        }

        /// <summary>
        /// Add a short to the container. May generate a new container.
        /// </summary>
        /// <param name="x">short to be added</param>
        /// <returns>this container, modified</returns>
        public override Container Add(ushort x)
        {
            long prevVal = Bitmap[x / 64];
            long newVal = prevVal | (1L << x);
            Bitmap[x / 64] = newVal;
            if (prevVal != newVal)
            {
                ++Cardinality;
            }
            return this;
        }

        /// <summary>
        /// Add to the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        /// <returns>this container, modified</returns>
        public override Container Add(ushort rangeStart, ushort rangeEnd)
        {
            // TODO: may need to convert to a RunContainer
            Utility.SetBitmapRange(Bitmap, rangeStart, rangeEnd);
            ComputeCardinality();
            return this;
        }

        public void LoadData(ArrayContainer arrayContainer)
        {
            this.Cardinality = arrayContainer.Cardinality;
            for (int k = 0; k < arrayContainer.Cardinality; ++k)
            {
                ushort x = arrayContainer.Content[k];
                Bitmap[x / 64] |= (1L << x);
            }
        }

        /// <summary>
        /// Copies the data to an array container
        /// </summary>
        /// <returns>The array container</returns>
        public ArrayContainer ToArrayContainer()
        {
            ArrayContainer ac = new ArrayContainer(Cardinality);
            ac.LoadData(this);
            return ac;
        }


        /// <summary>
        /// Fill the array with set bits.
        /// </summary>
        /// <param name="array">Container (should be sufficiently large)</param>
        public void FillArray(ushort[] array)
        {
            int pos = 0;
            for (int k = 0; k < Bitmap.Length; ++k)
            {
                long bitset = Bitmap[k];
                while (bitset != 0)
                {
                    long t = bitset & -bitset;
                    array[pos++] = (ushort)(k * 64 + Utility.LongBitCount(t - 1));
                    bitset ^= t;
                }
            }
        }

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container And(BitsetContainer x)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.Bitmap.Length; ++k)
            {
                newCardinality += Utility.LongBitCount(this.Bitmap[k] & x.Bitmap[k]);
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                BitsetContainer answer = new BitsetContainer();
                for (int k = 0; k < answer.Bitmap.Length; ++k)
                {
                    answer.Bitmap[k] = this.Bitmap[k] & x.Bitmap[k];
                }
                answer.Cardinality = newCardinality;
                return answer;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.FillArrayAND(ref ac.Content, this.Bitmap, x.Bitmap);
            ac.Cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container And(ArrayContainer x)
        {
            ArrayContainer answer = new ArrayContainer(x.Content.Length);
            int c = x.Cardinality;
            for (int i=0; i<c; i++)
            {
                ushort v = x.Content[i];
                if (this.Contains(v))
                {
                    answer.Content[answer.Cardinality++] = v;
                }
            }
            return answer;
        }

        /// <summary>
        /// Add a short to the container if it is not present, otherwise remove it. May generate a new
        /// container.
        /// </summary>
        /// <param name="i">short to be added</param>
        /// <returns>the new container</returns>
        public override Container Flip(ushort i)
        {
            int x = Utility.ToIntUnsigned(i);
            int index = x / 64;
            long bef = Bitmap[index];
            long mask = 1L << x;
            if (Cardinality == ArrayContainer.DEFAULT_MAX_SIZE + 1)
            {// this is
             // the
             // uncommon
             // path
                if ((bef & mask) != 0)
                {
                    --Cardinality;
                    Bitmap[index] &= ~mask;
                    return this.ToArrayContainer();
                }
            }
            // TODO: check whether a branchy version could be faster
            Cardinality += 1 - 2 * (int)((bef & mask) >> x);
            Bitmap[index] ^= mask;
            return this;
        }

        /// <summary>
        /// Add all shorts in [begin,end) using an unsigned interpretation. May generate a new container.
        /// </summary>
        /// <param name="begin">Start of range</param>
        /// <param name="end">End of range</param>
        /// <returns>The new container</returns>
        public override Container IAdd(ushort begin, ushort end)
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
            Utility.SetBitmapRange(Bitmap, begin, end);
            ComputeCardinality();
            return this;
        }

        /// <summary>
        /// Computes the in-place bitwise NOT of this container (complement). Only those bits within the
        /// range are affected.The current container is generally modified.May generate a new container.
        /// </summary>
        /// <param name="rangeStart">beginning of range (inclusive); 0 is beginning of this container.</param>
        /// <param name="rangeEnd">ending of range (exclusive)</param>
        /// <returns>(Partially) complemented container</returns>
        public override Container INot(int rangeStart, int rangeEnd)
        {
            if (rangeEnd - rangeStart == MAX_CAPACITY)
            {
                Utility.FlipBitsetRange(Bitmap, rangeStart, rangeEnd);
                Cardinality = MAX_CAPACITY - Cardinality;
            }
            else if (rangeEnd - rangeStart > MAX_CAPACITY / 2)
            {
                Utility.FlipBitsetRange(Bitmap, rangeStart, rangeEnd);
                ComputeCardinality();
            }
            else
            {
                Cardinality += Utility.FlipBitsetRangeAndCardinalityChange(Bitmap, rangeStart, rangeEnd);
            }

            if (Cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return ToArrayContainer();
            }
            return this;
        }

        /// <summary>
        /// Creates a deep copy of this bitset container.
        /// </summary>
        /// <returns>The cloned bitset container</returns>
        public override Container Clone()
        {
            long[] newBitmap = new long[this.Bitmap.Length];
            this.Bitmap.CopyTo(newBitmap, 0);

            return new BitsetContainer(this.Cardinality, newBitmap);
        }

        /// <summary>
        /// Checks whether the container contains the provided value.
        /// </summary>
        /// <param name="x">Value to check</param>
        public override bool Contains(ushort x)
        {
            return (Bitmap[x / 64] & (1L << x)) != 0;
        }

        /// <summary>
        /// Fill the least significant 16 bits of the integer array, starting at
        /// index i, with the short values from this container. The caller is
        /// responsible to allocate enough room. The most significant 16 bits of
        /// each integer are given by the most significant bits of the provided mask.
        /// </summary>
        /// <param name="x">Provided array</param>
        /// <param name="i">Starting index</param>
        /// <param name="mask">Indicates most significant bits</param>
        public override void FillLeastSignificant16bits(int[] x, int i, int mask)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the distinct number of short values in the container. Can be
        /// expected to run in constant time.
        /// </summary>
        /// <returns>The cardinality</returns>
        public override int GetCardinality()
        {
            return Cardinality;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// ArrayContainer.
        /// </summary>
        /// <param name="x">the ArrayContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container AndNot(ArrayContainer x)
        {
            BitsetContainer answer = (BitsetContainer) Clone();
            int c = x.Cardinality;
            for (int k = 0; k < c; ++k)
            {
                ushort v = x.Content[k];
                uint i = (uint) (Utility.ToIntUnsigned(v) >> 6);
                long w = answer.Bitmap[i];
                long aft = w & (~(1L << v));
                answer.Bitmap[i] = aft;
                answer.Cardinality -= (int) ((w ^ aft) >> v);
            }

            if (answer.Cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return answer.ToArrayContainer();
            }
            return answer;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// other BitsetContainer.
        /// </summary>
        /// <param name="x">the other BitsetContainer</param>
        /// <returns>A new container with the differences</returns>
        public override Container AndNot(BitsetContainer x)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.Bitmap.Length; ++k)
            {
                newCardinality += Utility.LongBitCount(this.Bitmap[k] & (~x.Bitmap[k]));
            }

            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                BitsetContainer answer = new BitsetContainer();
                for (int k = 0; k < answer.Bitmap.Length; ++k)
                {
                    answer.Bitmap[k] = this.Bitmap[k] & (~x.Bitmap[k]);
                }
                answer.Cardinality = newCardinality;
                return answer;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.FillArrayANDNOT(ac.Content, this.Bitmap, x.Bitmap);
            ac.Cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// ArrayContainer by modifying the current container in place. 
        /// </summary>
        /// <param name="x">the ArrayContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container IAndNot(ArrayContainer x)
        {
            for (int k = 0; k < x.Cardinality; ++k)
            {
                this.Remove(x.Content[k]);
            }
            if (Cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return this.ToArrayContainer();
            }
            return this;
        }

        /// <summary>
        /// Returns the elements of this BitsetContainer that are not in the
        /// other BitsetContainer. Depending on the cardinality of the result, 
        /// this will either modify the container in place or return a new ArrayContainer.
        /// </summary>
        /// <param name="x">the other BitsetContainer</param>
        /// <returns>The current container, modified the differences</returns>
        public override Container IAndNot(BitsetContainer x)
        {
            int newCardinality = 0;
            for (int k = 0; k < this.Bitmap.Length; ++k)
            {
                newCardinality += Utility.LongBitCount(this.Bitmap[k] & (~x.Bitmap[k]));
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                for (int k = 0; k < this.Bitmap.Length; ++k)
                {
                    this.Bitmap[k] = this.Bitmap[k] & (~x.Bitmap[k]);
                }
                this.Cardinality = newCardinality;
                return this;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.FillArrayANDNOT(ac.Content, this.Bitmap, x.Bitmap);
            ac.Cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Performs an intersection with another BitsetContainer. Depending on
        /// the cardinality of the result, this will either modify the container
        /// in place or return a new ArrayContainer.
        /// </summary>
        /// <param name="other">the other BitsetContainer to intersect</param>
        public override Container IAnd(BitsetContainer other)
        {
            int newCardinality = 0;
            for (int k = 0; k < Bitmap.Length; ++k)
            {
                newCardinality += Utility.LongBitCount(Bitmap[k] & other.Bitmap[k]);
            }
            if (newCardinality > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                for (int k = 0; k < Bitmap.Length; ++k)
                {
                    Bitmap[k] = Bitmap[k] & other.Bitmap[k];
                }
                Cardinality = newCardinality;
                return this;
            }
            ArrayContainer ac = new ArrayContainer(newCardinality);
            Utility.FillArrayAND(ref ac.Content, Bitmap, other.Bitmap);
            ac.Cardinality = newCardinality;
            return ac;
        }

        /// <summary>
        /// Performs an "in-place" intersection with an ArrayContainer. Since
        /// no in-place operation is actually possible, this method defaults to
        /// calling ArrayContainer's and() method with this as input.
        /// </summary>
        /// <param name="other">the ArrayContainer to intersect</param>
        public override Container IAnd(ArrayContainer other)
        {
            return other.And(this); // No in-place possible
        }

        /// <summary>
        /// Returns true if the current container intersects the other container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Whether they intersect</returns>
        public override bool Intersects(BitsetContainer x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the current container intersects the other container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Whether they intersect</returns>
        public override bool Intersects(ArrayContainer x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the in-place bitwise OR of this container with another
        /// (union). The current container is generally modified, whereas the
        /// provided container(x) is unaffected.May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container IOr(BitsetContainer x)
        {
            this.Cardinality = 0;
            for (int k = 0; k < this.Bitmap.Length; ++k)
            {
                long w = this.Bitmap[k] | x.Bitmap[k];
                this.Bitmap[k] = w;
                this.Cardinality += Utility.LongBitCount(w);
            }
            return this;
        }

        /// <summary>
        /// Computes the in-place bitwise OR of this container with another
        /// (union). The current container is generally modified, whereas the
        /// provided container(x) is unaffected.May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container IOr(ArrayContainer x)
        {
            int c = x.Cardinality;
            for (int k = 0; k < c; ++k)
            {
                ushort v = x.Content[k];
                int i = v >> 6;

                long w = this.Bitmap[i];
                long aft = w | (1L << v);
                this.Bitmap[i] = aft;

                this.Cardinality += (int)((w - aft) >> 63);
            }
            return this;
        }

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container Or(BitsetContainer x)
        {
            BitsetContainer answer = new BitsetContainer();
            answer.Cardinality = 0;
            for (int k = 0; k < answer.Bitmap.Length; ++k) {
                long w = this.Bitmap[k] | x.Bitmap[k];
                answer.Bitmap[k] = w;
                answer.Cardinality += Utility.LongBitCount(w);
            }
            return answer;
        }

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container Or(ArrayContainer x)
        {
            BitsetContainer answer = (BitsetContainer) Clone();

            int c = x.Cardinality;
            for (int k = 0; k < c ; ++k)
            {
                ushort v = x.Content[k];
                int i = v >> 6;
                long w = answer.Bitmap[i];
                long aft = w | (1L << v);

                answer.Bitmap[i] = aft;
                answer.Cardinality += (int)((w - aft) >> 63);
            }
            return answer;
        }

        /// <summary>
        /// Remove specified short from this container. May create a new container.
        /// </summary>
        /// <param name="x">Short to be removed</param>
        /// <returns>The new container</returns>
        public override Container Remove(ushort x)
        {
            int index = x / 64;
            long bef = Bitmap[index];
            long mask = (1L << x);

            if (Cardinality == ArrayContainer.DEFAULT_MAX_SIZE + 1)
            {// this is
             // the
             // uncommon
             // path
                if ((bef & mask) != 0)
                {
                    --Cardinality;
                    Bitmap[x / 64] = bef & (~mask);
                    return this.ToArrayContainer();
                }
            }

            long aft = bef & (~mask);
            Cardinality -= (aft - bef) != 0 ? 1 : 0;
            Bitmap[index] = aft;
            return this;
        }

        /// <summary>
        /// Remove shorts in [begin,end) using an unsigned interpretation. May generate a new container.
        /// </summary>
        /// <param name="begin">Start of range (inclusive)</param>
        /// <param name="end">End of range (exclusive)</param>
        /// <returns>The new container</returns>
        public override Container Remove(ushort begin, ushort end)
        {
            Utility.ResetBitmapRange(Bitmap, begin, end);
            ComputeCardinality();

            if (GetCardinality() <= ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return ToArrayContainer();
            }

            return this;
        }

        /// <summary>
        /// Return the jth value of the container.
        /// </summary>
        /// <param name="j">Index of the value </param>
        /// <returns>The jth value of the container</returns>
        public override ushort Select(int j)
        {
            int leftover = j;
            for (int k = 0; k < Bitmap.Length; ++k)
            {
                int w = Utility.LongBitCount(Bitmap[k]);
                if (w > leftover)
                {
                    return (ushort)(k * 64 + Utility.Select(Bitmap[k], leftover));
                }
                leftover -= w;
            }
            throw new ArgumentOutOfRangeException("Insufficient cardinality.");
        }

        public override bool Equals(Object o)
        {
            if (o is BitsetContainer)
            {
                BitsetContainer srb = (BitsetContainer)o;
                if (srb.Cardinality != this.Cardinality)
                {
                    return false;
                }
                return Array.Equals(this.Bitmap, srb.Bitmap);
            } 
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Cardinality;
                for (int i = 0; i < Bitmap.Length; i++)
                {
                    hash = 17 * hash + (int) Bitmap[i];
                }
                return hash;
            }
        }

        /// <summary>
        /// Serialize this container in a binary format.
        /// </summary>
        /// <param name="writer">The writer to which to serialize this container.</param>
        /// <remarks>The format of the serialization is the cardinality of this container as 
        /// a 32-bit integer, followed by the bit array. The cardinality is used in 
        /// deserialization to distinguish BitsetContainers from ArrayContainers.</remarks>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Cardinality);
            foreach(long value in Bitmap)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Deserialize a container from binary format, as written by the Serialize method minus 
        /// the first 32 bits giving the cardinality.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The first container represented by reader.</returns>
        public static BitsetContainer Deserialize(BinaryReader reader, int cardinality)
        {
            BitsetContainer container = new BitsetContainer();

            container.Cardinality = cardinality;
            for(int i = 0; i < container.Bitmap.Length; i++)
            {
                container.Bitmap[i] = reader.ReadInt64();
            }

            return container;
        }

        public override IEnumerator<ushort> GetEnumerator()
        {
            ushort index = 0;
            foreach(long value in Bitmap)
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

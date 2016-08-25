using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    public class ArrayContainer : Container
    {
        private const int DEFAULT_INIT_SIZE = 4;
        internal const int DEFAULT_MAX_SIZE = 4096;

        public int Cardinality;
        public ushort[] Content;

        public ArrayContainer() : this(DEFAULT_INIT_SIZE) {}
        
        public ArrayContainer(int capacity)
        {
            this.Cardinality = 0;
            this.Content = new ushort[capacity];
        }

        private ArrayContainer(int cardinality, ushort[] inpContent)
        {
            this.Cardinality = cardinality;
            this.Content = inpContent;
        }

        public ArrayContainer(ushort[] newContent)
        {
            this.Cardinality = newContent.Length;
            this.Content = newContent;
        }

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container Add(ushort x)
        {
            int loc = Utility.UnsignedBinarySearch(Content, 0, Cardinality, x);

            // if the location is positive, it means the number being added already existed in the
            // array, so no need to do anything.

            // if the location is negative, we did not find the value in the array. The location represents
            // the negative value of the position in the array (not the index) where we want to add the value
            if (loc < 0)
            {
                // Transform the ArrayContainer to a BitmapContainer
                // when cardinality = DEFAULT_MAX_SIZE
                if (Cardinality >= DEFAULT_MAX_SIZE)
                {
                    BitsetContainer a = this.ToBitsetContainer();
                    a.Add(x);
                    return a;
                }
                if (Cardinality >= this.Content.Length)
                {
                    IncreaseCapacity();
                }

                // insertion : shift the elements > x by one position to the right
                // and put x in its appropriate place
                Array.Copy(Content, -loc - 1, Content, -loc, Cardinality + loc + 1);
                Content[-loc - 1] = x;
                ++Cardinality;
            }
            return this;
        }

        /// <summary>
        /// Add to the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        public override Container Add(ushort rangeStart, ushort rangeEnd)
        {
            int indexstart = Utility.UnsignedBinarySearch(Content, 0, Cardinality, rangeStart);
            if (indexstart < 0)
            {
                indexstart = -indexstart - 1;
            }
            int indexend = Utility.UnsignedBinarySearch(Content, 0, Cardinality, (ushort)(rangeEnd - 1));

            if (indexend < 0)
            {
                indexend = -indexend - 1;
            }
            else
            {
                indexend++;
            }

            int rangelength = rangeEnd - rangeStart;
            int newcardinality = indexstart + (Cardinality - indexend) + rangelength;

            if (newcardinality > DEFAULT_MAX_SIZE)
            {
                BitsetContainer a = this.ToBitsetContainer();
                return a.Add(rangeStart, rangeEnd);
            }

            if (newcardinality >= this.Content.Length)
            {
                IncreaseCapacity(newcardinality);
            }

            Array.Copy(Content, indexend, this.Content, indexstart + rangelength, Cardinality - indexend);

            for (int k = 0; k < rangelength; ++k)
            {
                this.Content[k + indexstart] = (ushort)(rangeStart + k);
            }

            this.Cardinality = newcardinality;

            return this;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other ArrayContainer.
        /// </summary>
        /// <param name="x">the other ArrayContainer</param>
        /// <returns>A new container with the differences</returns>
        public override Container AndNot(ArrayContainer x)
        {
            int desiredCapacity = this.GetCardinality();
            var answer = new ArrayContainer(desiredCapacity);

            // Compute the cardinality of the new container
            answer.Cardinality = Utility.UnsignedDifference(this.Content,
                                                            desiredCapacity,
                                                            x.Content,
                                                            x.GetCardinality(),
                                                            answer.Content);
            return answer;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other BitSetContainer.
        /// </summary>
        /// <param name="x">the BitSetContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container AndNot(BitsetContainer x)
        {
            var answer = new ArrayContainer(Content.Length);
            int pos = 0;
            for (int k = 0; k < Cardinality; ++k)
            {
                ushort val = this.Content[k];
                if (!x.Contains(val))
                {
                    answer.Content[pos++] = val;
                }
            }
            answer.Cardinality = pos;
            return answer;
        }

        /// <summary>
        /// If elements is present in container, add it. Otherwise, remove it.
        /// </summary>
        /// <param name="x">Element to add</param>
        /// <returns>Modified container</returns>
        public override Container Flip(ushort x)
        {
            int loc = Utility.UnsignedBinarySearch(Content, 0, Cardinality, x);
            if (loc < 0)
            {
                // Transform the ArrayContainer to a BitmapContainer
                // when cardinality = DEFAULT_MAX_SIZE
                if (Cardinality >= DEFAULT_MAX_SIZE)
                {
                    BitsetContainer a = this.ToBitsetContainer();
                    a.Add(x);
                    return a;
                }
                if (Cardinality >= this.Content.Length)
                {
                    IncreaseCapacity();
                }
                // insertion : shift the elements > x by one position to
                // the right
                // and put x in it's appropriate place
                Array.Copy(Content, -loc - 1, Content, -loc, Cardinality + loc + 1);
                Content[-loc - 1] = x;
                ++Cardinality;
            }
            else
            {
                Array.Copy(Content, loc + 1, Content, loc, Cardinality - loc - 1);
                --Cardinality;
            }
            return this;
        }

        /// <summary>
        /// Adds range of elements (in-place) to this container.
        /// </summary>
        /// <param name="begin">Start of range (inclusive)</param>
        /// <param name="end">End of range (exclusive)</param>
        /// <returns>Modified container</returns>
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

            int indexStart = Utility.UnsignedBinarySearch(Content, 0, Cardinality, begin);
            if (indexStart < 0)
            {
                indexStart = -indexStart - 1;
            }

            int indexEnd = Utility.UnsignedBinarySearch(Content, 0, Cardinality, (ushort)(end - 1));
            if (indexEnd < 0)
            {
                indexEnd = -indexEnd - 1;
            }
            else
            {
                indexEnd++;
            }

            int rangeLength = end - begin;
            int newCardinality = indexStart + (Cardinality - indexEnd) + rangeLength;
            if (newCardinality > DEFAULT_MAX_SIZE)
            {
                BitsetContainer a = this.ToBitsetContainer();
                return a.IAdd(begin, end);
            }

            if (newCardinality >= this.Content.Length)
            {
                IncreaseCapacity(newCardinality);
            }

            Array.Copy(this.Content, indexEnd, this.Content, indexStart + rangeLength, Cardinality - indexEnd);
            for (int k = 0; k < rangeLength; ++k)
            {
                Content[k + indexStart] = (ushort)(begin + k);
            }
            Cardinality = newCardinality;
            return this;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other BitSetContainer. Modifies the current container in place.
        /// </summary>
        /// <param name="x">the BitSetContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container IAndNot(BitsetContainer x)
        {
            int pos = 0;
            for (int k = 0; k < Cardinality; ++k)
            {
                ushort v = this.Content[k];
                if (!x.Contains(v))
                {
                    this.Content[pos++] = v;
                }
            }
            this.Cardinality = pos;
            return this;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other ArrayContainer.
        /// </summary>
        /// <param name="x">the other ArrayContainer</param>
        /// <returns>The modified container</returns>
        public override Container IAndNot(ArrayContainer x)
        {
            this.Cardinality = Utility.UnsignedDifference(this.Content, 
                                                          this.GetCardinality(), 
                                                          x.Content, 
                                                          x.GetCardinality(), 
                                                          this.Content);
            return this;
        }

        /// <summary>
        /// Computes the in-place bitwise NOT of this container (complement). Only those bits within the
        /// range are affected.The current container is generally modified.May generate a new container.
        /// </summary>
        /// <param name="firstOfRange">beginning of range (inclusive); 0 is beginning of this container.</param>
        /// <param name="lastOfRange">ending of range (exclusive)</param>
        /// <returns>(Partially) complemented container</returns>
        public override Container INot(int firstOfRange, int lastOfRange)
        {
            // TODO: may need to convert to a RunContainer
            // determine the span of array indices to be affected
            int startIndex = Utility.UnsignedBinarySearch(Content, 0, Cardinality, (ushort)firstOfRange);
            if (startIndex < 0)
            {
                startIndex = -startIndex - 1;
            }

            int lastIndex = Utility.UnsignedBinarySearch(Content, 0, Cardinality, (ushort)(lastOfRange - 1));
            if (lastIndex < 0)
            {
                lastIndex = -lastIndex - 1 - 1;
            }
            int currentValuesInRange = lastIndex - startIndex + 1;
            int spanToBeFlipped = lastOfRange - firstOfRange;
            int newValuesInRange = spanToBeFlipped - currentValuesInRange;
            ushort[] buffer = new ushort[newValuesInRange];
            int cardinalityChange = newValuesInRange - currentValuesInRange;
            int newCardinality = Cardinality + cardinalityChange;

            if (cardinalityChange > 0)
            { // expansion, right shifting needed
                if (newCardinality > Content.Length)
                {
                    // so big we need a bitmap?
                    if (newCardinality > DEFAULT_MAX_SIZE)
                    {
                        return ToBitsetContainer().INot(firstOfRange, lastOfRange);
                    }
                    // Change the size of the array based on the new cardinality
                    Array.Resize(ref Content, newCardinality);
                }
                // slide right the contents after the range
                Array.Copy(Content, 
                           lastIndex + 1, Content, 
                           lastIndex + 1 + cardinalityChange,
                           Cardinality - 1 - lastIndex);
                NegateRange(buffer, startIndex, lastIndex, firstOfRange, lastOfRange);
            }
            else
            { // no expansion needed
                NegateRange(buffer, startIndex, lastIndex, firstOfRange, lastOfRange);
                if (cardinalityChange < 0)
                {
                    // contraction, left sliding.
                    // Leave array oversize
                    Array.Copy(Content, startIndex + newValuesInRange - cardinalityChange, 
                               Content, startIndex + newValuesInRange, 
                               newCardinality - (startIndex + newValuesInRange));
                }
            }
            Cardinality = newCardinality;
            return this;
        }

        // for use in inot range known to be nonempty
        private void NegateRange(ushort[] buffer, int startIndex, int lastIndex, int startRange, int lastRange)
        {
            // compute the negation into buffer

            int outPos = 0;
            int inPos = startIndex; // value here always >= valInRange,
                                    // until it is exhausted
                                    // n.b., we can start initially exhausted.

            int valInRange = startRange;
            for (; valInRange < lastRange && inPos <= lastIndex; ++valInRange)
            {
                if ((short)valInRange != Content[inPos])
                {
                    buffer[outPos++] = (ushort)valInRange;
                }
                else
                {
                    ++inPos;
                }
            }

            // if there are extra items (greater than the biggest
            // pre-existing one in range), buffer them
            for (; valInRange < lastRange; ++valInRange)
            {
                buffer[outPos++] = (ushort)valInRange;
            }

            if (outPos != buffer.Length)
            {
                throw new SystemException("negateRange: outPos " + outPos + 
                                          " whereas buffer.length=" + buffer.Length);
            }
            // copy back from buffer...caller must ensure there is room
            int i = startIndex;
            foreach (ushort item in buffer)
            {
                Content[i++] = item;
            }
        }

        private void IncreaseCapacity(int min)
        {
            int newCapacity;
            if(this.Content.Length == 0)
            {
                newCapacity = DEFAULT_INIT_SIZE;
            }
            else if(this.Content.Length < 64)
            {
                newCapacity = this.Content.Length * 2;
            }
            else if(this.Content.Length < 1024)
            {
                newCapacity = this.Content.Length * 3 / 2;
            }
            else
            {
                newCapacity = 5 / 4;
            }

            if (newCapacity < min)
            {
                newCapacity = min;
            }
            // never allocate more than we will ever need
            if (newCapacity > ArrayContainer.DEFAULT_MAX_SIZE)
            {
                newCapacity = ArrayContainer.DEFAULT_MAX_SIZE;
            }
            // if we are within 1/16th of the max, go to max 
            if (newCapacity < ArrayContainer.DEFAULT_MAX_SIZE - ArrayContainer.DEFAULT_MAX_SIZE / 16)
            {
                newCapacity = ArrayContainer.DEFAULT_MAX_SIZE;
            }
            Array.Resize(ref this.Content, newCapacity);
        }

        //TODO: This needs to be optimized. It should increase capacity by more than just 1 each time
        public void IncreaseCapacity()
        {
            int currCapacity = this.Content.Length;
            //TODO: Tori says this may be jank
            Array.Resize(ref this.Content, currCapacity + 1);
        }

        public BitsetContainer ToBitsetContainer()
        {
            BitsetContainer bc = new BitsetContainer();
            bc.LoadData(this);
            return bc;
        }

        public void LoadData(BitsetContainer bitsetContainer)
        {
            this.Cardinality = bitsetContainer.Cardinality;
            bitsetContainer.FillArray(Content);
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
            return x.And(this);
        }

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container And(ArrayContainer value2)
        {
            ArrayContainer value1 = this;
            int desiredCapacity = Math.Min(value1.GetCardinality(), value2.GetCardinality());
            ArrayContainer answer = new ArrayContainer(desiredCapacity);
            answer.Cardinality = Utility.UnsignedIntersect2by2(value1.Content,
                                                               value1.GetCardinality(), 
                                                               value2.Content,
                                                               value2.GetCardinality(), 
                                                               answer.Content);
            return answer;
        }

        /// <summary>
        /// Creates a deep copy of this array container.
        /// </summary>
        /// <returns>Cloned array container</returns>
        public override Container Clone()
        {
            ushort[] newContent = new ushort[this.Content.Length];
            this.Content.CopyTo(newContent, 0);
            return new ArrayContainer(this.Cardinality, newContent);
        }

        /// <summary>
        /// Checks whether the container contains the provided value.
        /// </summary>
        /// <param name="x">Value to check</param>
        public override bool Contains(ushort x)
        {
            return Utility.UnsignedBinarySearch(Content, 0, Cardinality, x) >= 0;
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
        /// Computes the in-place bitwise AND of this container with another
        /// (intersection). The current container is generally modified, whereas
        /// the provided container (x) is unaffected. May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container IAnd(BitsetContainer other)
        {
            int pos = 0;
            for (int k = 0; k < Cardinality; k++)
            {
                ushort v = Content[k];
                if (other.Contains(v))
                {
                    Content[pos++] = v;
                }
            }
            Cardinality = pos;
            return this;
        }

        /// <summary>
        /// Computes the in-place bitwise AND of this container with another
        /// (intersection). The current container is generally modified, whereas
        /// the provided container (x) is unaffected. May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container IAnd(ArrayContainer other)
        {
            Cardinality = Utility.UnsignedIntersect2by2(Content,
                                                        GetCardinality(), 
                                                        other.Content,
                                                        other.GetCardinality(), 
                                                        Content);
            return this;
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
            return x.Or(this);
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
            return this.Or(x);
        }

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container Or(BitsetContainer x)
        {
            return x.Or((ArrayContainer) this);
        }

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public override Container Or(ArrayContainer x)
        {
            ArrayContainer value1 = this;
            int totalCardinality = value1.GetCardinality() + x.GetCardinality();
            if (totalCardinality > DEFAULT_MAX_SIZE)
            {
                // it could be a bitmap!
                BitsetContainer bc = new BitsetContainer();
                for (int k = 0; k < x.Cardinality; ++k)
                {
                    ushort v = x.Content[k];
                    int i = v >> 6;
                    bc.Bitmap[i] |= (1L << v);
                }
                for (int k = 0; k < this.Cardinality; ++k)
                {
                    ushort v = this.Content[k];
                    int i = v >> 6;
                    bc.Bitmap[i] |= (1L << v);
                }
                bc.Cardinality = 0;
                foreach (long k in bc.Bitmap)
                {
                    bc.Cardinality += Utility.LongBitCount(k);
                }
                if (bc.Cardinality <= DEFAULT_MAX_SIZE)
                {
                    return bc.ToArrayContainer();
                }
                return bc;
            }
            else
            {
                // remains an array container
                int desiredCapacity = totalCardinality; // Math.min(BitmapContainer.MAX_CAPACITY,
                                                        // totalCardinality);
                ArrayContainer answer = new ArrayContainer(desiredCapacity);
                answer.Cardinality = Utility.UnsignedUnion2by2(value1.Content,
                                                               value1.GetCardinality(), 
                                                               x.Content,
                                                               x.GetCardinality(), 
                                                               answer.Content);
                return answer;
            }
        }

        /// <summary>
        /// Remove specified short from this container. May create a new container.
        /// </summary>
        /// <param name="x">Short to be removed</param>
        /// <returns>The new container</returns>
        public override Container Remove(ushort x)
        {
            int loc = Utility.UnsignedBinarySearch(Content, 0, Cardinality, x);
            if (loc >= 0)
            {
                // insertion
                Array.Copy(Content, loc + 1, Content, loc, Cardinality - loc - 1);
                --Cardinality;
            }
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
            int indexstart = Utility.UnsignedBinarySearch(Content, 0, Cardinality, begin);

            if (indexstart < 0)
            {
                indexstart = -indexstart - 1;
            }

            int indexend = Utility.UnsignedBinarySearch(Content, 0, Cardinality, (ushort)(end - 1));

            if (indexend < 0)
            {
                indexend = -indexend - 1;
            }
            else
            {
                indexend++;
            }

            int rangelength = indexend - indexstart;
            Array.Copy(Content, indexstart + rangelength, Content, indexstart, Cardinality - indexstart - rangelength);
            Cardinality -= rangelength;
            return this;
        }

        /// <summary>
        /// Return the jth value of the container.
        /// </summary>
        /// <param name="j">Index of the value </param>
        /// <returns>The jth value of the container</returns>
        public override ushort Select(int j)
        {
            return this.Content[j];
        }

        public override bool Equals(Object o)
        {
            if (!(o is ArrayContainer))
            {
                return false;
            }

            ArrayContainer srb = (ArrayContainer) o;
            if (srb.Cardinality != this.Cardinality)
            {
                return false;
            }
            for (int i = 0; i < this.Cardinality; ++i)
            {
                if (this.Content[i] != srb.Content[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Cardinality;
                for (int i = 0; i < Cardinality; i++)
                {
                    hash = 17 * hash + Content[i];
                }
                return hash;
            }

        }

        /// <summary>
        /// Serialize this container in a binary format.
        /// </summary>
        /// <param name="writer">The writer to which to serialize this container.</param>
        /// <remarks>The serialization format is first the cardinality of the container as 
        /// a 32-bit integer, followed by an array of the indices in this container as 16-bit integers.</remarks>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Cardinality);
            for (int i = 0; i < Cardinality; i++)
            {
                writer.Write(Content[i]);
            }
        }

        /// <summary>
        /// Deserialize a container from binary format, as written by the 
        /// Serialize method, minus the first 32 bits giving the cardinality.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The first container represented by reader.</returns>
        public static ArrayContainer Deserialize(BinaryReader reader, int cardinality)
        {
            ArrayContainer container = new ArrayContainer(cardinality);

            container.Cardinality = cardinality;
            for(int i = 0; i < cardinality; i++)
            {
                container.Content[i] = (ushort) reader.ReadInt16();
            }

            return container;
        }

        public override IEnumerator<ushort> GetEnumerator()
        {
            int index = 0;

            while (index < Cardinality)
            {
                yield return Content[index];
                index++;
            }
        }
    }
}

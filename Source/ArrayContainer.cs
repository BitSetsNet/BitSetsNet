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
        public const int DEFAULT_MAX_SIZE = 4096;

        public int cardinality;
        public ushort[] content;

        public ArrayContainer() : this(DEFAULT_INIT_SIZE) {}
        
        public ArrayContainer(int capacity)
        {
            this.cardinality = 0;
            this.content = new ushort[capacity];
        }

        private ArrayContainer(int cardinality, ushort[] inpContent)
        {
            this.cardinality = cardinality;
            this.content = inpContent;
        }

        public ArrayContainer(ushort[] newContent)
        {
            this.cardinality = newContent.Length;
            this.content = newContent;
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

        
        public override Container flip(ushort x)
        {
            int loc = Utility.unsignedBinarySearch(content, 0, cardinality, x);
            if (loc < 0)
            {
                // Transform the ArrayContainer to a BitmapContainer
                // when cardinality = DEFAULT_MAX_SIZE
                if (cardinality >= DEFAULT_MAX_SIZE)
                {
                    BitsetContainer a = this.toBitsetContainer();
                    a.add(x);
                    return a;
                }
                if (cardinality >= this.content.Length)
                {
                    increaseCapacity();
                }
                // insertion : shift the elements > x by one position to
                // the right
                // and put x in it's appropriate place
                Array.Copy(content, -loc - 1, content, -loc, cardinality + loc + 1);
                content[-loc - 1] = x;
                ++cardinality;
            }
            else
            {
                Array.Copy(content, loc + 1, content, loc, cardinality - loc - 1);
                --cardinality;
            }
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
            int indexstart = Utility.unsignedBinarySearch(content, 0, cardinality, begin);
            if (indexstart < 0)
            {
                indexstart = -indexstart - 1;
            }
            int indexend = Utility.unsignedBinarySearch(content, 0, cardinality, (ushort)(end - 1));
            if (indexend < 0)
            {
                indexend = -indexend - 1;
            }
            else
            {
                indexend++;
            }
            int rangelength = end - begin;
            int newcardinality = indexstart + (cardinality - indexend) + rangelength;
            if (newcardinality > DEFAULT_MAX_SIZE)
            {
                BitsetContainer a = this.toBitsetContainer();
                return a.iadd(begin, end);
            }
            if (newcardinality >= this.content.Length)
            {
                increaseCapacity(newcardinality);
            }
            Array.Copy(content, indexend, content, indexstart + rangelength, cardinality - indexend);
            for (int k = 0; k < rangelength; ++k)
            {
                content[k + indexstart] = (ushort)(begin + k);
            }
            cardinality = newcardinality;
            return this;
        }
        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other ArrayContainer.
        /// </summary>
        /// <param name="x">the other ArrayContainer</param>
        /// <returns>A new container with the differences</returns>
        public override Container andNot (ArrayContainer x)
        {
            int desiredCapacity = this.getCardinality();
            var answer = new ArrayContainer(desiredCapacity);

            // Compute the cardinality of the new container
            answer.cardinality = Utility.unsignedDifference(this.content,
                                                            desiredCapacity,
                                                            x.content,
                                                            x.getCardinality(),
                                                            answer.content);
            return answer;

        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other BitSetContainer.
        /// </summary>
        /// <param name="x">the BitSetContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container andNot (BitsetContainer x)
        {
            var answer = new ArrayContainer(content.Length);
            int pos = 0;
            for (int k = 0; k < cardinality; ++k)
            {
                ushort val = this.content[k];
                if (!x.contains(val))
                    answer.content[pos++] = val;
            }
            answer.cardinality = pos;
            return answer;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other BitSetContainer. Modifies the current container in place.
        /// </summary>
        /// <param name="x">the BitSetContainer to compare against</param>
        /// <returns>A new container with the differences</returns>
        public override Container iandNot(BitsetContainer x)
        {
            int pos = 0;
            for (int k = 0; k < cardinality; ++k)
            {
                ushort v = this.content[k];
                if (!x.contains(v))
                    this.content[pos++] = v;
            }
            this.cardinality = pos;
            return this;
        }

        /// <summary>
        /// Returns the elements of this ArrayContainer that are not in the
        /// other ArrayContainer.
        /// </summary>
        /// <param name="x">the other ArrayContainer</param>
        /// <returns>The modified container</returns>
        public override Container iandNot(ArrayContainer x)
        {
            this.cardinality = Utility.unsignedDifference(this.content,
                this.getCardinality(), x.content,
                x.getCardinality(), this.content);
            return this;
        }

        public override Container inot(int firstOfRange, int lastOfRange)
        {
            // TODO: may need to convert to a RunContainer
            // determine the span of array indices to be affected
            int startIndex = Utility.unsignedBinarySearch(content, 0, cardinality, (ushort)firstOfRange);
            if (startIndex < 0)
            {
                startIndex = -startIndex - 1;
            }
            int lastIndex = Utility.unsignedBinarySearch(content, 0, cardinality, (ushort)(lastOfRange - 1));
            if (lastIndex < 0)
            {
                lastIndex = -lastIndex - 1 - 1;
            }
            int currentValuesInRange = lastIndex - startIndex + 1;
            int spanToBeFlipped = lastOfRange - firstOfRange;
            int newValuesInRange = spanToBeFlipped - currentValuesInRange;
            ushort[] buffer = new ushort[newValuesInRange];
            int cardinalityChange = newValuesInRange - currentValuesInRange;
            int newCardinality = cardinality + cardinalityChange;

            if (cardinalityChange > 0)
            { // expansion, right shifting needed
                if (newCardinality > content.Length)
                {
                    // so big we need a bitmap?
                    if (newCardinality > DEFAULT_MAX_SIZE)
                    {
                        return toBitsetContainer().inot(firstOfRange, lastOfRange);
                    }
                    // Change the size of the array based on the new cardinality
                    Array.Resize(ref content, newCardinality);
                }
                // slide right the contents after the range
                Array.Copy(content, lastIndex + 1, content, lastIndex + 1 + cardinalityChange,
                    cardinality - 1 - lastIndex);
                negateRange(buffer, startIndex, lastIndex, firstOfRange, lastOfRange);
            }
            else
            { // no expansion needed
                negateRange(buffer, startIndex, lastIndex, firstOfRange, lastOfRange);
                if (cardinalityChange < 0)
                {
                    // contraction, left sliding.
                    // Leave array oversize
                    Array.Copy(content, startIndex + newValuesInRange - cardinalityChange, content,
                        startIndex + newValuesInRange, newCardinality - (startIndex + newValuesInRange));
                }
            }
            cardinality = newCardinality;
            return this;
        }

        // for use in inot range known to be nonempty
        private void negateRange(ushort[] buffer, int startIndex, int lastIndex,
            int startRange, int lastRange)
        {
            // compute the negation into buffer

            int outPos = 0;
            int inPos = startIndex; // value here always >= valInRange,
                                    // until it is exhausted
                                    // n.b., we can start initially exhausted.

            int valInRange = startRange;
            for (; valInRange < lastRange && inPos <= lastIndex; ++valInRange)
            {
                if ((short)valInRange != content[inPos])
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
                throw new SystemException(
                    "negateRange: outPos " + outPos + " whereas buffer.length=" + buffer.Length);
            }
            // copy back from buffer...caller must ensure there is room
            int i = startIndex;
            foreach (ushort item in buffer)
            {
                content[i++] = item;
            }
        }

        public override Container add(ushort begin, ushort end)
        {
            int indexstart = 
                Utility.unsignedBinarySearch(content, 0, cardinality, begin);
            if (indexstart < 0)
                indexstart = -indexstart - 1;
            int indexend = 
                Utility.unsignedBinarySearch(content, 0, cardinality, (ushort)(end - 1));

            if (indexend < 0)
                indexend = -indexend - 1;
            else
                indexend++;

            int rangelength = end - begin;
            int newcardinality = 
                indexstart + (cardinality - indexend) + rangelength;

            if (newcardinality > DEFAULT_MAX_SIZE)
            {
                BitsetContainer a = this.toBitsetContainer();
                return a.add(begin, end);
            }

            if (newcardinality >= this.content.Length)
                increaseCapacity(newcardinality);

            Array.Copy(content, indexend, this.content, indexstart
                    + rangelength, cardinality - indexend);

            for (int k = 0; k < rangelength; ++k)
            {
                this.content[k + indexstart] = (ushort)(begin + k);
            }

            this.cardinality = newcardinality;

            return this;
        }

        private void increaseCapacity(int min)
        {
            int newCapacity = (this.content.Length == 0) ? DEFAULT_INIT_SIZE : this.content.Length < 64 ? this.content.Length * 2
                    : this.content.Length < 1024 ? this.content.Length * 3 / 2
                    : this.content.Length * 5 / 4;
            if (newCapacity < min) newCapacity = min;
            // never allocate more than we will ever need
            if (newCapacity > ArrayContainer.DEFAULT_MAX_SIZE)
                newCapacity = ArrayContainer.DEFAULT_MAX_SIZE;
            // if we are within 1/16th of the max, go to max 
            if (newCapacity < ArrayContainer.DEFAULT_MAX_SIZE - ArrayContainer.DEFAULT_MAX_SIZE / 16)
                newCapacity = ArrayContainer.DEFAULT_MAX_SIZE;
            Array.Resize(ref this.content, newCapacity);
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

        public void loadData(BitsetContainer bitsetContainer)
        {
            this.cardinality = bitsetContainer.cardinality;
            bitsetContainer.fillArray(content);
        }

        public override Container and(BitsetContainer x)
        {
            return x.and(this);
        }

        public override Container and(ArrayContainer value2)
        {
            ArrayContainer value1 = this;
            int desiredCapacity = Math.Min(value1.getCardinality(), value2.getCardinality());
            ArrayContainer answer = new ArrayContainer(desiredCapacity);
            answer.cardinality = Utility.unsignedIntersect2by2(value1.content,
                    value1.getCardinality(), value2.content,
                    value2.getCardinality(), answer.content);
            return answer;
        }

        public override Container clone()
        {
            ushort[] newContent = new ushort[this.content.Length];
            this.content.CopyTo(newContent, 0);
            return new ArrayContainer(this.cardinality, newContent);
        }

        public override bool contains(ushort x)
        {
            return Utility.unsignedBinarySearch(content, 0, cardinality, x) >= 0;
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
        /// Performs an in-place intersection with a BitsetContainer.
        /// </summary>
        /// <param name="other">the BitsetContainer to intersect</param>
        public override Container iand(BitsetContainer other)
        {
            int pos = 0;
            for (int k = 0; k < cardinality; k++)
            {
                ushort v = content[k];
                if (other.contains(v))
                    content[pos++] = v;
            }
            cardinality = pos;
            return this;
        }

        /// <summary>
        /// Performs an in-place intersection with another ArrayContainer.
        /// </summary>
        /// <param name="other">the other ArrayContainer to intersect</param>
        public override Container iand(ArrayContainer other)
        {
            cardinality = Utility.unsignedIntersect2by2(content,
                getCardinality(), other.content,
                other.getCardinality(), content);
            return this;
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
            return x.or(this);
        }

        public override Container ior(ArrayContainer x)
        {
            return this.or(x);
        }

        public override Container or(BitsetContainer x)
        {
            return x.or((ArrayContainer) this);
        }

        public override Container or(ArrayContainer value2)
        {
            ArrayContainer value1 = this;
            int totalCardinality = value1.getCardinality() + value2.getCardinality();
            if (totalCardinality > DEFAULT_MAX_SIZE) {
                // it could be a bitmap!
                BitsetContainer bc = new BitsetContainer();
                for (int k = 0; k < value2.cardinality; ++k)
                {
                    ushort v = value2.content[k];
                    int i = v >> 6;
                    bc.bitmap[i] |= (1L << v);
                }
                for (int k = 0; k < this.cardinality; ++k)
                {
                    ushort v = this.content[k];
                    int i = v >> 6;
                    bc.bitmap[i] |= (1L << v);
                }
                bc.cardinality = 0;
                foreach (long k in bc.bitmap)
                {
                    bc.cardinality += Utility.longBitCount(k);
                }
                if (bc.cardinality <= DEFAULT_MAX_SIZE)
                    return bc.toArrayContainer();
                return bc;
            } else {
                // remains an array container
                int desiredCapacity = totalCardinality; // Math.min(BitmapContainer.MAX_CAPACITY,
                                                        // totalCardinality);
                ArrayContainer answer = new ArrayContainer(desiredCapacity);
                answer.cardinality = Utility.unsignedUnion2by2(value1.content,
                        value1.getCardinality(), value2.content,
                        value2.getCardinality(), answer.content);
                return answer;
            }
        }

        public override Container remove(ushort x)
        {
            int loc = Utility.unsignedBinarySearch(content, 0, cardinality, x);
            if (loc >= 0)
            {
                // insertion
                Array.Copy(content, loc + 1, content, loc, cardinality - loc - 1);
                --cardinality;
            }
            return this;
        }

        public override Container remove(ushort begin, ushort end)
        {
            int indexstart = 
                Utility.unsignedBinarySearch(content, 0, cardinality, begin);

            if (indexstart < 0)
                indexstart = -indexstart - 1;

            int indexend = 
                Utility.unsignedBinarySearch(content, 0, cardinality, (ushort)(end - 1));

            if (indexend < 0)
                indexend = -indexend - 1;
            else
                indexend++;

            int rangelength = indexend - indexstart;
            Array.Copy(content, indexstart + rangelength, content, indexstart,
                    cardinality - indexstart - rangelength);
            cardinality -= rangelength;
            return this;
        }

        public override ushort select(int j)
        {
            return this.content[j];
        }

        public override bool Equals(Object o) {
            if (o is ArrayContainer) {
                ArrayContainer srb = (ArrayContainer) o;
                if (srb.cardinality != this.cardinality)
                    return false;
                for (int i = 0; i < this.cardinality; ++i) {
                    if (this.content[i] != srb.content[i])
                        return false;
                }
                return true;
            } 
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = cardinality;
                for (int i = 0; i < cardinality; i++)
                {
                    hash = 17 * hash + content[i];
                }
                return hash;
            }

        }

        /// <summary>
        /// Serialize this container in a binary format.
        /// </summary>
        /// <param name="writer">The writer to which to serialize this container.</param>
        /// <remarks>The serialization format is first the cardinality of the container as a 32-bit integer, followed by an array of the indices in this container as 16-bit integers.</remarks>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(cardinality);
            foreach(ushort index in content)
            {
                writer.Write(index);
            }
        }

        /// <summary>
        /// Deserialize a container from binary format, as written by the Serialize method, minus the first 32 bits giving the cardinality.
        /// </summary>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The first container represented by reader.</returns>
        public static ArrayContainer Deserialize(BinaryReader reader, int cardinality)
        {
            ArrayContainer container = new ArrayContainer(cardinality);

            container.cardinality = cardinality;
            for(int i = 0; i < cardinality; i++)
            {
                container.content[i] = (ushort) reader.ReadInt16();
            }

            return container;
        }

        public override IEnumerator<ushort> GetEnumerator()
        {
            int index = 0;

            while (index < cardinality) {
                yield return content[index];
                index++;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BitsetsNET
{
    public class RoaringBitset : IBitset
    {

        RoaringArray containers = new RoaringArray();

        public static RoaringBitset Create(int[] input)
        {
            RoaringBitset rb = new RoaringBitset();
            foreach (int i in input) {
                rb.add(i);
            }
            return rb;
        }

        public void add(int x)
        {
            ushort highBits = Utility.GetHighBits(x);
            int containerIndex = containers.getIndex(highBits);

            if (containerIndex >= 0)
                // a container exists at this index already.
                // find the right container, get the low order bits to add to the container and add them
            {
                containers.setContainerAtIndex(containerIndex, containers.getContainerAtIndex(containerIndex).add(Utility.GetLowBits(x))
                );
            }
            else
            {
                // no container exists for this index
                // create a new ArrayContainer, since it will only hold one integer to start
                // get the low order bits and att to the newly created container
                // add the newly created container to the array of containers
                ArrayContainer newac = new ArrayContainer();
                containers.insertNewKeyValueAt(-containerIndex - 1, highBits, newac.add(Utility.GetLowBits(x)));
            }

        }

        /// <summary>
        /// Add to the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        public void add(int rangeStart, int rangeEnd)
        {
            if (rangeStart >= rangeEnd)
                return; // empty range

            ushort hbStart = Utility.GetHighBits(rangeStart);
            ushort lbStart = Utility.GetLowBits(rangeStart);
            ushort hbLast = Utility.GetHighBits(rangeEnd - 1);
            ushort lbLast = Utility.GetLowBits(rangeEnd - 1);

            for (ushort hb = hbStart; hb <= hbLast; ++hb)
            {

                // first container may contain partial range
                ushort containerStart = 0;
                if (hb == hbStart) { containerStart = lbStart; }

                // last container may contain partial range
                ushort containerLast = (hb == hbLast) ? lbLast : ushort.MaxValue;
                int containerIndex = containers.getIndex(hb);

                if (containerIndex >= 0)
                {
                    Container c = containers.getContainerAtIndex(containerIndex).add(
                                   containerStart, (ushort)(containerLast + 1));
                    containers.setContainerAtIndex(containerIndex, c);
                }
                else {
                    Container newContainer = new ArrayContainer(100);
                    newContainer = newContainer.add(lbStart, lbLast);
                    containers.insertNewKeyValueAt(-containerIndex - 1, hb, newContainer);
                }
            }
        }

        /// <summary>
        /// Remove from the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        public void remove(int rangeStart, int rangeEnd)
        {
            if (rangeStart >= rangeEnd)
                return; // empty range

            ushort hbStart = Utility.GetHighBits(rangeStart);
            ushort lbStart = Utility.GetLowBits(rangeStart);
            ushort hbLast = Utility.GetHighBits(rangeEnd - 1);
            ushort lbLast = Utility.GetLowBits(rangeEnd - 1);

            if (hbStart == hbLast)
            {
                int containerIndex = containers.getIndex(hbStart);

                if (containerIndex < 0) return;

                Container c = containers.getContainerAtIndex(containerIndex).remove(
                        lbStart, (ushort)(lbLast + 1));

                if (c.getCardinality() > 0)
                    containers.setContainerAtIndex(containerIndex, c);
                else
                    containers.removeAtIndex(containerIndex);
                return;
            }

            int ifirst = containers.getIndex(hbStart);
            int ilast = containers.getIndex(hbLast);

            if (ifirst >= 0)
            {
                if (lbStart != 0)
                {
                    Container c = containers.getContainerAtIndex(ifirst).remove(
                             lbStart, ushort.MaxValue);

                    if (c.getCardinality() > 0)
                    {
                        containers.setContainerAtIndex(ifirst, c);
                        ifirst++;
                    }
                }
            }
            else {
                ifirst = -ifirst - 1;
            }

            if (ilast >= 0)
            {
                if (lbLast != ushort.MaxValue)
                {
                    Container c = containers.getContainerAtIndex(ilast).remove(
                            0, (ushort)(lbLast + 1));

                    if (c.getCardinality() > 0)
                    {
                        containers.setContainerAtIndex(ilast, c);
                    }
                    else ilast++;
                }
                else ilast++;
            }
            else {
                ilast = -ilast - 1;
            }

            containers.removeIndexRange(ifirst, ilast);
        }

        public static RoaringBitset and(RoaringBitset x1,
                                        RoaringBitset x2)
        {
            RoaringBitset answer = new RoaringBitset();
            int length1 = x1.containers.size, length2 = x2.containers.size;
            int pos1 = 0, pos2 = 0;

            while (pos1 < length1 && pos2 < length2) {
                ushort s1 = x1.containers.getKeyAtIndex(pos1);
                ushort s2 = x2.containers.getKeyAtIndex(pos2);

                if (s1 == s2) {
                    Container c1 = x1.containers.getContainerAtIndex(pos1);
                    Container c2 = x2.containers.getContainerAtIndex(pos2);
                    Container c = c1.and(c2);

                    if (c.getCardinality() > 0) {
                        answer.containers.append(s1, c);
                    }

                    ++pos1;
                    ++pos2;
                } else if (s1 < s2 ) { // s1 < s2
                    pos1 = x1.containers.advanceUntil(s2, pos1);
                } else { // s1 > s2
                    pos2 = x2.containers.advanceUntil(s1, pos2);
                }
            }
            return answer;
        }

        /// <summary>
        /// Performs an in-place intersection of two Roaring Bitsets.
        /// </summary>
        /// <param name="other">the second Roaring Bitset to intersect</param>
        private void andWith(RoaringBitset other)
        {
            int length1 = this.containers.size, length2 = other.containers.size;
            int pos1 = 0, pos2 = 0, intersectionSize = 0;

            while (pos1 < length1 && pos2 < length2)
            {
                ushort s1 = this.containers.getKeyAtIndex(pos1);
                ushort s2 = other.containers.getKeyAtIndex(pos2);

                if (s1 == s2)
                {
                    Container c1 = this.containers.getContainerAtIndex(pos1);
                    Container c2 = other.containers.getContainerAtIndex(pos2);
                    Container c = c1.iand(c2);

                    if (c.getCardinality() > 0)
                    {
                        this.containers.replaceKeyAndContainerAtIndex(intersectionSize++, s1, c);
                    }
                        
                    ++pos1;
                    ++pos2;
                }
                else if (s1 < s2)
                { // s1 < s2
                    pos1 = this.containers.advanceUntil(s2, pos1);
                }
                else
                { // s1 > s2
                    pos2 = other.containers.advanceUntil(s1, pos2);
                }
            }
            this.containers.resize(intersectionSize);
        }

        public override bool Equals(Object o)
        {
            if (o is RoaringBitset) {
                RoaringBitset srb = (RoaringBitset) o;
                return srb.containers.Equals(this.containers);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return containers.GetHashCode();
        }

        public int select(int j)
        {
            int leftover = j;
            for (int i = 0; i < this.containers.size; i++)
            {
                Container c = this.containers.getContainerAtIndex(i);
                int thiscard = c.getCardinality();
                if (thiscard > leftover)
                {
                    uint keycontrib = (uint) this.containers.getKeyAtIndex(i) << 16;
                    uint lowcontrib = (uint) c.select(leftover);
                    return (int) (lowcontrib + keycontrib);
                }
                leftover -= thiscard;
            }
            throw new ArgumentOutOfRangeException("select " + j + " when the cardinality is " + this.Cardinality());
        }
        
        public IBitset And(IBitset otherSet)
        {
            if (otherSet is RoaringBitset)
            {
                return and(this, (RoaringBitset)otherSet);
            }
            throw new ArgumentOutOfRangeException("otherSet must be a RoaringBitset");
        }

        /// <summary>
        /// Performs an in-place intersection of two Roaring Bitsets.
        /// </summary>
        /// <param name="otherSet">the second Roaring Bitset to intersect</param>
        public void AndWith(IBitset otherSet)
        {
            if (otherSet is RoaringBitset)
            {
                andWith((RoaringBitset)otherSet);
            }
            else
            {
                throw new ArgumentOutOfRangeException("otherSet must be a RoaringBitset");
            }
        }

        public IBitset Clone()
        {
            RoaringBitset x = new RoaringBitset();
            x.containers = containers.clone();
            return x;
        }

        public IBitset Or(IBitset otherSet)
        {
            if (!(otherSet is RoaringBitset))
                throw new ArgumentOutOfRangeException("otherSet must be a RoaringBitSet");
            
            RoaringBitset answer = new RoaringBitset();
            RoaringBitset x2 = (RoaringBitset) otherSet;

            int pos1 = 0, pos2 = 0;
            int length1 = this.containers.size, length2 = x2.containers.size;

            if (pos1 < length1 && pos2 < length2) {
                ushort s1 = this.containers.getKeyAtIndex(pos1);
                ushort s2 = x2.containers.getKeyAtIndex(pos2);

                while (true) {
                    if (s1 == s2) {
                        answer.containers.append(
                            s1, 
                            this.containers.getContainerAtIndex(pos1).or(
                                x2.containers.getContainerAtIndex(pos2)
                            )
                        );
                        pos1++;
                        pos2++;
                        if ((pos1 == length1) || (pos2 == length2)) {
                            break;
                        }
                        s1 = this.containers.getKeyAtIndex(pos1);
                        s2 = x2.containers.getKeyAtIndex(pos2);
                    } else if (s1 < s2) {
                        answer.containers.appendCopy(this.containers, pos1);
                        pos1++;
                        if (pos1 == length1) {
                            break;
                        }
                        s1 = this.containers.getKeyAtIndex(pos1);
                    } else { // s1 > s2
                        answer.containers.appendCopy(x2.containers, pos2);
                        pos2++;
                        if (pos2 == length2) {
                            break;
                        }
                        s2 = x2.containers.getKeyAtIndex(pos2);
                    }
                }
            }

            if (pos1 == length1) {
                answer.containers.appendCopy(x2.containers, pos2, length2);
            } else if (pos2 == length2) {
                answer.containers.appendCopy(this.containers, pos1, length1);
            }

            return answer;
        }

        public void OrWith(IBitset otherSet)
        {
            if (!(otherSet is RoaringBitset))
                throw new ArgumentOutOfRangeException("otherSet must be a RoaringBitSet");


            RoaringBitset x2 = (RoaringBitset)otherSet;

            int pos1 = 0, pos2 = 0;
            int length1 = this.containers.size, length2 = x2.containers.size;

            if (pos1 < length1 && pos2 < length2)
            {
                ushort s1 = this.containers.getKeyAtIndex(pos1);
                ushort s2 = x2.containers.getKeyAtIndex(pos2);

                while (true)
                {
                    if (s1 == s2)
                    {
                        this.containers.setContainerAtIndex(
                            pos1,
                            this.containers.getContainerAtIndex(pos1).ior(
                                x2.containers.getContainerAtIndex(pos2)
                            )
                        );
                        pos1++;
                        pos2++;
                        if ((pos1 == length1) || (pos2 == length2))
                        {
                            break;
                        }
                        s1 = this.containers.getKeyAtIndex(pos1);
                        s2 = x2.containers.getKeyAtIndex(pos2);
                    }
                    else if (s1 < s2)
                    {
                        pos1++;
                        if (pos1 == length1)
                        {
                            break;
                        }
                        s1 = this.containers.getKeyAtIndex(pos1);
                    }
                    else
                    { // s1 > s2
                        this.containers.insertNewKeyValueAt(pos1, s2, x2.containers.getContainerAtIndex(pos2));
                        pos1++;
                        length1++;
                        pos2++;
                        if (pos2 == length2)
                        {
                            break;
                        }
                        s2 = x2.containers.getKeyAtIndex(pos2);
                    }
                }
            }

            if (pos1 == length1)
            {
                this.containers.appendCopy(x2.containers, pos2, length2);
            } 
        }

        public bool Get(int index)
        {
            ushort highBits = Utility.GetHighBits(index);
            int containerIndex = containers.getIndex(highBits);

            // a container exists at this index already.
            // find the right container, get the low order bits to add to the 
            // container and add them
            if (containerIndex >= 0)
            {
                return containers.getContainerAtIndex(containerIndex).contains(
                    Utility.GetLowBits(index));
            }
            else
            {
                // no container exists for this index
                return false;
            }
        }
        
        /// <summary>
        /// Adds the current index to the set if value is true, otherwise 
        /// removes it if the set contains it.
        /// </summary>
        /// <param name="index">
        /// the index to set
        /// </param>
        /// <param name="value">
        /// boolean of whether to add or remove the index
        /// </param>
        public void Set(int index, bool value)
        {
            if (value)
            {
                add(index);
            } else { 
                ushort hb = Utility.GetHighBits(index);
                int containerIndex = containers.getIndex(hb);

                if (containerIndex > -1)
                {
                    Container updatedContainer = 
                        containers.getContainerAtIndex(containerIndex).remove(
                            Utility.GetLowBits(index)
                        );
                    containers.setContainerAtIndex(containerIndex, updatedContainer);
                }
            }
        }

        public void Set(int start, int end, bool value)
        {
            if (value)
            {
                add(start, end);
            } else
            {
                remove(start, end);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public int Cardinality()
        {
            int size = 0;
            for (int i = 0; i < this.containers.size; i++)
            {
                size += this.containers.getContainerAtIndex(i).getCardinality();
            }
            return size;
        }

        /// <summary>
        /// Write a binary serialization of this roaring bitset.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void Serialize(Stream stream)
        {
            //We don't care about the encoding, but we have to specify something to be able to set the stream as leave open.
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Default, true))
            {
                containers.Serialize(writer);
            }
        }

        /// <summary>
        /// Read a binary serialization of a roaring bitset, as written by the Serialize method.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The bitset deserialized from the stream.</returns>
        public static RoaringBitset Deserialize(Stream stream)
        {
            RoaringBitset bitset = new RoaringBitset();

            //We don't care about the encoding, but we have to specify something to be able to set the stream as leave open.
            using (BinaryReader reader = new BinaryReader(stream, Encoding.Default, true))
            {
                bitset.containers = RoaringArray.Deserialize(reader);
            }

            return bitset;
        }

        /// <summary>
        /// Get an enumerator of the set indices of this bitset.
        /// </summary>
        /// <returns>A enumerator giving the set (i.e. for which the bit is '1' or true) indices for this bitset.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return containers.GetEnumerator();
        }

        public void Flip(int x)
        {
            ushort hb = Utility.GetHighBits(x);
            int i = containers.getIndex(hb);

            if (i >= 0)
            {
                Container c = containers.getContainerAtIndex(i).flip(Utility.GetLowBits(x));
                if (c.getCardinality() > 0)
                {
                    containers.setContainerAtIndex(i, c);
                }
                else
                {
                    containers.removeAtIndex(i);
                }
            }
            else
            {
                ArrayContainer newac = new ArrayContainer();
                containers.insertNewKeyValueAt(-i - 1, hb, newac.add(Utility.GetLowBits(x)));
            }
        }

        /// <summary>
        /// Toggles the values for the given range of indices.
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="end">The ending index</param>
        public void Flip(int start, int end)
        {
            if (start >= end)
            {
                return; // empty range
            }

            // Separate out the ranges of higher and lower-order bits
            int hbStart = Utility.toIntUnsigned(Utility.GetHighBits(start));
            int lbStart = Utility.toIntUnsigned(Utility.GetLowBits(start));
            int hbLast = Utility.toIntUnsigned(Utility.GetHighBits(end - 1));
            int lbLast = Utility.toIntUnsigned(Utility.GetLowBits(end - 1));

            for (int hb = hbStart; hb <= hbLast; hb++)
            {
                // first container may contain partial range
                int containerStart = (hb == hbStart) ? lbStart : 0;
                // last container may contain partial range
                int containerLast = (hb == hbLast) ? lbLast : Utility.GetMaxLowBitAsInteger();
                int i = containers.getIndex((ushort)hb);

                if (i >= 0)
                {
                    Container c = containers.getContainerAtIndex(i).inot(containerStart, containerLast + 1);
                    if (c.getCardinality() > 0)
                    {
                        containers.setContainerAtIndex(i, c);
                    }
                    else
                    {
                        containers.removeAtIndex(i);
                    }
                }
                else
                {
                    containers.insertNewKeyValueAt(-i - 1, (ushort)hb,
                        Container.rangeOfOnes((ushort) containerStart, (ushort) (containerLast + 1)));
                }
            }
        }

        /// <summary>
        /// Finds members of a bitset that are not in the other set (ANDNOT).
        /// This does not modify either bitset.
        /// </summary>
        /// <param name="otherSet">The set to compare against</param>
        /// <returns>A new IBitset containing the members that are in
        /// the first bitset but not in the second.</returns>
        public IBitset andNot(RoaringBitset otherSet)
        {

            RoaringBitset answer = new RoaringBitset();
            int pos1 = 0, pos2 = 0;
            int length1 = containers.size, length2 = otherSet.containers.size;

            while (pos1 < length1 && pos2 < length2)
            {
                ushort s1 = containers.getKeyAtIndex(pos1);
                ushort s2 = otherSet.containers.getKeyAtIndex(pos2);
                if (s1 == s2)
                {
                    Container c1 = containers.getContainerAtIndex(pos1);
                    Container c2 = otherSet.containers.getContainerAtIndex(pos2);
                    Container c = c1.andNot(c2);
                    if (c.getCardinality() > 0)
                    {
                        answer.containers.append(s1, c);
                    }
                    ++pos1;
                    ++pos2;
                }
                else if (Utility.compareUnsigned(s1, s2) < 0)
                { // s1 < s2
                    int nextPos1 = containers.advanceUntil(s2, pos1);
                    answer.containers.appendCopy(containers, pos1, nextPos1);
                    pos1 = nextPos1;
                }
                else
                { // s1 > s2
                    pos2 = otherSet.containers.advanceUntil(s1, pos2);
                }
            }
            if (pos2 == length2)
            {
                answer.containers.appendCopy(containers, pos1, length1);
            }
            return answer;
        }

        /// <summary>
        /// Finds members of this bitset that are not in the other set (ANDNOT).
        /// Modifies current bitset in place.
        /// </summary>
        /// <param name="otherSet">The set to compare against</param>
        public void iandNot(RoaringBitset otherSet)
        {
            int pos1 = 0, pos2 = 0, intersectionSize = 0;
            int length1 = containers.size, length2 = otherSet.containers.size;

            while (pos1 < length1 && pos2 < length2)
            {
                ushort s1 = containers.getKeyAtIndex(pos1);
                ushort s2 = otherSet.containers.getKeyAtIndex(pos2);
                if (s1 == s2)
                {
                    Container c1 = containers.getContainerAtIndex(pos1);
                    Container c2 = otherSet.containers.getContainerAtIndex(pos2);
                    Container c = c1.iandNot(c2);
                    if (c.getCardinality() > 0)
                    {
                        containers.replaceKeyAndContainerAtIndex(intersectionSize++, s1, c);
                    }
                    ++pos1;
                    ++pos2;
                }
                else if (Utility.compareUnsigned(s1, s2) < 0)
                { // s1 < s2
                    if (pos1 != intersectionSize)
                    {
                        Container c1 = containers.getContainerAtIndex(pos1);
                        containers.replaceKeyAndContainerAtIndex(intersectionSize, s1, c1);
                    }
                    ++intersectionSize;
                    ++pos1;
                }
                else
                { // s1 > s2
                    pos2 = otherSet.containers.advanceUntil(s1, pos2);
                }
            }
            if (pos1 < length1)
            {
                containers.copyRange(pos1, length1, intersectionSize);
                intersectionSize += length1 - pos1;
            }
            containers.resize(intersectionSize);
        }

        /// <summary>
        /// Finds members of this bitset that are not in the other set (ANDNOT).
        /// This does not modify either bitset.
        /// </summary>
        /// <param name="otherSet">The set to compare against</param>
        /// <returns>A new IBitset containing the members that are in
        /// this bitset but not in the other.</returns>
        public IBitset Difference(IBitset otherSet)
        {
            if (otherSet is RoaringBitset)
            {
                return this.andNot((RoaringBitset) otherSet);
            }
            throw new ArgumentOutOfRangeException("Other set must be a roaring bitset");      
        }

        /// <summary>
        /// Finds members of this bitset that are not in the other set (ANDNOT).
        /// Places the results in the current bitset (modifies in place).
        /// </summary>
        /// <param name="otherSet">The set to compare against</param>
        /// <returns>A new IBitset containing the members that are in
        /// this bitset but not in the other.</returns>
        public void DifferenceWith(IBitset otherSet)
        {
            if (otherSet is RoaringBitset)
            {
                this.iandNot((RoaringBitset)otherSet);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Other set must be a roaring bitset");
            }
            
        }

        public BitArray ToBitArray()
        {
            throw new NotImplementedException();
        }
    }
}

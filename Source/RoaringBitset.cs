using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BitsetsNET
{
    public class RoaringBitset : IBitset
    {

        RoaringArray containers = new RoaringArray();

        public static RoaringBitset Crte(int[] input)
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

        public static RoaringBitset and(RoaringBitset x1,
                                        RoaringBitset x2) {
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

        public override bool Equals(Object o) {
            if (o is RoaringBitset) {
                RoaringBitset srb = (RoaringBitset) o;
                return srb.containers.Equals(this.containers);
            }
            return false;
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
            throw new ArgumentOutOfRangeException("select " + j + " when the cardinality is " + this.getCardinality());
        }

        public int getCardinality()
        {
            int size = 0;
            for (int i = 0; i < this.containers.size; i++)
            {
                size += this.containers.getContainerAtIndex(i).getCardinality();
            }
            return size;
        }

        public IBitset And(IBitset otherSet)
        {
            if (otherSet is RoaringBitset) {
                return and(this, (RoaringBitset)otherSet);
            }
            throw new ArgumentOutOfRangeException("otherSet must be a RoaringBitset");
        }

        public void AndWith(IBitset otherSet)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public int Length()
        {
            return getCardinality();
        }

        public void Set(int index, bool value)
        {
            add(index);
        }

        public void SetAll(bool value)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Cardinality()
        {
            return 0;
        }

        public IBitset Not()
        {
            throw new NotImplementedException();
        }

    }
}

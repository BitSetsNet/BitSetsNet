using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    public class RoaringBitset
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
    }
}

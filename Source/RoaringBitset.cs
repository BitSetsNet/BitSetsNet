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
    }
}

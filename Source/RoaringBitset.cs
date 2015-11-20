using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class RoaringBitset
    {

        ContainerArray containers = null;

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
            short highBits = Utility.GetHighBits(x);
            int i = containers.getIndex(highBits);

            if (i >= 0)
            {
                containers.setContainerAtIndex(i, containers.getContainerAtIndex(i).add(Utility.GetLowBits(x))
                );
            }
            else
            {
                ArrayContainer newac = new ArrayContainer();
                containers.insertNewKeyValueAt(-i - 1, highBits, newac.add(Utility.GetLowBits(x)));
            }

        }
    }
}

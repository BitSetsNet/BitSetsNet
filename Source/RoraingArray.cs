using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace BitsetsNET
{
    class RoaringArray 
    {
        protected static short INITIAL_CAPACITY = 5;

        public ushort[] keys = null;
        public Container[] values = null;
        public int size = 0;

        protected RoaringArray() {
            keys = new ushort[INITIAL_CAPACITY];
            values = new Container[INITIAL_CAPACITY];
        }

        public int getIndex(ushort x)
        {
            //TODO: optimize this
            //before the binary search we optimize for frequent cases
            if ((size == 0) || (keys[size - 1] == x)) {
                return size - 1;
            }
            return this.binarySearch(0, size, x); 
        }

        private int binarySearch(int begin, int end, ushort key) {
            return Utility.unsignedBinarySearch(keys, begin, end, key);
        }

        public void setContainerAtIndex(int i, Container c)
        {
            values[i] = c;
        }

        public Container getContainerAtIndex(int i) {
            return values[i];
        }

        // insert a new key, it is assumed that it does not exist
        public void insertNewKeyValueAt(int i, ushort key, Container value)
        {
            extendArray(1);
            Array.Copy(keys, i, keys, i + 1, size - i);
            keys[i] = key;
            Array.Copy(values, i, values, i + 1, size - i);
            values[i] = value;
            size++;
        }

        // make sure there is capacity for at least k more elements
        public void extendArray(int k)
        {
            // size + 1 could overflow
            if (this.size + k >= this.keys.Length)
            {
                int newCapacity;
                if (this.keys.Length < 1024)
                {
                    newCapacity = 2 * (this.size + k);
                }
                else
                {
                    newCapacity = 5 * (this.size + k) / 4;
                }
                //TODO: this may be jank
                Array.Resize(ref this.keys, newCapacity);
                Array.Resize(ref this.values, newCapacity);
            }
        }
    }
}

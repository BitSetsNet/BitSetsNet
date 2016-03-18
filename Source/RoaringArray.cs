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

        public RoaringArray() : this(INITIAL_CAPACITY) {
            //no additional work needed
        }

        public RoaringArray(int capacity)
        {
            keys = new ushort[capacity];
            values = new Container[capacity];
        }

        public void append(ushort key, Container value)
        {
            extendArray(1);
            this.keys[this.size] = key;
            this.values[this.size] = value;
            this.size++;
        }

        public RoaringArray clone() {
            RoaringArray sa = new RoaringArray();

            sa.keys = new ushort[this.keys.Length];
            this.keys.CopyTo(sa.keys, 0);

            sa.values = new Container[this.values.Length];
            this.values.CopyTo(sa.values, 0);

            for (int k = 0; k < this.size; ++k)
                sa.values[k] = sa.values[k].clone();

            sa.size = this.size;
            
            return sa;
        }
        /// <summary>
        /// Append copy of the one value from another array
        /// </summary>
        /// <param name="sa">
        /// other array
        /// </param>
        /// <param name="index">
        /// index in the other array
        /// </param>
        public void appendCopy(RoaringArray sa, int index)
        {
            extendArray(1);
            this.keys[this.size] = sa.keys[index];
            this.values[this.size] = sa.values[index].clone();
            this.size++;
        }

        /// <summary>
        /// Copies a range of keys and values from one location in 
        /// the roaring array to another.
        /// </summary>
        /// <param name="begin">Original starting index</param>
        /// <param name="end">Original ending index</param>
        /// <param name="newBegin">New starting index</param>
        internal void copyRange(int begin, int end, int newBegin)
        {
            int range = end - begin;
            Array.Copy(this.keys, begin, this.keys, newBegin, range);
            Array.Copy(this.values, begin, this.values, newBegin, range);
        }

        /// <summary>
        /// Append copies of the values from another array
        /// </summary>
        /// <param name="sa">
        /// other array
        /// </param>
        /// <param name="startingIndex">
        /// starting index in the other array
        /// </param>
        /// <param name="end">
        /// (exclusive) in the other array
        /// </param>
        public void appendCopy(RoaringArray sa, int startingIndex, int end)
        {
            extendArray(end - startingIndex);
            for (int i = startingIndex; i < end; ++i)
            {
                this.keys[this.size] = sa.keys[i];
                this.values[this.size] = sa.values[i].clone();
                this.size++;
            }
        }

        public int advanceUntil(ushort x, int pos)
        {
            int lower = pos + 1;

            // special handling for a possibly common sequential case
            if (lower >= size || keys[lower] >= x)
            {
                return lower;
            }

            int spansize = 1; // could set larger
            // bootstrap an upper limit

            while (lower + spansize < size && keys[lower + spansize] < x)
                spansize *= 2; // hoping for compiler will reduce to shift
            int upper = (lower + spansize < size) ? lower + spansize : size - 1;

            // maybe we are lucky (could be common case when the seek ahead
            // expected to be small and sequential will otherwise make us look bad)
            if (keys[upper] == x)
            {
                return upper;
            }

            if (keys[upper] < x)
            {// means array has no item key >= x
                return size;
            }

            // we know that the next-smallest span was too small
            lower += (spansize / 2);

            // else begin binary search
            // invariant: array[lower]<x && array[upper]>x
            while (lower + 1 != upper)
            {
                int mid = (lower + upper) / 2;
                if (keys[mid] == x)
                    return mid;
                else if (keys[mid] < x)
                    lower = mid;
                else
                    upper = mid;
            }
            return upper;
        }

        public ushort getKeyAtIndex(int i)
        {
            return this.keys[i];
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

        /// <summary>
        /// Logically resizes the Roaring Array after an in-place operation.
        /// Fills all keys and values after its new last index with zeros
        /// and null, respectively, and changes the size to the new size.
        /// </summary>
        /// <param name="newSize">the new size of the roaring array</param>
        public void resize (int newSize)
        {
            Utility.Fill(keys, newSize, size, (ushort)0);
            Utility.Fill(values, newSize, size, null);
            size = newSize;
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

        public void removeAtIndex(int i)
        {
            Array.Copy(keys, i + 1, keys, i, size - i - 1);
            keys[size - 1] = 0;
            Array.Copy(values, i + 1, values, i, size - i - 1);
            values[size - 1] = null;
            size--;
        }

        public void removeIndexRange(int begin, int end)
        {
            if (end <= begin) return;
                int range = end - begin;

            Array.Copy(keys, end, keys, begin, size - end);
            Array.Copy(values, end, values, begin, size - end);

            for (int i = 1; i <= range; ++i)
            {
                keys[size - i] = 0;
                values[size - i] = null;
            }
            size -= range;
        }
        
        /// <summary>
        /// insert a new key, it is assumed that it does not exist
        /// </summary>
        /// <param name="i"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void insertNewKeyValueAt(int i, ushort key, Container value)
        {
            extendArray(1);
            Array.Copy(keys, i, keys, i + 1, size - i);
            keys[i] = key;
            Array.Copy(values, i, values, i + 1, size - i);
            values[i] = value;
            size++;
        }

        /// <summary>
        /// Replaces the key and container value at a given index.
        /// </summary>
        /// <param name="i">the working index</param>
        /// <param name="key">key to set</param>
        /// <param name="c">container to set</param>
        public void replaceKeyAndContainerAtIndex(int i, ushort key, Container c)
        {
            keys[i] = key;
            values[i] = c;
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

        public override bool Equals(Object o) {
            if (o is RoaringArray) {
                RoaringArray srb = (RoaringArray) o;
                if (srb.size != this.size) {
                    return false;
                }
                for (int i = 0; i < srb.size; ++i) {
                    if (this.keys[i] != srb.keys[i] || !this.values[i].Equals(srb.values[i])) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = size;
                for (int i = 0; i < size; i++)
                {
                    hash = unchecked(17 * hash + keys[i]);
                }
                return hash;
            }
        }

        /// <summary>
        /// Serialize the roaring array into a binary format.
        /// </summary>
        /// <param name="writer">The writer to write the serialization to.</param>
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(size);

            for(int i = 0; i < size; i++)
            {
                writer.Write(keys[i]);
                values[i].Serialize(writer);
            }
        }

        /// <summary>
        /// Deserialize a roaring array from a binary format, as written by the Serialize method.
        /// </summary>
        /// <param name="reader">The reader from which to deserialize the roaring array.</param>
        /// <returns></returns>
        public static RoaringArray Deserialize(BinaryReader reader)
        {
            RoaringArray array = new RoaringArray(reader.ReadInt32());
            
            for(int i = 0; i < array.size; i++)
            {
                array.keys[i] = (ushort) reader.ReadInt16();
                array.values[i] = Container.Deserialize(reader);
            }

            return array;
        }

        /// <summary>
        /// Get an enumerator of the set indices of this bitset.
        /// </summary>
        /// <returns>A enumerator giving the set (i.e. for which the bit is '1' or true) indices for this bitset.</returns>
        public IEnumerator<int> GetEnumerator()
        {
            for(int i = 0; i < size; i++)
            {
                int highbits = keys[i] << 16;
                foreach(ushort lowbits in values[i])
                {
                    yield return highbits + lowbits;
                }
            }
        }
    }
}

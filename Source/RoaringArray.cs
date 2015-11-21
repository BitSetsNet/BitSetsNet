﻿using System;
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

        public RoaringArray() {
            keys = new ushort[INITIAL_CAPACITY];
            values = new Container[INITIAL_CAPACITY];
        }

        public void append(ushort key, Container value)
        {
            extendArray(1);
            this.keys[this.size] = key;
            this.values[this.size] = value;
            this.size++;
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

using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace BitsetsNET
{
    public class UncompressedBitArray : IBitset
    {
        private BitArray array;
        
        public UncompressedBitArray() { }

        public UncompressedBitArray(UncompressedBitArray copy)
        {
            array = new BitArray(copy.array);
        }

        public UncompressedBitArray(int[] indices) : this(indices, indices.Max()) { }

        public UncompressedBitArray(int[] indices, int capacity)
        {
            int length = capacity;
            array = new BitArray(length);
            foreach (int index in indices)
            {
                array.Set(index, true);
            }
        }

        public int Length
        {
            get
            {
                return array.Length;
            }
        }

        /// <summary>
        /// Creates a new bitset that is the bitwise AND of this bitset with another
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        /// <returns>A new IBitset</returns>
        public IBitset And(IBitset otherSet)
        {
            IBitset result = this.Clone();
            result.AndWith(otherSet);
            return result;
        }

        /// <summary>
        /// Performs an in-place intersection of two Roaring Bitsets.
        /// </summary>
        /// <param name="otherSet">the second Roaring Bitset to intersect</param>
        public void AndWith(IBitset otherSet)
        {
            array = array.And(((UncompressedBitArray)otherSet).array);
        }

        /// <summary>
        /// Create a new bitset that is a deep copy of this one.
        /// </summary>
        /// <returns>The cloned bitset</returns>
        public IBitset Clone()
        {
            return new UncompressedBitArray(this);
        }

        /// <summary>
        /// Return whether the given index is a member of this set
        /// </summary>
        /// <param name="index">the index to test</param>
        /// <returns>True if the index is a member of this set</returns>
        public bool Get(int index)
        {
            return array.Get(index);
        }

        /// <summary>
        /// Creates a new bitset that is the bitwise OR of this bitset with another bitset.
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        /// <returns>A new IBitset</returns>
        public IBitset Or(IBitset otherSet)
        {
            IBitset result = this.Clone();
            result.OrWith(otherSet);
            return result;
        }

        /// <summary>
        /// Computes the in-place bitwise OR of this bitset with another bitset.
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        public void OrWith(IBitset otherSet)
        {
            array = array.Or(((UncompressedBitArray)otherSet).array);
        }

        /// <summary>
        /// If the value is true and given index is not in the set add it. If
        /// the value is false and the index is in the set remove it. Otherwise,
        /// do nothing.
        /// </summary>
        /// <param name="index">the index to set</param>
        public void Set(int index, bool value)
        {
            array.Set(index, value);
        }

        /// <summary>
        /// Sets all bits in the array to the specified value
        /// </summary>
        /// <param name="value"></param>
        public void SetAll(bool value)
        {
            array.SetAll(value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.array.Length; i++)
            {
                if (this.array.Get(i) == true)
                { 
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Returns a new bitset that consists of all elements that are not
        /// in this bitset.
        /// </summary>
        /// <returns>The new bitset</returns>
        public IBitset Not()
        {
            var newSet = new UncompressedBitArray();
            newSet.array = array.Not();

            return newSet;
        }

        /// <summary>
        /// The number of members in the set
        /// </summary>
        /// <returns>an integer for the number of members in the set</returns>
        public int Cardinality()
        {
            int rtnValue = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i]) rtnValue += 1;
            }

            return rtnValue;
        }

        /// <summary>
        /// For indices in the range [start, end) add the index to the set if
        /// the value is true, otherwise remove it.
        /// </summary>
        /// <param name="start">the index to start from (inclusive)</param>
        /// <param name="end">the index to stop at (exclusive)</param>
        public void Set(int start, int end, bool value)
        {
            for (int i = start; i <= end; i++ )
            {
                array.Set(i, value);
            }
        }

        /// <summary>
        /// If the given index is not in the set add it, otherwise remove it.
        /// </summary>
        /// <param name="index">the index to flip</param>
        public void Flip(int index)
        {
            array[index] = !array[index];
        }

        /// <summary>
        /// For indices in the range [start, end) add the index to the set if
        /// it does not exists, otherwise remove it.
        /// </summary>
        /// <param name="start">the index to start from (inclusive)</param>
        /// <param name="end">the index to stop at (exclusive)</param>
        public void Flip(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                Flip(i);
            }
        }

        /// <summary>
        /// Creates a new IBitSet that has the members of this BitSet that are 
        /// not members of the other bitset
        /// </summary>
        /// <param name="otherSet">The other bitset</param>
        /// <returns>a new IBitSet</returns>
        public IBitset Difference(IBitset otherSet)
        {
            UncompressedBitArray workset = null;
            if (otherSet is UncompressedBitArray)
            {
                workset = (UncompressedBitArray)otherSet;
            }
            else
            {
                throw new InvalidOperationException("otherSet is not an UncompressedBitArray");
            }

            UncompressedBitArray newArray = (UncompressedBitArray) this.Clone();

            for (int i = 0; i < workset.array.Length; i++)
            {
                if (workset.array[i] && i < this.Length)
                {
                    newArray.Set(i, false);
                }
            }
            return newArray;
        }

        public BitArray ToBitArray()
        {
            return new BitArray(this.array);
        }

        public void Serialize(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            UncompressedBitArray compare = (UncompressedBitArray)obj;
            bool equalLength = (this.Length == compare.Length);
            if (equalLength)
            {
                for (int i = 0; i < this.Length; i++)
                {
                    if (this.array.Get(i) != compare.array.Get(i))
                    {
                        equalLength = false;
                    }

                }
            }
            return equalLength;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Length;
                for (int i = 0; i < Length; i++)
                {
                    hash = 17 * hash + array.Get(i).GetHashCode();

                }
                return hash;
            }
        }
    }
}

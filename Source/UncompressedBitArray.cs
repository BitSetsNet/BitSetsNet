using System;
using System.Collections;
using System.Linq;

namespace BitsetsNET
{
    public class UncompressedBitArray : IBitset
    {
        private BitArray _Array;

        public UncompressedBitArray()
        {

        }

        public UncompressedBitArray(UncompressedBitArray copy)
        {
            _Array = new BitArray(copy._Array);
        }

        public UncompressedBitArray(int[] indices) : this(indices, indices.Max())
        {
            //call the other one   
        }

        public UncompressedBitArray(int[] indices, int capacity)
        {
            int length = capacity;
            _Array = new BitArray(length);
            foreach (int index in indices)
            {
                _Array.Set(index, true);
            }
        }

        public IBitset And(IBitset otherSet)
        {
            if (otherSet.GetType().Equals(this))
            {
                //same type proceed 
                IBitset result = this.Clone();
                result.AndWith(otherSet);
                return result;
            }
            else
            {
                throw new InvalidCastException(otherSet.GetType().Name);
            }
        }

        public void AndWith(IBitset otherSet)
        {
            _Array = _Array.And(((UncompressedBitArray)otherSet)._Array);
        }

        public IBitset Clone()
        {
            return new UncompressedBitArray(this);
        }

        public bool Get(int index)
        {
            return _Array.Get(index);
        }

        public int Length()
        {
            return _Array.Length;
        }

        public IBitset Or(IBitset otherSet)
        {
            if (otherSet.GetType().Equals(this))
            {
                //same type proceed 
                IBitset result = this.Clone();
                result.AndWith(otherSet);
                return result;
            }
            else
            {
                throw new InvalidCastException(otherSet.GetType().Name);
            }
        }

        public void OrWith(IBitset otherSet)
        {
            _Array = _Array.Or(((UncompressedBitArray)otherSet)._Array);
        }

        public void Set(int index, bool value)
        {
            _Array.Set(index, value);
        }

        public void SetAll(bool value)
        {
            _Array.SetAll(value);
        }
    }
}

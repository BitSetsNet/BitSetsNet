using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace BitsetsNET
{
    public class UncompressedBitArray : IBitset
    {
        private BitArray _Array;

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                UncompressedBitArray compare = (UncompressedBitArray)obj;
                bool answer = (this.Length() == compare.Length());
                if (answer) {
                    for (int i = 0; i < Length(); i++)
                    {
                        if (this._Array.Get(i) != compare._Array.Get(i))
                        {
                            answer = false;
                        }

                    }
                }
                return answer;
            }
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

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
            IBitset result = this.Clone();
            result.AndWith(otherSet);
            return result;
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
            IBitset result = this.Clone();
            result.OrWith(otherSet);
            return result;
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _Array.GetEnumerator();
        }

        public IBitset Not()
        {
            var newSet = new UncompressedBitArray();
            newSet._Array = _Array.Not();

            return newSet;
        }

        public int Cardinality()
        {
            throw new NotImplementedException();
        }
    }
}

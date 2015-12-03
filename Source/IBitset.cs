using System;
using System.Collections;

namespace BitsetsNET
{
    /// <summary>
    /// This is the common interface for all bitsets regardless of compression. It defines common operations. 
    /// </summary>
    public interface IBitset : IEnumerable, System.Runtime.Serialization.ISerializable
    {
        IBitset And(IBitset otherSet);

        void AndWith(IBitset otherSet);

        IBitset Clone();

        IBitset Or(IBitset otherSet);      

        void OrWith(IBitset otherSet);

        bool Get(int index);

        int Length();

        void Set(int index, bool value);

        void SetAll(bool value);

        bool Equals(object obj);

        IBitset Not();

        int Cardinality();


    }
       
}

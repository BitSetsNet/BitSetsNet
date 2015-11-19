using System;

namespace BitsetsNET
{
    /// <summary>
    /// This is the common interface for all bitsets regardless of compression. It defines common operations. 
    /// </summary>
    public interface IBitset 
    {
        IBitset And(IBitset x);
        IBitset Or(IBitset y);
        int Length();
        bool Get(int index);
        void Set(int index, bool value);
        void SetAll(bool value);
    }

    public interface IBitSet<T> : IBitset 
    {
        T And(T x);
        T Or(T x);       
    }

}

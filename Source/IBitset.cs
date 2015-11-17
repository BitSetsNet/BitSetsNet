namespace BitsetNET
{
    /// <summary>
    /// This is the common interface for all bitsets regardless of compression. It defines common operations. 
    /// </summary>
    public interface IBitset 
    {
        IBitset And(IBitset x);
        IBitset Or(IBitset y);
        int Length();
        void SetOne(int index);
        void SetZero(int index);
        bool IsOne(int index);
    }
}

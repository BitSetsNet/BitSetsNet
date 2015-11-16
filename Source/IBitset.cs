namespace BitsetsNET
{
    /// <summary>
    /// This is the common interface for all bitsets regardless of compression. It defines common operations. 
    /// </summary>
    public interface IBitset 
    {
        IBitset And(IBitset x);
        IBitset Or(IBitset y);
        IBitset Length();
        void SetOne(int index);
        void SetZero(int index);
        bool IsOne(int index);
    }
}

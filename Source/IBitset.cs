using System.Collections;

namespace BitsetsNET
{
    /// <summary>
    /// This is the common interface for all bitsets regardless of compression. It defines common operations. 
    /// </summary>
    public interface IBitset : IEnumerable, System.Runtime.Serialization.ISerializable
    {
        /// <summary>
        /// Creates a new bitset that is the bitwise AND of this bitset with another bitset
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        /// <returns>A new IBitset</returns>
        IBitset And(IBitset otherSet);

        /// <summary>
        /// Performs an in-place intersection of two Bitsets.
        /// </summary>
        /// <param name="otherSet">the second Bitset to intersect</param>
        void AndWith(IBitset otherSet);

        /// <summary>
        /// Create a new bitset that is a deep copy of this one.
        /// </summary>
        /// <returns>The cloned bitset</returns>
        IBitset Clone();

        /// <summary>
        /// Creates a new bitset that is the bitwise OR of this bitset with another bitset.
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        /// <returns>A new IBitset</returns>
        IBitset Or(IBitset otherSet);

        /// <summary>
        /// Computes the in-place bitwise OR of this bitset with another bitset.
        /// </summary>
        /// <param name="otherSet">Other bitset</param>
        void OrWith(IBitset otherSet);

        /// <summary>
        /// Return whether the given index is a member of this set
        /// </summary>
        /// <param name="index">the index to test</param>
        /// <returns>True if the index is a member of this set</returns>
        bool Get(int index);

        /// <summary>
        /// If the value is true and given index is not in the set add it. If
        /// the value is false and the index is in the set remove it. Otherwise,
        /// do nothing.
        /// </summary>
        /// <param name="index">the index to set</param>
        void Set(int index, bool value);

        /// <summary>
        /// For indices in the range [start, end) add the index to the set if
        /// the value is true, otherwise remove it.
        /// </summary>
        /// <param name="start">the index to start from (inclusive)</param>
        /// <param name="end">the index to stop at (exclusive)</param>
        void Set(int start, int end, bool value);

        /// <summary>
        /// If the given index is not in the set add it, otherwise remove it.
        /// </summary>
        /// <param name="index">the index to flip</param>
        void Flip(int index);

        /// <summary>
        /// For indices in the range [start, end) add the index to the set if
        /// it does not exists, otherwise remove it.
        /// </summary>
        /// <param name="start">the index to start from (inclusive)</param>
        /// <param name="end">the index to stop at (exclusive)</param>
        void Flip(int start, int end);

        /// <summary>
        /// Creates a new IBitSet that has the members of this BitSet that are 
        /// not members of the otherSet.
        /// </summary>
        /// <param name="otherSet">The other bitset</param>
        /// <returns>a new IBitSet</returns>
        IBitset Difference(IBitset otherSet);

        bool Equals(object obj);

        /// <summary>
        /// The number of members in the set
        /// </summary>
        /// <returns>an integer for the number of members in the set</returns>
        int Cardinality();

        void Serialize(System.IO.Stream stream);

    }
}

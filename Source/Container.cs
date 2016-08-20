using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    public abstract class Container : IEnumerable<ushort>
    {
        
        /// <summary>
        /// Add a short to the container. May generate a new container.
        /// </summary>
        /// <param name="x">short to be added</param>
        /// <returns>this container, modified</returns>
        public abstract Container Add(ushort x);

        /// <summary>
        /// Add to the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        /// <returns>this container, modified</returns>
        public abstract Container Add(ushort rangeStart, ushort rangeEnd);

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container And(ArrayContainer x);

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container And(BitsetContainer x);

        /// <summary>
        /// Computes the bitwise AND of this container with another
        /// (intersection). This container as well as the provided container are
        /// left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public Container And(Container x)
        {
            if (x is ArrayContainer)
            {
                return And((ArrayContainer)x);
            }
            return And((BitsetContainer) x);
        }

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). This container as well as the provided container are
        /// left unaffected. 
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>A new container with the result</returns>
        public abstract Container AndNot(ArrayContainer x);

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). This container as well as the provided container are
        /// left unaffected. 
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>A new container with the result</returns>
        public abstract Container AndNot(BitsetContainer x);

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). This container as well as the provided container are
        /// left unaffected. 
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>A new container with the result</returns>
        public Container AndNot(Container x)
        {
            if (x is ArrayContainer)
            {
                return AndNot((ArrayContainer)x);
            }
            return AndNot((BitsetContainer) x);
        }

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). Modifies the current container in place.
        /// </summary>
        /// <param name="x">Other container</param>
        public abstract Container IAndNot(ArrayContainer x);

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). Modifies the current container in place.
        /// </summary>
        /// <param name="x">Other container</param>
        public abstract Container IAndNot(BitsetContainer x);

        /// <summary>
        /// Computes the bitwise ANDNOT of this container with another
        /// (difference). Modifies the current container in place.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>A new container with the result</returns>
        public Container IAndNot(Container x)
        {
            if (x is ArrayContainer)
            {
                return IAndNot((ArrayContainer)x);
            }
            return IAndNot((BitsetContainer)x);
        }

        public abstract Container Clone();

        /// <summary>
        /// Checks whether the container contains the provided value.
        /// </summary>
        /// <param name="x">Value to check</param>
        public abstract bool Contains(ushort x);

        /// <summary>
        /// Fill the least significant 16 bits of the integer array, starting at
        /// index i, with the short values from this container. The caller is
        /// responsible to allocate enough room. The most significant 16 bits of
        /// each integer are given by the most significant bits of the provided mask.
        /// </summary>
        /// <param name="x">Provided array</param>
        /// <param name="i">Starting index</param>
        /// <param name="mask">Indicates most significant bits</param>
        public abstract void FillLeastSignificant16bits(int[] x, int i, int mask);

        /// <summary>
        /// Computes the distinct number of short values in the container. Can be
        /// expected to run in constant time.
        /// </summary>
        /// <returns>The cardinality</returns>
        public abstract int GetCardinality();

        /// <summary>
        /// Computes the in-place bitwise AND of this container with another
        /// (intersection). The current container is generally modified, whereas
        /// the provided container (x) is unaffected. May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container IAnd(ArrayContainer x);

        /// <summary>
        /// Computes the in-place bitwise AND of this container with another
        /// (intersection). The current container is generally modified, whereas
        /// the provided container (x) is unaffected. May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container IAnd(BitsetContainer x);

        /// <summary>
        /// Computes the in-place bitwise AND of this container with another
        /// (intersection). The current container is generally modified, whereas
        /// the provided container (x) is unaffected. May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public Container IAnd(Container x)
        {
            if (x is ArrayContainer)
            {
                return IAnd((ArrayContainer)x);
            }
            return IAnd((BitsetContainer)x);
        }

        /// <summary>
        /// Computes the in-place bitwise NOT of this container (complement). Only those bits within the
        /// range are affected.The current container is generally modified.May generate a new container.
        /// </summary>
        /// <param name="start">beginning of range (inclusive); 0 is beginning of this container.</param>
        /// <param name="end">ending of range (exclusive)</param>
        /// <returns>(Partially) complemented container</returns>
        public abstract Container INot(int start, int end);

        /// <summary>
        /// Returns true if the current container intersects the other container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Whether they intersect</returns>
        public bool Intersects(Container x)
        {
            if (x is ArrayContainer)
            {
                return Intersects((ArrayContainer)x);
            }
            return Intersects((BitsetContainer)x);
        }

        /// <summary>
        /// Returns true if the current container intersects the other container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Whether they intersect</returns>
        public abstract bool Intersects(ArrayContainer x);

        /// <summary>
        /// Returns true if the current container intersects the other container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Whether they intersect</returns>
        public abstract bool Intersects(BitsetContainer x);

        /// <summary>
        /// Create a container initialized with a range of consecutive values.
        /// </summary>
        /// <param name="start">First index</param>
        /// <param name="last">Last index</param>
        /// <returns>A new container initialized with the specified values</returns>
        /// <remarks>In the original lemire version, there is some optimization here
        /// to choose between an ArrayContainer and a RunContainer based on serialized size.
        /// For now, this has been stripped out and always uses an ArrayContainer.</remarks>
        public static Container RangeOfOnes(ushort start, ushort last)
        {
            //TODO: Add in logic for RunContainers
            Container answer = new ArrayContainer();
            answer = answer.IAdd(start, last);
            return answer;
        }

        /// <summary>
        /// Add a short to the container if it is not present, otherwise remove it. May generate a new
        /// container.
        /// </summary>
        /// <param name="x">short to be added</param>
        /// <returns>the new container</returns>
        public abstract Container Flip(ushort x);

        /// <summary>
        /// Add all shorts in [begin,end) using an unsigned interpretation. May generate a new container.
        /// </summary>
        /// <param name="begin">Start of range</param>
        /// <param name="end">End of range</param>
        /// <returns>The new container</returns>
        public abstract Container IAdd(ushort begin, ushort end);

        /// <summary>
        /// Computes the in-place bitwise OR of this container with another
        /// (union). The current container is generally modified, whereas the
        /// provided container(x) is unaffected.May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container IOr(ArrayContainer x);

        /// <summary>
        /// Computes the in-place bitwise OR of this container with another
        /// (union). The current container is generally modified, whereas the
        /// provided container(x) is unaffected.May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container IOr(BitsetContainer x);

        /// <summary>
        /// Computes the in-place bitwise OR of this container with another
        /// (union). The current container is generally modified, whereas the
        /// provided container(x) is unaffected.May generate a new container.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public Container IOr(Container x)
        {
            if (x is ArrayContainer)
            {
                return IOr((ArrayContainer)x);
            }
            return IOr((BitsetContainer)x);
        }

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container Or(ArrayContainer x);

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public abstract Container Or(BitsetContainer x);

        /// <summary>
        /// Computes the bitwise OR of this container with another (union). This
        /// container as well as the provided container are left unaffected.
        /// </summary>
        /// <param name="x">Other container</param>
        /// <returns>Aggregated container</returns>
        public Container Or(Container x)
        {
            if (x is ArrayContainer)
            {
                return Or((ArrayContainer)x);
            }
            return Or((BitsetContainer)x);
        }
        
        /// <summary>
        /// Remove specified short from this container. May create a new container.
        /// </summary>
        /// <param name="x">Short to be removed</param>
        /// <returns>The new container</returns>
        public abstract Container Remove(ushort x);

        /// <summary>
        /// Remove shorts in [begin,end) using an unsigned interpretation. May generate a new container.
        /// </summary>
        /// <param name="begin">Start of range (inclusive)</param>
        /// <param name="end">End of range (exclusive)</param>
        /// <returns>The new container</returns>
        public abstract Container Remove(ushort begin, ushort end);

        /// <summary>
        /// Return the jth value of the container.
        /// </summary>
        /// <param name="j">Index of the value </param>
        /// <returns>The jth value of the container</returns>
        public abstract ushort Select(int j);

        /// <summary>
        /// Serialize this container in a binary format.
        /// </summary>
        /// <param name="writer">The binary writer to write the serialization to.</param>
        public abstract void Serialize(System.IO.BinaryWriter writer);

        /// <summary>
        /// Deserialize a container from a binary reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The next container represented by the reader.</returns>
        /// <remarks>The binary format for deserialization is the format written by the Serialize method.</remarks>
        public static Container Deserialize(System.IO.BinaryReader reader)
        {
            int cardinality = reader.ReadInt32();
            if(cardinality < ArrayContainer.DEFAULT_MAX_SIZE)
            {
                return ArrayContainer.Deserialize(reader, cardinality);
            }
            else
            {
                return BitsetContainer.Deserialize(reader, cardinality);
            }
        }

        public abstract IEnumerator<ushort> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

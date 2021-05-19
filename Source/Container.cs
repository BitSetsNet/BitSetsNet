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
        /// <returns>this container modified</returns>
        public abstract Container add(ushort x);

        /**
        * Add to the current bitmap all integers in [rangeStart,rangeEnd).
        *
        * @param rangeStart inclusive beginning of range
        * @param rangeEnd   exclusive ending of range
        */

        /// <summary>
        /// Add to the current bitmap all integers in [rangeStart,rangeEnd).
        /// </summary>
        /// <param name="rangeStart">inclusive beginning of range</param>
        /// <param name="rangeEnd">exclusive ending of range</param>
        public abstract Container add(ushort rangeStart, ushort rangeEnd);

        /**
         * Computes the bitwise AND of this container with another
         * (intersection). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container and(ArrayContainer x);

        /**
         * Computes the bitwise AND of this container with another
         * (intersection). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container and(BitsetContainer x);

        /**
         * Computes the bitwise AND of this container with another
         * (intersection). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public Container and(Container x)
        {
            if (x is ArrayContainer)
                return and((ArrayContainer) x);
            return and((BitsetContainer) x);
        }

        /**
         * Computes the bitwise Difference of this container with another.
         * This container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container difference(ArrayContainer x);

        /**
         * Computes the bitwise Difference of this container with another.
         * This container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container difference(BitsetContainer x);

        /**
         * Computes the bitwise Difference of this container with another.
         * This container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public Container difference(Container x)
        {
            if (x is ArrayContainer)
                return difference((ArrayContainer) x);
            return difference((BitsetContainer) x);
        }

        /**
        * Computes whether any elements are shared between the two containers.
        *
        * @param x other container
        * @return true if elements are shared
        */
        public abstract bool overlaps(ArrayContainer x);

        /**
        * Computes whether any elements are shared between the two containers.
        *
        * @param x other container
        * @return true if elements are shared
        */
        public abstract bool overlaps(BitsetContainer x);

        /**
         * Computes whether any elements are shared between the two containers.
         *
         * @param x other container
         * @return true if elements are shared
         */
        public bool overlaps(Container x)
        {
            if (x is ArrayContainer)
                return overlaps((ArrayContainer) x);
            return overlaps((BitsetContainer) x);
        }

        
        public abstract Container clone();

        /**
         * Checks whether the contain contains the provided value
         *
         * @param x value to check
         * @return whether the value is in the container
         */
        public abstract bool contains(ushort x);

        /**
         * Fill the least significant 16 bits of the integer array, starting at
         * index i, with the short values from this container. The caller is
         * responsible to allocate enough room. The most significant 16 bits of
         * each integer are given by the most significant bits of the provided
         * mask.
         *
         * @param x    provided array
         * @param i    starting index
         * @param mask indicates most significant bits
         */
        public abstract void fillLeastSignificant16bits(int[] x, int i, int mask);

        /**
         * Computes the distinct number of short values in the container. Can be
         * expected to run in constant time.
         *
         * @return the cardinality
         */
        public abstract int getCardinality();

        /**
         * Computes the in-place bitwise AND of this container with another
         * (intersection). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container iand(ArrayContainer x);

        /**
         * Computes the in-place bitwise AND of this container with another
         * (intersection). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container iand(BitsetContainer x);

        /**
         * Computes the in-place bitwise AND of this container with another
         * (intersection). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public Container iand(Container x)
        {
            if (x is ArrayContainer)
                return iand((ArrayContainer)x);
            return iand((BitsetContainer)x);
        }

        /**
         * Returns true if the current container intersects the other container.
         *
         * @param x other container
         * @return whether they intersect
         */
        public bool intersects(Container x)
        {
            if (x is ArrayContainer)
                return intersects((ArrayContainer)x);
            return intersects((BitsetContainer)x);
        }

        /**
         * Returns true if the current container intersects the other container.
         *
         * @param x other container
         * @return whether they intersect
         */
        public abstract bool intersects(ArrayContainer x);

        /**
         * Returns true if the current container intersects the other container.
         *
         * @param x other container
         * @return whether they intersect
          */
        public abstract bool intersects(BitsetContainer x);


        /**
 * Computes the in-place bitwise OR of this container with another
 * (union). The current container is generally modified, whereas the
 * provided container (x) is unaffected. May generate a new container.
 *
 * @param x other container
 * @return aggregated container
 */
        public abstract Container ior(ArrayContainer x);

        /**
         * Computes the in-place bitwise OR of this container with another
         * (union). The current container is generally modified, whereas the
         * provided container (x) is unaffected. May generate a new container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container ior(BitsetContainer x);

        /**
 * Computes the in-place bitwise OR of this container with another
 * (union). The current container is generally modified, whereas the
 * provided container (x) is unaffected. May generate a new container.
 *
 * @param x other container
 * @return aggregated container
 */
        public Container ior(Container x)
        {
            if (x is ArrayContainer)
                return ior((ArrayContainer)x);
            return ior((BitsetContainer)x);
        }


        /**
         * Computes the bitwise OR of this container with another (union). This
         * container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container or(ArrayContainer x);

        /**
         * Computes the bitwise OR of this container with another (union). This
         * container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container or(BitsetContainer x);

        /**
 * Computes the bitwise OR of this container with another (union). This
 * container as well as the provided container are left unaffected.
 *
 * @param x other container
 * @return aggregated container
 */
        public Container or(Container x)
        {
            if (x is ArrayContainer)
                return or((ArrayContainer)x);
            return or((BitsetContainer)x);
        }
        
        /// <summary>
        /// Remove the short from this container. May create a new container.
        /// </summary>
        /// <param name="x">to be removed</param>
        /// <returns>the new container</returns>
        public abstract Container remove(ushort x);

        /// <summary>
        /// Remove shorts in [begin,end) using an unsigned interpretation. May generate a new container.
        /// </summary>
        /// <param name="begin">start of range (inclusive)</param>
        /// <param name="end">end of range (exclusive)</param>
        /// <returns>the new container</returns>
        public abstract Container remove(ushort begin, ushort end);

        /**
         * Return the jth value 
         * 
         * @param j index of the value 
         *
         * @return the value
         */
        public abstract ushort select(int j);


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

            if(cardinality <= ArrayContainer.DEFAULT_MAX_SIZE)
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

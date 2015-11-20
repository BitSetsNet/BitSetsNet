using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    public abstract class Container
    {

        /**
         * Get the name of this container. 
         * 
         * @return name of the container
         */
        public String getContainerName()
        {
            if (this is BitsetContainer) {
                return "bitset";
            } else if (this is ArrayContainer) {
                return "array";
            } else {
                return "run";
            }
        }

        /**
         * Add a short to the container. May generate a new container.
         *
         * @param x short to be added
         * @return the new container
         */
        public abstract Container add(short x);

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

        /* Computes the bitwise ANDNOT of this container with another
         * (difference). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container andNot(ArrayContainer x);

        /**
         * Computes the bitwise ANDNOT of this container with another
         * (difference). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container andNot(BitsetContainer x);

        /**
         * Computes the bitwise ANDNOT of this container with another
         * (difference). This container as well as the provided container are
         * left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public Container andNot(Container x)
        {
            if (x is ArrayContainer)
                return andNot((ArrayContainer)x);
            return andNot((BitsetContainer)x);
        }

        /**
         * Empties the container
         */
        public abstract void clear();

        
        public abstract override Container clone();

        /**
         * Checks whether the contain contains the provided value
         *
         * @param x value to check
         * @return whether the value is in the container
         */
        public abstract bool contains(short x);

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
         * Add a short to the container if it is not present, otherwise remove it. 
         * May generate a new container.
         *
         * @param x short to be added
         * @return the new container
         */
        public abstract Container flip(short x);

        /**
         * Size of the underlying array
         *
         * @return size in bytes
         */
        protected abstract int getArraySizeInBytes();

        /**
         * Computes the distinct number of short values in the container. Can be
         * expected to run in constant time.
         *
         * @return the cardinality
         */
        public abstract int getCardinality();

        /**
         * Iterator to visit the short values in the container in ascending order.
         *
         * @return iterator
         */
        public abstract ShortIterator getShortIterator();


        /**
         * Iterator to visit the short values in the container in descending order.
         *
         * @return iterator
         */
        public abstract ShortIterator getReverseShortIterator();

        /**
         * Computes an estimate of the memory usage of this container. The
         * estimate is not meant to be exact.
         *
         * @return estimated memory usage in bytes
         */
        public abstract int getSizeInBytes();

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
         * Computes the in-place bitwise ANDNOT of this container with another
         * (difference). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container iandNot(ArrayContainer x);

        /**
         * Computes the in-place bitwise ANDNOT of this container with another
         * (difference). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container iandNot(BitsetContainer x);

        /**
         * Computes the in-place bitwise ANDNOT of this container with another
         * (difference). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container iandNot(RunContainer x);


        /**
         * Computes the in-place bitwise ANDNOT of this container with another
         * (difference). The current container is generally modified, whereas
         * the provided container (x) is unaffected. May generate a new
         * container.
         *
         * @param x other container
         * @return aggregated container
         */
        public Container iandNot(Container x)
        {
            if (x is ArrayContainer)
                return iandNot((ArrayContainer)x);
            return iandNot((BitsetContainer)x);
        }

        /**
         * Computes the in-place bitwise NOT of this container (complement).
         * Only those bits within the range are affected. The current container
         * is generally modified. May generate a new container.
         *
         * @param rangeStart beginning of range (inclusive); 0 is beginning of this
         *                   container.
         * @param rangeEnd   ending of range (exclusive)
         * @return (partially) complemented container
         */
        public abstract Container inot(int rangeStart, int rangeEnd);

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
         * Computes the in-place bitwise XOR of this container with another
         * (symmetric difference). The current container is generally modified, whereas the
         * provided container (x) is unaffected. May generate a new container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container ixor(ArrayContainer x);

        /**
         * Computes the in-place bitwise XOR of this container with another
         * (symmetric difference). The current container is generally modified, whereas the
         * provided container (x) is unaffected. May generate a new container.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container ixor(BitsetContainer x);

        /**
 * Computes the in-place bitwise OR of this container with another
 * (union). The current container is generally modified, whereas the
 * provided container (x) is unaffected. May generate a new container.
 *
 * @param x other container
 * @return aggregated container
 */
        public Container ixor(Container x)
        {
            if (x is ArrayContainer)
                return ixor((ArrayContainer)x);
            return ixor((BitsetContainer)x);
        }

        /**
    * Computes the bitwise NOT of this container (complement). Only those
    * bits within the range are affected. The current container is left
    * unaffected.
    *
    * @param rangeStart beginning of range (inclusive); 0 is beginning of this
    *                   container.
    * @param rangeEnd   ending of range (exclusive)
    * @return (partially) complemented container
    */
        public abstract Container not(int rangeStart, int rangeEnd);

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

        /**
         * Remove the short from this container. May create a new container.
         *
         * @param x to be removed
         * @return New container
         */
        public abstract Container remove(short x);

        /**
 * Computes the bitwise XOR of this container with another (symmetric difference). This
 * container as well as the provided container are left unaffected.
 *
 * @param x other container
 * @return aggregated container
 */
        public abstract Container xor(ArrayContainer x);

        /**
         * Computes the bitwise XOR of this container with another (symmetric difference). This
         * container as well as the provided container are left unaffected.
         *
         * @param x other container
         * @return aggregated container
         */
        public abstract Container xor(BitsetContainer x);

        /**
 * Computes the bitwise OR of this container with another (symmetric difference). This
 * container as well as the provided container are left unaffected.
 *
 * @param x other parameter
 * @return aggregated container
 */
        public Container xor(Container x)
        {
            if (x instanceof ArrayContainer)
                return xor((ArrayContainer)x);
            return xor((BitsetContainer)x);
        }

        /**
         * Rank returns the number of integers that are smaller or equal to x (Rank(infinity) would be GetCardinality()).
         * @param lowbits upper limit
         *
         * @return the rank
         */
        public abstract int rank(short lowbits);

        /**
         * Return the jth value 
         * 
         * @param j index of the value 
         *
         * @return the value
         */
        public abstract short select(int j);



    }
}

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
         * Add a short to the container. May generate a new container.
         *
         * @param x short to be added
         * @return the new container
         */
        public abstract Container add(ushort x);

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

        /**
         * Remove the short from this container. May create a new container.
         *
         * @param x to be removed
         * @return New container
         */
        public abstract Container remove(ushort x);

        /**
         * Return the jth value 
         * 
         * @param j index of the value 
         *
         * @return the value
         */
        public abstract ushort select(int j);



    }
}

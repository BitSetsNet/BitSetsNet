using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace BitsetsNET
{
    class ContainerArray 
    {
        protected static short INITIAL_CAPACITY = 5;

        public short[] Keys = null;
        public Container[] Values = null;
        public int Size = 0;

        protected ContainerArray() {


            this.Keys = new short[INITIAL_CAPACITY];
            this.Values = new Container[INITIAL_CAPACITY];
        }

        protected int getIndex(short x)
        {
            //TODO: optimize this
            //Add binarySearch to Util
            //if((Size = 0) || (Keys[Size - 1] == x))
            return Array.IndexOf(Keys, x);
        }

        protected void setContainerAtIndex(int i, Container c)
        {
            this.Values[i] = c;
        }
    }
}

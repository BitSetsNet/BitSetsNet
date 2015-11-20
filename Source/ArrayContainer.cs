using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class ArrayContainer : Container
    {
        private const int DEFAULT_INIT_SIZE = 4;
        private const int DEFAULT_MAX_SIZE = 4096;

        protected int cardinality;
        protected short[] content;

        // TODO: FIX THIS
        //public ArrayContainer()
        //{
        //    this(DEFAULT_INIT_SIZE); //I'm not sure what the C# equivalent of this is
        //}
        
        public ArrayContainer(int capacity)
        {
            this.cardinality = 0;
            this.content = new short[capacity];
        }

    }
}

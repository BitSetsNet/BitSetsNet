using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET
{
    class Utility
    {
        public static short GetHighBits(int x)
        {
            uint u = (uint)(x);
            return (short) (u >> 16);
        }

        public static short GetLowBits(int x)
        {
            return (short)(x & 0xFFFF);
        }
    }
}

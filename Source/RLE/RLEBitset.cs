using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.RLE
{
    class RLEBitset : IBitset
    {

        #region "Struct"

        public struct Run
        {
            public int StartIndex;
            public int RunLength;

            public Run(int strtIdx, int length)
            {
                StartIndex = strtIdx;
                RunLength = length;
            }

        }

        #endregion

        #region "Declaration"

        private List<Run> _RLEArray = new List<Run>();

        #endregion

        #region "Factory Methods"

        public static IBitset CreateFrom(BitArray bits)
        {
            RLEBitset rtnVal = new RLEBitset();
            Run currRun = new Run();
            for (int i = 0; i < bits.Count; i++)
            {
                if (bits.Get(i) == true)
                {
                    currRun.StartIndex = i;
                    for (int j = i; j < bits.Count; j++)
                    {
                        if (bits.Get(j) == false)
                        {
                            currRun.RunLength = j - i;
                            rtnVal._RLEArray.Add(currRun);
                            i = j;
                            currRun = new Run();
                            break;
                        }
                    }
                }

            }
            return rtnVal;
        }

        public static IBitset CreateFrom(int[] indicies)
        {
            RLEBitset rtnVal = new RLEBitset();
            Run currRun = new Run();
            for (int i = 0; i < indicies.Length; i++)
            {
                if (indicies[i + 1] - indicies[i] == 1)
                {
                    currRun.StartIndex = i;
                    currRun.RunLength = currRun.RunLength + 1;
                }
                else
                {
                    rtnVal._RLEArray.Add(currRun);
                    currRun = new Run();
                }
            }
            return rtnVal;
        }

        #endregion


        public IBitset And(IBitset otherSet)
        {
            throw new NotImplementedException();
        }

        public void AndWith(IBitset otherSet)
        {
            throw new NotImplementedException();
        }

        public IBitset Clone()
        {
            throw new NotImplementedException();
        }

        public bool Get(int index)
        {
            throw new NotImplementedException();
        }

        public int Length()
        {
            throw new NotImplementedException();
        }

        public IBitset Or(IBitset otherSet)
        {
            throw new NotImplementedException();
        }

        public void OrWith(IBitset otherSet)
        {
            throw new NotImplementedException();
        }

        public void Set(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetAll(bool value)
        {
            throw new NotImplementedException();
        }
    }
}

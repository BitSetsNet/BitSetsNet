using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BitsetsNET.RLE
{
    public class RLEBitset : IBitset
    {

        #region "Struct"

        public struct Run
        {
            public int StartIndex;
            public int EndIndex;

            public Run(int strtIdx, int endIdx)
            {
                StartIndex = strtIdx;
                EndIndex = endIdx;
            }

        }

        #endregion

        #region "Declaration"

        private List<Run> _RunArray = new List<Run>();
        private int _Length;

        #endregion

        #region "Factory Methods"

        /// <summary>
        /// Creates a RLEBitset from a BitArray.
        /// </summary>
        /// <param name="bits">a BitArray</param>
        /// <returns>an RLEBitset</returns>
        public static IBitset CreateFrom(BitArray bits)
        {
            RLEBitset rtnVal = new RLEBitset();
            rtnVal._Length = bits.Length;
            Run currRun = new Run();
            for (int i = 0; i < bits.Count; i++)
            {
                if (bits.Get(i) == true)
                {
                    currRun.StartIndex = i;
                    currRun.EndIndex = i;
                    for (int j = i + 1; j < bits.Count; j++)
                    {
                        if (bits.Get(j))
                        {
                            currRun.EndIndex = j;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = currRun.EndIndex; //move the counter to the end of the run we just found
                    rtnVal._RunArray.Add(currRun);
                    currRun = new Run();
                }

            }
            return rtnVal;
        }

        /// <summary>
        /// Creates a RLEBitset from an array of indices. 
        /// Each value in the input array represents the position (i.e. index) of a 1.
        /// </summary>
        /// <param name="indices">an array of integers representing the positions of 1's</param>
        /// <returns>an RLEBitset</returns>
        public static IBitset CreateFrom(int[] indices)
        {
            int capacity = indices.Max();
            return RLEBitset.CreateFrom(indices, capacity);
        }

        /// <summary>
        /// Creates a RLEBitset from an array of indices and a capacity value. 
        /// Each value in the input array represents the position (i.e. index) of a 1.
        /// The capity represents the length of the uncompressed data.
        /// </summary>
        /// <param name="indices">an array of integers representing the positions of 1's</param>
        /// <param name="capacity">the length of the uncompressed array</param>
        /// <returns></returns>
        public static IBitset CreateFrom(int[] indices, int capacity)
        {
            Array.Sort(indices); // sort the input array first.

            if (indices.Last() > capacity) // capacity must be larger than highest index value in input array.
            {
                throw new ArgumentException("capacity cannot be less than max index value");
            }
          
            RLEBitset rtnVal = new RLEBitset();
            rtnVal._Length = capacity;
            Run currRun = new Run();
            currRun.StartIndex = indices.FirstOrDefault();
            for (int i = 0; i < indices.Length; i++)
            {
                if (i == indices.Length - 1)
                {
                    currRun.EndIndex = indices[i];
                    rtnVal._RunArray.Add(currRun);
                }
                else if(indices[i + 1] - indices[i] > 1)
                {
                    currRun.EndIndex = indices[i];
                    rtnVal._RunArray.Add(currRun);
                    currRun = new Run();
                    currRun.StartIndex = indices[i+1];
                }
            }
            return rtnVal;
        }

        #endregion

        #region "Public Methods"

        public IBitset And(IBitset otherSet)
        {
            if (!(otherSet is RLEBitset))
            {
                throw new ArgumentException("otherSet must be a RLEBitset to perform this operation.");
            }
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset

            RLEBitset rtnVal = new RLEBitset(); // instantiate the return value
            
            List<Run> runsA = this._RunArray;
            List<Run> runsB = otherRLESet._RunArray;

            int i = 0;
            int j = 0;

            while (i < runsA.Count && j < runsB.Count)
            {
                // check for overlap of the runs.
                Run currRun = overlapAnd(runsA[i], runsB[j]);
                if (currRun.StartIndex <= currRun.EndIndex)
                {
                    rtnVal._RunArray.Add(currRun);
                }

                // iterate the counters appropriately to compare the next set of runs for overlap.
                if (runsA[i].EndIndex > runsB[j].EndIndex)
                {
                    j += 1;
                }
                else if (runsA[i].EndIndex < runsB[j].EndIndex)
                {
                    i += 1;
                }
                else
                {
                    i += 1;
                    j += 1;
                }

            }

            return rtnVal;
        }

        public void AndWith(IBitset otherSet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a deep copy of this RLEBitset.
        /// </summary>
        public IBitset Clone()
        {
            RLEBitset rtnVal = new RLEBitset();
            Run[] tempRuns = new Run[this._RunArray.Count];
            this._RunArray.CopyTo(tempRuns);
            rtnVal._RunArray = tempRuns.ToList();
            rtnVal._Length = this._Length;
            return rtnVal;
        }

        public bool Get(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the length of the uncompressed set (i.e. 1's and 0's).
        /// </summary>
        public int Length()
        {
            return _Length;
        }

        /// <summary>
        /// Returns the number of 1's in the uncompressed set (i.e. 1's only).
        /// </summary>
        public int Cardinality()
        {
            int rtnVal = 0;
            foreach (Run r in _RunArray)
            {
                rtnVal = rtnVal + (r.EndIndex - r.StartIndex + 1);
            }
            return rtnVal;
        }

        public IBitset Or(IBitset otherSet)
        {
            //if (!(otherSet is RLEBitset))
            //{
            //    throw new ArgumentException("otherSet must be a RLEBitset to perform this operation.");
            //}
            //RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset

            //RLEBitset rtnVal = new RLEBitset(); // instantiate the return value

            //List<Run> runsA = this._RunArray;
            //List<Run> runsB = otherRLESet._RunArray;

            //int i = 0;
            //int j = 0;

            //while (i < runsA.Count && j < runsB.Count)
            //{
            //    //check for overlap of the runs.
            //    Run currRun = overlapOr(runsA[i], runsB[j]);
            //    if (currRun.StartIndex <= currRun.EndIndex)
            //    {
            //        rtnVal._RunArray.Add(currRun);
            //    }

            //}

            //return rtnVal;

            throw new NotImplementedException();
        }

        public void OrWith(IBitset otherSet)
        {
            if (!(otherSet is RLEBitset))
            {
                throw new ArgumentException("otherSet must be a RLEBitset to perform this operation.");
            }
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset

            List<Run> runsA = this._RunArray;
            List<Run> runsB = otherRLESet._RunArray;

            int i = 0;
            int j = 0;

            while (i < runsA.Count && j < runsB.Count)
            {
                //check for overlap of the runs.
                Run currRun = overlapOr(runsA[i], runsB[j]);
                if (currRun.StartIndex <= currRun.EndIndex)
                {
                    runsA[i] = currRun;

                    // the run we just created and replaced in place may encompass (partially or fully) the next run in the same array.
                    // so we loop on that array building it out until we find that it DOES NOT encompass the next run by anything.
                    while (i < runsA.Count -1 && runsA[i].EndIndex >= runsA[i+1].StartIndex)
                    {
                        if (runsA[i].EndIndex >= runsA[i + 1].EndIndex)
                        {
                            runsA.RemoveAt(i + 1);
                        }
                        else
                        {
                            runsA[i] = overlapOr(runsA[i], runsA[i + 1]);
                            runsA.RemoveAt(i + 1);
                        }
                    }

                    // the run in i'th position of runsA may have grown, so we catch up iterator j for runsB.
                    while (j < runsB.Count && runsA[i].EndIndex >= runsB[j].EndIndex)
                    {
                        j += 1;
                    }
                }

                // if there's now a gap in overlap, we can move iterator i.             
                if (j < runsB.Count && runsA[i].EndIndex - runsB[j].StartIndex < -1)
                {
                    i += 1;
                }

            }

        }

        public void Set(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetAll(bool value)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IBitset Not()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private Run overlapAnd(Run runA, Run runB)
        {
            Run rtnVal = new Run();
            rtnVal.StartIndex = (runA.StartIndex >= runB.StartIndex ? runA.StartIndex : runB.StartIndex); // take the higher START index
            rtnVal.EndIndex = (runA.EndIndex <= runB.EndIndex ? runA.EndIndex : runB.EndIndex);           // take the lower END index
            return rtnVal;
        }

        private Run overlapOr(Run runA, Run runB)
        {
            Run rtnVal = overlapAnd(runA, runB);
            //if (rtnVal.StartIndex <= rtnVal.EndIndex)
            if (rtnVal.EndIndex - rtnVal.StartIndex >= -1)
            {
                rtnVal.StartIndex = (runA.StartIndex >= runB.StartIndex ? runB.StartIndex : runA.StartIndex); // take the lower START index
                rtnVal.EndIndex = (runA.EndIndex >= runB.EndIndex ? runA.EndIndex : runB.EndIndex);           // take the higher END index
            }
            return rtnVal;
        }

        #endregion

    }
}

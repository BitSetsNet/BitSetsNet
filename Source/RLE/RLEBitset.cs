using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BitsetsNET
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
            int capacity = 0;
            if (indices.Length > 0)
            {
                capacity = indices.Max() + 1;
            }
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
            RLEBitset rtnVal = new RLEBitset();
            rtnVal._Length = capacity;

            if (indices.Length > 0)
            {

                Array.Sort(indices); // sort the input array first.
                if (indices.Last() > capacity) // capacity must be larger than highest index value in input array.
                {
                    throw new ArgumentException("capacity cannot be less than max index value");
                }

                Run currRun = new Run();
                currRun.StartIndex = indices.FirstOrDefault();
                for (int i = 0; i < indices.Length - 1; i++)
                {
                    if (indices[i + 1] - indices[i] > 1)
                    {
                        currRun.EndIndex = indices[i];
                        rtnVal._RunArray.Add(currRun);
                        currRun = new Run();
                        currRun.StartIndex = indices[i + 1];
                    }
                }
                currRun.EndIndex = indices.LastOrDefault();
                rtnVal._RunArray.Add(currRun);

            }

            return rtnVal;
        }

        #endregion

        #region "Public Methods"

        public IBitset And(IBitset otherSet)
        {
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast
            RLEBitset rtnVal = new RLEBitset(); // instantiate the return value
            rtnVal._Length = this._Length;

            List<Run> runsA = this._RunArray;
            List<Run> runsB = otherRLESet._RunArray;

            int i = 0;
            int j = 0;

            while (i < runsA.Count && j < runsB.Count)
            {
                // check for overlap of the runs.
                Run currRun = new Run();
                if (tryCreateIntersection(runsA[i], runsB[j], ref currRun))
                {
                    rtnVal._RunArray.Add(currRun);
                }

                // iterate the counters appropriately to compare the next set of runs for overlap.
                if (runsA[i].EndIndex > runsB[j].EndIndex + 1)
                {
                    j += 1;
                }
                else if (runsA[i].EndIndex < runsB[j].EndIndex - 1)
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

        /// <summary>
        /// Gets the boolean value at the given index.
        /// </summary>
        /// <param name="index">an index</param>
        /// <returns>a boolean</returns>
        public bool Get(int index)
        {
            bool rtnVal = false;
            foreach (Run r in this._RunArray)
            {
                if (index >= r.StartIndex && index <= r.EndIndex)
                {
                    rtnVal = true;
                    break;
                }
            }
            return rtnVal;
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
            //RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast

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
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast

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

        /// <summary>
        /// Inverts all the values in the current IBitset, 
        /// so that elements set to true are changed to false, and elements set to false are changed to true.
        /// </summary>
        /// <returns>a new IBitset with inverted values</returns>
        public IBitset Not()
        {
            RLEBitset rtnVal = new RLEBitset();
            rtnVal._Length = this._Length;
            if (_RunArray.Count > 0)
            {
                Run currRun = new Run();

                // handle first run if needed.
                if (_RunArray[0].StartIndex > 0)
                {
                    currRun.StartIndex = 0;
                    currRun.EndIndex = _RunArray[0].StartIndex - 1;
                    rtnVal._RunArray.Add(currRun);
                    currRun = new Run();
                }

                // handle the middle runs.
                currRun.StartIndex = _RunArray[0].EndIndex + 1;
                for (int i = 0; i < _RunArray.Count; i++)
                {
                    currRun.EndIndex = _RunArray[i + 1].StartIndex - 1;
                    rtnVal._RunArray.Add(currRun);
                    currRun = new Run();
                    currRun.StartIndex = _RunArray[i + 1].EndIndex + 1;
                }

                // handle the last run.
                if (_Length >= currRun.StartIndex)
                {
                    currRun.EndIndex = _Length - 1;
                    rtnVal._RunArray.Add(currRun);
                }

            }

            return rtnVal;
        }

        /// <summary>
        /// Determines if the other IBitset is equal to this one.
        /// </summary>
        /// <param name="otherSet">the other IBitset</param>
        /// <returns>a boolean</returns>
        public override bool Equals(object otherSet)
        {
            bool rtnVal = false;
            if ((otherSet is RLEBitset))
            {
                RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast

                if (this._Length == otherRLESet._Length &&
                this._RunArray.Count == otherRLESet._RunArray.Count)
                {

                    if (this._RunArray.Count == 0)
                    {
                        rtnVal = true;
                    }

                    for (int i = 0; i < this._RunArray.Count; i++)
                    {
                        if (this._RunArray[i].StartIndex == otherRLESet._RunArray[i].StartIndex &&
                            this._RunArray[i].EndIndex == otherRLESet._RunArray[i].EndIndex)
                        {
                            rtnVal = true;
                        }
                        else
                        {
                            rtnVal = false;
                            break;
                        }
                    }
                }

            }
            
            return rtnVal;
        }

        #endregion

        #region "Private Methods"

        private bool tryCreateIntersection(Run runA, Run runB, ref Run output) 
        {
            Run first = runA;
            Run second = runB;
            if (runA.StartIndex > runB.StartIndex)
            {
                first = runB;
                second = runA;
            }

            bool rtnVal = false;
            if (first.EndIndex >= second.StartIndex)
            {
                //overlap
                output.StartIndex = second.StartIndex;
                output.EndIndex = first.EndIndex;
                rtnVal = true;
            }
            return rtnVal;
        }

        private bool tryCreateUnion(Run runA, Run runB, ref Run output)
        {
            Run first = runA;
            Run second = runB;
            if (runA.StartIndex > runB.StartIndex)
            {
                first = runB;
                second = runA;
            }

            bool rtnVal = false;
            if (first.EndIndex >= second.StartIndex - 1)
            {
                //overlap
                output.StartIndex = first.StartIndex;
                output.EndIndex = second.EndIndex;
                rtnVal = true;
            }
            return rtnVal;
        }

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

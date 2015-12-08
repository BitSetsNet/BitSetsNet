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

            Run current = new Run();
            int nextThisIndex = 0; 
            int nextOtherIndex = 0;

            if (this._RunArray.Count == 0)
            {
                this._RunArray = new List<Run>(otherRLESet._RunArray); 
            }
            else if (otherRLESet._RunArray.Count == 0)
            {
                //do nothing    
            }
            else if (this._RunArray[0].StartIndex <= otherRLESet._RunArray[0].StartIndex)
            {
                current = this._RunArray[0];
                nextThisIndex = 1;
                nextOtherIndex = 0;
            }
            else if (tryCreateUnion(this._RunArray[0], otherRLESet._RunArray[0], ref current))
            {
                //first two sets overlap
                this._RunArray[0] = current;
                nextThisIndex = 1;
                nextOtherIndex = 1;
            }
            else
            {
                this._RunArray.Insert(0, otherRLESet._RunArray[0]);
                nextThisIndex = 1;
                nextOtherIndex = 1;
            }

            while ((nextThisIndex < this._RunArray.Count) && (nextOtherIndex < otherRLESet._RunArray.Count))
            {
                if (this._RunArray[nextThisIndex].StartIndex >
                    otherRLESet._RunArray[nextOtherIndex].StartIndex)
                {
                    mergeOtherRun(otherRLESet, ref current, ref nextThisIndex, ref nextOtherIndex);
                }
                else
                {    
                    mergeExistingRun(ref current, ref nextThisIndex);   
                }

            }

            if (nextThisIndex < this._RunArray.Count)
            {
                //we finished the other, finish this one
                while (nextThisIndex < this._RunArray.Count)
                {
                    mergeExistingRun(ref current, ref nextThisIndex);
                }
            }
            else
            {
                //we finished this one, finish the other
                while (nextOtherIndex < otherRLESet._RunArray.Count)
                {
                    mergeOtherRun(otherRLESet, ref current, ref nextThisIndex, ref nextOtherIndex);
                }
            }
        }

        private void mergeOtherRun(RLEBitset other, ref Run current, ref int nextThisIndex, ref int nextOtherIndex)
        {
            Run next = other._RunArray[nextOtherIndex];
            nextOtherIndex++;
            if (!merge(ref current, ref next, true, nextThisIndex - 1))
            {
                //no merge, so a new interval has been inserted
                nextThisIndex++;
            }
        }

        private void mergeExistingRun(ref Run current, ref int nextIndex)
        {
            Run next = this._RunArray[nextIndex];
            if (merge(ref current, ref next, false, nextIndex - 1))
            {
                //next has been merged, remove it
                _RunArray.RemoveAt(nextIndex);
            }
            else
            {
                nextIndex++;
            }
        }

        private bool merge(ref Run current, ref Run next, bool shouldInsert, int index)
        {
            bool mergedOverlappingIntervalIndicator = false;
            if (tryCreateUnion(current, next, ref current))
            {
                //union made. Replace the current in place
                this._RunArray[index] = current;
                mergedOverlappingIntervalIndicator = true;
            }
            else
            {
                current = next;
                if (shouldInsert)
                {
                    this._RunArray.Insert(index + 1, next);   
                }
            }

            return mergedOverlappingIntervalIndicator;
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
                for (int i = 0; i < _RunArray.Count - 1; i++)
                {
                    currRun.EndIndex = _RunArray[i + 1].StartIndex - 1;
                    rtnVal._RunArray.Add(currRun);
                    currRun = new Run();
                    currRun.StartIndex = _RunArray[i + 1].EndIndex + 1;
                }

                // handle the last run.
                if (_Length > currRun.StartIndex)
                {
                    currRun.EndIndex = _Length - 1;
                    rtnVal._RunArray.Add(currRun);
                }

            }
            else 
            {
                rtnVal._RunArray.Add(new Run(0, _Length - 1));
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

            int startIdx = (runA.StartIndex >= runB.StartIndex ? runA.StartIndex : runB.StartIndex); // take the higher START index
            int endIdx = (runA.EndIndex <= runB.EndIndex ? runA.EndIndex : runB.EndIndex);           // take the lower END index

            // determine if there is an intersection overlap and set return values accordingly
            bool rtnVal = false;
            if (endIdx >= startIdx)
            {
                rtnVal = true;
                output.StartIndex = startIdx;
                output.EndIndex = endIdx;
            }
            
            return rtnVal;
        }

        private bool tryCreateUnion(Run runA, Run runB, ref Run output)
        {

            int startIdx = (runA.StartIndex >= runB.StartIndex ? runA.StartIndex : runB.StartIndex); // take the higher START index
            int endIdx = (runA.EndIndex <= runB.EndIndex ? runA.EndIndex : runB.EndIndex);           // take the lower END index

            // if intersection exists, expand find the union
            bool rtnVal = false;
            if (endIdx >= startIdx - 1)
            {
                rtnVal = true;
                output.StartIndex = (runA.StartIndex >= runB.StartIndex ? runB.StartIndex : runA.StartIndex); // take the lower START index
                output.EndIndex = (runA.EndIndex >= runB.EndIndex ? runA.EndIndex : runB.EndIndex);           // take the higher END index
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

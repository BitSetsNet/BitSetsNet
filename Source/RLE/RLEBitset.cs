using System;
using System.IO;
using System.Text;
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

            internal static int serializedSizeInBytes(int numberOfRuns)
            {
                return 2 + 2 * 2 * numberOfRuns;
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
                    j++;
                }
                else if (runsA[i].EndIndex < runsB[j].EndIndex - 1)
                {
                    i++;
                }
                else
                {
                    i++;
                    j++;
                }

            }

            return rtnVal;
        }

        /// <summary>
        /// Intersects an IBitset with another IBitset, modifying the first IBitset rather 
        /// than creating a new IBitset
        /// </summary>
        /// <param name="otherSet">the other IBitset</param>
        /// <returns>void</returns>
        public void AndWith(IBitset otherSet)
        {
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset

            List<Run> runsA = this._RunArray;
            List<Run> runsB = otherRLESet._RunArray;
             
            int i = 0;
            int j = 0;

            while (i < runsA.Count && j < runsB.Count)
            {
                int x = runsA[i].StartIndex;
                int y = runsA[i].EndIndex;
                int w = runsB[j].StartIndex;
                int z = runsB[j].EndIndex;

                if (x < w)
                {
                    if (y < w)
                    {
                        runsA.RemoveAt(i);
                    }
                    else // (y >= w)
                    {
                        // crops the current run in runsA from the left to align with 
                        // the start of the current run in runsB
                        Run ithRun = runsA[i];
                        ithRun.StartIndex = w;
                        runsA[i] = ithRun;
                        var what = this._RunArray[i];
                        if (y <= z)
                        {
                            i++;
                        }
                        else // (y > z )
                        {
                            // splits the run from runsA into two runs
                            Run newRun =  new Run(z + 1, y);
                            Run newRun2 = runsA[i];
                            newRun2.EndIndex = z;
                            runsA[i] = newRun2;
                            runsA.Insert(i + 1, newRun); 
                            i++;
                            j++;
                        }
                    }
                }
                else // (x >= w)
                {
                    if (y <= z)
                    {
                        i++;
                    }
                    else // (y > z)
                    {
                        if (x <= z)
                        {
                            // splits the run from runsA into two runs
                            Run newRun = new Run(z + 1, y);
                            Run newRun2 = runsA[i];
                            newRun2.EndIndex = z;
                            runsA[i] = newRun2;
                            runsA.Insert(i + 1, newRun); 
                            i++;
                            j++;
                        }
                        else 
                        {
                            j++;
                        }
                    }
                }
            }
            //this truncates runsA if we've considered all of the runs in runsB
            this._RunArray = this._RunArray.Take(i).ToList();
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
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast

            RLEBitset rtnVal = new RLEBitset(); // instantiate the return value
            rtnVal._Length = (this._Length > otherRLESet._Length) ? this._Length : otherRLESet._Length;

            List<Run> runsA = this._RunArray;
            List<Run> runsB = otherRLESet._RunArray;

            int i = 0;
            int j = 0;

            while (i < runsA.Count && j < runsB.Count)
            {
                Run currRun = new Run();
                if (tryCreateUnion(runsA[i], runsB[j], ref currRun))
                {
                    //there is an overlap
                    //now compare the overlapping run you created to the previous run you added to see if they should be merged
                    addRunToRLE(ref rtnVal, currRun);

                    //now move the counters.
                    if (runsA[i].EndIndex < runsB[j].EndIndex)
                    {
                        i++;
                    }
                    else if (runsA[i].EndIndex > runsB[j].EndIndex)
                    {
                        j++;
                    }
                    else
                    {
                        i++;
                        j++;
                    }
                    
                }
                else
                {
                    //no overlap, so let's just add lower run and step that counter.
                    if (runsA[i].StartIndex < runsB[j].StartIndex)
                    {
                        addRunToRLE(ref rtnVal, runsA[i]);
                        i++;
                    }
                    else
                    {
                        addRunToRLE(ref rtnVal, runsB[j]);
                        j++;
                    }
                }

            }

            //account for remaining runs in one of our sets.
            int remCounter = 0;
            List<Run> remRuns = new List<Run>();

            if (i < runsA.Count)
            {
                remCounter = i;
                remRuns = runsA;
            }
            else if (j < runsB.Count)
            {
                remCounter = j;
                remRuns = runsB;
            }

            while (remCounter < remRuns.Count)
            {
                addRunToRLE(ref rtnVal, remRuns[remCounter]);
                remCounter++;
            }

            return rtnVal;

        }

        /// <summary>
        /// Helper function for Or operations. 
        /// Adds the given run to the run-array by. Either:
        ///  a) merges it with the previous run if overlap with previous in array 
        ///  b) adds it as next run if no overlap with previous in array
        /// </summary>
        /// <param name="currRLE">the RLE to be modified</param>
        /// <param name="runToAdd">the Run to add</param>
        private void addRunToRLE(ref RLEBitset currRLE, Run runToAdd)
        {
            Run currRun = new Run();
            if (tryCreateUnion(currRLE._RunArray.LastOrDefault(), runToAdd, ref currRun) && currRLE._RunArray.Count > 0)
            {
                //there is overlap with the previous run in run-array so we merge this run with the previous in the array.
                int tmpIndx = currRLE._RunArray.Count - 1;
                currRLE._RunArray[tmpIndx] = currRun;
            }
            else
            {
                //no overlap with previous run in run-array, so we add the overlapping run as is.
                currRLE._RunArray.Add(runToAdd);
            }
        }

        public void OrWith(IBitset otherSet)
        {
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast

            Run current = new Run();
            int nextThisIndex, nextOtherIndex; 
 
            if (this._RunArray.Count == 0)
            {
                this._RunArray = new List<Run>(otherRLESet._RunArray);
                this._Length = otherRLESet._Length;
                nextThisIndex = this._RunArray.Count; //this stops the loops 
                nextOtherIndex = this._RunArray.Count; 
                
            }
            else if (otherRLESet._RunArray.Count == 0)
            {
                nextThisIndex = this._RunArray.Count; //this stops the loops
                nextOtherIndex = this._RunArray.Count; 
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
                    int lastNextIndex = this._RunArray.Count;
                    mergeOtherRun(otherRLESet, ref current, ref lastNextIndex, ref nextOtherIndex);
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

        /// <summary>
        /// Performs the set difference, defined as the set in A and not in B.
        /// </summary>
        /// <param name="otherSet">the other IBitset</param>
        /// <returns>The set difference of this set and the other.</returns>
        public IBitset Difference(IBitset otherSet)
        {
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast
            IBitset rtnVal = And(otherRLESet.Not());
            return rtnVal;
        }

        /// <summary>
        /// Performs the set difference, defined as the set in A and not in B.
        /// </summary>
        /// <param name="otherSet">the other IBitset</param>
        public void DifferenceWith(IBitset otherSet)
        {
            RLEBitset otherRLESet = (RLEBitset)otherSet; // cast to an RLEBitset - errors if cannot cast
            AndWith(otherRLESet.Not());
        }

        /// <summary>
        /// Sets the bit at a specific position in the IBitset to the specified value.
        /// </summary>
        /// <param name="index">The zero-based index of the bit to set.</param>
        /// <param name="value">The Boolean value to assign to the bit.</param>
        public void Set(int index, bool value)
        {
            int[] tmpIndices = { index };
            IBitset other = RLEBitset.CreateFrom(tmpIndices);
            if (value)
            {
                OrWith(other);
            }
            else
            {
                DifferenceWith(other);
            }

        }

        /// <summary>
        /// For indices in the range [start, end] add the index to the set if
        /// the value is true, otherwise remove it.
        /// </summary>
        /// <param name="start">the index to start from (inclusive)</param>
        /// <param name="end">the index to stop at (exclusive)</param>
        public void Set(int start, int end, bool value)
        {
            RLEBitset other = new RLEBitset();
            other._RunArray.Add(new Run(start, end - 1));
            if (value)
            {
                OrWith(other);
            }
            else
            {
                DifferenceWith(other);
            }
        }

        /// <summary>
        /// Sets all bits in the given range to the specified value.
        /// </summary>
        /// <param name="startIndex">The zero-based start position of the range.</param>
        /// <param name="count">The number of bits in the range.</param>
        /// <param name="value">The Boolean value to assign to the bits.</param>
        public void SetRange(int startIndex, int count, bool value)
        {
            RLEBitset other = new RLEBitset();
            other._RunArray.Add(new Run(startIndex, startIndex + count - 1));
            if (value)
            {
                OrWith(other);
            }
            else
            {
                DifferenceWith(other);
            }
        }

        /// <summary>
        /// Flips the bit at the specified index.
        /// </summary>
        /// <param name="index">Index to be flipped</param>
        public void Flip(int index)
        {
            if (_RunArray.Count == 0)
            {
                _RunArray.Add(new Run(index, index));
            }
            else
            {
                if (this.Get(index) == true)
                {
                    this.Set(index, false);
                }
                else
                {
                    this.Set(index, true);
                }
            }
        }

        /// <summary>
        /// Flips each bit in the specified range
        /// </summary>
        /// <param name="start">Start of range</param>
        /// <param name="end">End of range</param>
        public void Flip(int start, int end)
        {
            if (end < start)
            {
                throw new ArgumentException("parameter 'end' must be greater than or equal to 'start'");
            }

            if (_RunArray.Count == 0)
            {
                _RunArray.Add(new Run(start, end));
            }
            else
            {
                List<Run> rangeToFlip = this.getRange(start, end);

                this.Set(rangeToFlip[0].StartIndex, rangeToFlip[0].EndIndex, false);

                int rangeCount = rangeToFlip.Count;
                for (int i = 1; i < rangeCount; i++)
                {
                    this.Set(rangeToFlip[i].StartIndex, rangeToFlip[i].EndIndex, false);
                    this.Set(rangeToFlip[i - 1].EndIndex + 1, rangeToFlip[i].StartIndex - 1, true);
                }

                if (start < rangeToFlip[0].StartIndex)
                {
                    this.Set(start, rangeToFlip[0].StartIndex - 1, true);
                }

                if (end > rangeToFlip[rangeCount - 1].EndIndex)
                {
                    this.Set(rangeToFlip[rangeCount - 1].EndIndex + 1, end, true);
                }
            }
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


        /// <summary>
        /// Returns the contents of this set as a bit array where the value is
        /// set to true for each index that is a member of this set
        /// </summary>
        /// <returns>a new BitArray</returns>
        public BitArray ToBitArray()
        {
            int arrayLength = 0;
            if (this._RunArray.Count > 0)
            {
                arrayLength = this._RunArray.Last().EndIndex + 1;
            }

            BitArray rtnVal = new BitArray(arrayLength, false);

            foreach (Run r in this._RunArray)
            {
                for (int i = r.StartIndex; i < r.EndIndex + 1; i++)
                {
                    rtnVal[i] = true;
                }
            }

            return rtnVal;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Write a binary serialization of this RLE bitset.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void Serialize(Stream stream)
        {
            //We don't care about the encoding, but we have to specify something to be able to set the stream as leave open.
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Default, true))
            {
                writer.Write(this._Length);
                foreach (Run r in this._RunArray)
                {
                    writer.Write(r.StartIndex);
                    writer.Write(r.EndIndex);
                }
            }
        }

        /// <summary>
        /// Read a binary serialization of a RLE bitset, as written by the Serialize method.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The bitset deserialized from the stream.</returns>
        public static RLEBitset Deserialize(Stream stream)
        {
            RLEBitset bitset = new RLEBitset();

            //We don't care about the encoding, but we have to specify something to be able to set the stream as leave open.
            using (BinaryReader reader = new BinaryReader(stream, Encoding.Default, true))
            {
                bitset._Length = reader.ReadInt32();
                while (stream.Position < stream.Length - 1)
                {
                    Run currRun = new Run();
                    currRun.StartIndex = reader.ReadInt32();
                    currRun.EndIndex = reader.ReadInt32();
                    bitset._RunArray.Add(currRun);
                }
            }
            return bitset;
        }
		
		/// <summary>
        /// Get an enumerator of the set indices of this bitset. 
        /// Meaning, it returns the indicies where the value is set to "true" or "1".
        /// </summary>
        /// <returns>A enumerator giving the set (i.e. for which the bit is '1' or true) indices for this bitset.</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (Run r in this._RunArray)
            {
                for (int i = r.StartIndex; i < r.EndIndex +1; i++)
                {
                    yield return i;
                }
            }
        }
		
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = _Length;
                foreach (var run in _RunArray)
                {
                    hash = unchecked(17 * hash + run.StartIndex);
                    hash = unchecked(17 * hash + run.EndIndex);
                }
                return hash;
			}
		}
        #endregion

        #region "Private Methods"

        private List<Run> getRange(int start, int end)
        {
            List<Run> range = new List<Run>();
            for (int i = 0; i < _RunArray.Count; i++)
            {
                if(_RunArray[i].StartIndex >= start && _RunArray[i].EndIndex <= end)
                {
                    range.Add(_RunArray[i]);
                }
                else if(_RunArray[i].StartIndex < start && _RunArray[i].EndIndex <= end)
                {
                    range.Add(new Run(start, _RunArray[i].EndIndex));
                }
                else if(_RunArray[i].StartIndex < start && _RunArray[i].EndIndex > end)
                {
                    range.Add(new Run(start, end));
                    break;
                }
                else if(_RunArray[i].StartIndex >= start && _RunArray[i].EndIndex > end)
                {
                    range.Add(new Run(_RunArray[i].StartIndex, end));
                    break;
                }
            }

            return range;
        }


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

        #endregion 

    }
}

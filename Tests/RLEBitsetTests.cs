using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsetsNET.Tests
{
    [TestClass()]
    public class RLEBitsetTests : BaseBitSetTests
    {
        protected override IBitset CreateSetFromIndices(int[] indices, int length)
        {
            return RLEBitset.CreateFrom(indices, length);
        }

        [TestMethod()]
        public virtual void SerializationTest()
        {
            int TEST_SET_LENGTH = 10;
            int[] indicies = SetGenerator.GetRandomArray(TEST_SET_LENGTH);

            RLEBitset actual = (RLEBitset)CreateSetFromIndices(indicies, TEST_SET_LENGTH);
            RLEBitset expected = null;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                actual.Serialize(ms);
                ms.Position = 0;
                expected = RLEBitset.Deserialize(ms);
            }

            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public virtual void ToBitArrayTest()
        {
            int TEST_SET_LENGTH = 10;
            int[] set = SetGenerator.GetRandomArray(TEST_SET_LENGTH);
            BitArray setArray = new BitArray(TEST_SET_LENGTH);

            foreach (int index in set)
            {
                setArray[index] = true;
            }

            RLEBitset testSet = (RLEBitset)CreateSetFromIndices(set, TEST_SET_LENGTH);
            BitArray testArray = testSet.ToBitArray();

            bool expected = true;
            bool actual = true;

            for (int i = 0; i < setArray.Length; i++)
            {
                if (setArray[i])
                {
                    actual &= setArray[i] == testArray[i];
                }
                else
                {
                    //do nothing
                }
            }

            Assert.AreEqual(expected, actual);
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            System.IO.FileStream fileWriter = new System.IO.FileStream(@"C:\Development\RLE.bin", System.IO.FileMode.Create);
            actual.Serialize(fileWriter);
            fileWriter.Close();
            fileWriter.Dispose();

            System.IO.FileStream fileReader = new System.IO.FileStream(@"C:\Development\RLE.bin", System.IO.FileMode.Open);
            RLEBitset expected = RLEBitset.Deserialize(fileReader);
            fileReader.Close();
            fileReader.Dispose();

            Assert.AreEqual(actual, expected);

        }

    }
}

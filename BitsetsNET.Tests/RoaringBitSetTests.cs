using System.Linq;
using Xunit;

namespace BitsetsNET.Tests
{
    public class RoaringBitSetTests : BaseBitSetTests 
    {
        protected override IBitset CreateSetFromIndicies(int[] indices, int length)
        {
            return RoaringBitset.Create(indices);
        }

        [Fact]
        public virtual void SetTrueLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(9, 5009);
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(8, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.Equal(expected, result);
        }

        [Fact]
        public virtual void SetFalseLarge()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(2, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public virtual void SetRangeTrueLargeTest()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000);
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(5007, 5009, true);
            bool expected = true;
            bool result = testSet.Get(8);
            Assert.Equal(expected, result);
        }

        [Fact]
        public virtual void SetRangeFalseLargeTest()
        {
            int[] set = SetGenerator.GetContiguousArray(0, 5000); ;
            IBitset testSet = CreateSetFromIndicies(set, 5000);
            testSet.Set(1, 3, false);
            bool expected = false;
            bool result = testSet.Get(2);
            Assert.Equal(expected, result);
        }

        /*
         The roaring bit set stores 2 different types of containers. 
         The switch is based primarily on size, Array (small) and Bitset (large).
         We want to ensure logic works between large and small containers.
        */

        [Fact]
        public virtual void VariedSizeOrTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(smallSize);
            int[] result = first.Union(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, largeSize);
            IBitset actual = CreateSetFromIndicies(first, largeSize).Or(CreateSetFromIndicies(second, smallSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void LargeSetOrTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(largeSize);
            int[] result = first.Union(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, largeSize);
            IBitset actual = CreateSetFromIndicies(first, largeSize).Or(CreateSetFromIndicies(second, largeSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void VariedSizeOrWithTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(smallSize);
            int[] result = first.Union(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, largeSize);
            testSet.OrWith(CreateSetFromIndicies(second, smallSize));

            Assert.Equal(CreateSetFromIndicies(result, largeSize), testSet);
        }

        [Fact]
        public virtual void LargeSetOrWithTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(largeSize);
            int[] result = first.Union(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, largeSize);
            testSet.OrWith(CreateSetFromIndicies(second, largeSize));

            Assert.Equal(CreateSetFromIndicies(result, largeSize), testSet);
        }

        [Fact]
        public virtual void VariedSizeAndTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(smallSize);
            int[] result = first.Intersect(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, largeSize);
            IBitset actual = CreateSetFromIndicies(first, largeSize).And(CreateSetFromIndicies(second, smallSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void LargeSetAndTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(largeSize);
            int[] result = first.Intersect(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, largeSize);
            IBitset actual = CreateSetFromIndicies(first, largeSize).And(CreateSetFromIndicies(second, largeSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void VariedSizeAndWithTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(smallSize);
            int[] result = first.Intersect(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, largeSize);
            testSet.AndWith(CreateSetFromIndicies(second, smallSize));

            Assert.Equal(CreateSetFromIndicies(result, largeSize), testSet);
        }

        [Fact]
        public virtual void LargeSetAndWithTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(largeSize);
            int[] result = first.Intersect(second).ToArray();
            IBitset testSet = CreateSetFromIndicies(first, largeSize);
            testSet.AndWith(CreateSetFromIndicies(second, largeSize));

            Assert.Equal(CreateSetFromIndicies(result, largeSize), testSet);
        }

        [Fact]
        public virtual void VariedSizeDifferenceTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(smallSize);
            int[] result = first.Except(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, result.Length);
            IBitset actual = CreateSetFromIndicies(first, largeSize).Difference(CreateSetFromIndicies(second, smallSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void LargeSetDifferenceTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetRandomArray(largeSize);
            int[] second = SetGenerator.GetRandomArray(largeSize);
            int[] result = first.Except(second).ToArray();
            IBitset expected = CreateSetFromIndicies(result, result.Length);
            IBitset actual = CreateSetFromIndicies(first, largeSize).Difference(CreateSetFromIndicies(second, largeSize));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public virtual void VariedSizeOverlapsWithSharedElementsTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetContiguousArray(50, largeSize + 50);
            int[] second = SetGenerator.GetContiguousArray(0, smallSize);
            bool actual = CreateSetFromIndicies(first, largeSize).Overlaps(CreateSetFromIndicies(second, smallSize));

            Assert.True(actual);
        }

        [Fact]
        public virtual void LargeSetOverlapsWithSharedElementsTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetContiguousArray(50, largeSize + 50);
            int[] second = SetGenerator.GetContiguousArray(0, largeSize);
            bool actual = CreateSetFromIndicies(first, largeSize).Overlaps(CreateSetFromIndicies(second, largeSize));

            Assert.True(actual);
        }

        [Fact]
        public virtual void VariedSizeOverlapsWithNoSharedElementsTest()
        {
            int largeSize = 100000;
            int smallSize = 100;
            int[] first = SetGenerator.GetContiguousArray(101, largeSize + 101);
            int[] second = SetGenerator.GetContiguousArray(0, smallSize);
            bool actual = CreateSetFromIndicies(first, largeSize).Overlaps(CreateSetFromIndicies(second, smallSize));

            Assert.False(actual);
        }

        [Fact]
        public virtual void LargeSetOverlapsWithNoSharedElementsTest()
        {
            int largeSize = 100000;
            int[] first = SetGenerator.GetContiguousArray(0, largeSize);
            int[] second = SetGenerator.GetContiguousArray(largeSize + 1, largeSize * 2);
            bool actual = CreateSetFromIndicies(first, largeSize).Overlaps(CreateSetFromIndicies(second, largeSize));

            Assert.False(actual);
        }

        [Fact(Skip = "Method Not Implemented")]
        public override void ToBitArrayTest()
        {
            // Pass
        }
    }
}

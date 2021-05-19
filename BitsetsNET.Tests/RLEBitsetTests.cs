using Xunit;

namespace BitsetsNET.Tests
{
    public class RLEBitsetTests : BaseBitSetTests
    {
        protected override IBitset CreateSetFromIndicies(int[] indices, int length)
        {
            return RLEBitset.CreateFrom(indices, length);
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void FlipFalseTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void FlipRangeFalseTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void FlipTrueTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void FlipRangeTrueTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void OrTest()
        {
            // Pass
            // Note: The code for the implementation is commented out
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void SetFalseTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void SetRangeFalseTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void SetRangeTrueTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void SetTrueTest()
        {
            // Pass
        }
        
        [Fact(Skip = "Method Not Implemented")]
        public override void ToBitArrayTest()
        {
            // Pass
        }
    }
}

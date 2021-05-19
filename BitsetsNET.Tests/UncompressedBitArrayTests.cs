namespace BitsetsNET.Tests
{
    public class UncompressedBitArrayTests : BaseBitSetTests
    {

        protected override IBitset CreateSetFromIndicies(int[] indices, int length)
        {
            return new UncompressedBitArray(indices, length);
        }
    }
}
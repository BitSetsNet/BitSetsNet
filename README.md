# BitSetsNet
A .NET library for compressed bit set data structures. We have currently included implementations for Run-Length Encoding and Roaring compression algorithms. 

## Installation
You can download the latest release packages from NuGet: https://www.nuget.org/packages/BitsetsNET/ or from GitHub: https://github.com/BitSetsNet/BitSetsNet/releases

## Usage

```
using BitsetsNET;

namespace BitSetUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] indicies1 = new int[] {3,6,10,200,1500};
            RoaringBitset rbs1 = RoaringBitset.Create(indicies1);
            int[] indicies2 = new int[] { 3, 9, 10, 250, 1700};
            RoaringBitset rbs2 = RoaringBitset.Create(indicies2);

            RoaringBitset andResult = RoaringBitset.and(rbs1, rbs2); //Creates a new Bitset
            rbs1.OrWith(rbs2); //Modifies the bitset in-place

            //iterate through the bitset
            foreach (int i in rbs1)
            {
                System.Console.WriteLine(i);
            }
        }
    }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push your feature branch
5. Submit a pull request

## References
Samy Chambi, Daniel Lemire, Owen Kaser, Robert Godin, Better bitmap performance with Roaring bitmaps, Software: Practice and Experience Volume 46, Issue 5, pages 709–719, May 2016 http://arxiv.org/abs/1402.6407 This paper used data from http://lemire.me/data/realroaring2014.html

## Credits

The implementation for the Roaring algorithm is a port from the Roaring Bitmap Java library: https://github.com/RoaringBitmap/RoaringBitmap

using System;
using Chaow.Extensions;

namespace Chaow.Numeric.Sequence
{
    //RandomSequence allows you to generate random integer sequence
    public static class RandomSequence_
    {
        public static void RandomSeq()
        {
            //this example shows how to create a random sequence

            //you can create RandomSequence by new Random(seed, minValue, maxValue)
            //if you omit seed, random seed will be assumed
            //if you omit minValue, 0 will be assumed
            //and if you omit maxValue, int.MaxValue will be assumed
            //below is create random integer sequence for dice (1-6)
            var dice = new RandomSequence(0, 6);
            Console.WriteLine(dice.Take(25).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("same object of RandomSequence will generate the same sequence over time");
            Console.WriteLine(dice.Take(25).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("different seed number of RandomSequence will generate different sequence");
            Console.WriteLine(new RandomSequence(0, 6).Take(25).ToString(", "));
        }

        public static void RandomSeq_Members()
        {
            //this example shows members of random sequence

            //here are members of random sequence
            var dice = new RandomSequence(0, 6);
            Console.WriteLine("Sequence is {0}", dice.Take(25).ToString(", "));
            Console.WriteLine("Seed is {0}", dice.Seed);
            Console.WriteLine("MinValue is {0}", dice.MinValue);
            Console.WriteLine("MaxValue is {0}", dice.MaxValue);
            Console.WriteLine("You can shuffle RandomSequence with randomSeq.Shuffle");
            dice.Shuffle();
            Console.WriteLine("Now sequence is {0}", dice.Take(25).ToString(", "));
            Console.WriteLine("Now seed is {0}", dice.Seed);
        }
    }
}
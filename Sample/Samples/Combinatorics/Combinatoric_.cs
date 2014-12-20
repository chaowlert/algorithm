using System;
using System.Collections.Generic;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    //Combinatoric is utility class for Combinatorics namespace
    public static class Combinatoric_
    {
        public static void RandomSingle()
        {
            //this example shows how to get a random item from collection

            //create collection
            var list = 1.To(10);

            //use collection.RandomSingle() to get a random item
            Console.WriteLine("Get a random item from 1-10");
            Console.WriteLine(list.RandomSingle());
        }

        public static void RandomSamples()
        {
            //this example shows how to get random sample set from collection

            //create collection
            var list = 1.To(100);

            //use collection.RandomSample(length, repetition) to get random sample set
            //length is length of the sample
            //repetition is whether you allow repetition in sample
            //you can omit repetition, if you omit repetition, false will be assumed
            Console.WriteLine(list.RandomSamples(10, false).ToString(", "));
        }

        public static void Factorial()
        {
            //this example shows how to get factorial

            //use Combinatoric.Factorial(n) for n!
            Console.WriteLine(Combinatoric.Factorial(10));
        }

        public static void FallingFactorial()
        {
            //this example shows how to get falling factorial

            //use Combinatoric.FallingFactorial(n, r) for n!/r!
            Console.WriteLine(Combinatoric.FallingFactorial(10, 5));
        }

        public static void Choose()
        {
            //this example shows how to get binomial

            //use Combinatoric.Choose(n, k) for n!/(k!*(n-k)!)
            Console.WriteLine(Combinatoric.Choose(10, 5));
        }

        public static void Multinomial()
        {
            //this example shows how to get multinomial

            //use Combinatoric.Multinomial(collection_of_x) to get multinomial
            Console.WriteLine(Combinatoric.Multinomial(new[] {1, 4, 4, 2}));
        }
    }
}
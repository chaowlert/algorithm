using System;
using System.Linq;

//Chaow.Numeric.Analysis is collection of advance mathematical methods

namespace Chaow.Numeric.Analysis
{
    //ContinuedFraction represents sequence of rationals toward a value
    public static class ContinuedFraction_
    {
        public static void Create()
        {
            //this example shows how to create ContinuedFraction

            //you can create continued fraction by new ContinuedFraction(x, y, z)
            //where formula is (Sqrt(x) + y) / z
            //if you omit y, 0 will be assumed
            //if you omit z, 1 will be assumed
            var sqrtOf2 = new ContinuedFraction(2);

            //show result
            Console.WriteLine(sqrtOf2);
        }

        public static void Sequence()
        {
            //this example shows how to iterate ContinuedFraction

            //create ContinuedFraction
            var sqrtOf2 = new ContinuedFraction(2);

            //ContinuedFraction derived from BaseSequence you can iterate as sequence
            foreach (var r in sqrtOf2.Take(10))
                Console.WriteLine("{0:0.000000}: {1}", (double)r, r);
        }

        public static void Constant()
        {
            //this example shows how to get ContinuedFraction of Phi and E

            //you can get Phi by ContinuedFraction.Phi
            var phi = ContinuedFraction.Phi;

            //you can get E by ContinuedFraction.E
            var e = ContinuedFraction.E;

            //show results
            Console.WriteLine("Phi = {0}", phi);
            Console.WriteLine("E = {0}", e);
        }

        public static void Sample_Sqrt_of_2()
        {
            //this example shows how to use ContinuedFraction to solve problem

            //In the first 1000 expansions of sqrt of 2, 
            //how many fractions contain a numerator with more digits than denominator?
            //(Question from http://projecteuler.net)
            var sqrtOf2 = new ContinuedFraction(2);

            Console.WriteLine(sqrtOf2.Take(1001).Count(r => r.Numerator.ToString().Length > r.Denominator.ToString().Length));
        }

        public static void Sample_E()
        {
            //this example shows how to use ContinuedFraction to solve problem

            //Find the sum of digits in the numerator of the 100th convergent of the continued fraction for e
            //(Question from http://projecteuler.net)
            var e = ContinuedFraction.E;

            Console.WriteLine(e.ElementAt(99).Numerator.ToString().Sum(c => c - '0'));
        }
    }
}
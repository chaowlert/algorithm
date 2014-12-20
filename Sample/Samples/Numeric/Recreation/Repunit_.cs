using System;
using System.Linq;
using Chaow.Numeric.Sequence;

//Chaow.Numeric.Recreation is collection of less useful numeric types

namespace Chaow.Numeric.Recreation
{
    //Repunit represents number contains only digit 1
    public static class Repunit_
    {
        public static void Create()
        {
            //this example shows how to create Repunit

            //create Repunit by new Repunit(length)
            var repunit = new Repunit(4);

            //show result
            Console.WriteLine(repunit);
        }

        public static void CreateDivisibleBy()
        {
            //this example shows how to create least value repunit which is divisible by a number

            //create Repunit by Repunit.CreateDivisibleBy(num)
            var repunit = Repunit.CreateDivisibleBy(7);

            //show result
            Console.WriteLine("{0}/7 = {1}", repunit, int.Parse(repunit.ToString()) / 7);
        }

        public static void IsDivisibleBy()
        {
            //this example shows if the repunit is divisible by a number

            //create Repunit
            var repunit = new Repunit(10);

            //use repunit.IsDivisibleBy(num) to get if the repunit is divisibly by a number
            Console.WriteLine("Is {0} divisible by 441? {1}", repunit, repunit.IsDivisibleBy(441));
            Console.WriteLine("Is {0} divisible by 451? {1}", repunit, repunit.IsDivisibleBy(451));
        }

        public static void Length()
        {
            //this example shows how to get repunit length

            //create Repunit by Repunit.CreateDivisibleBy(num)
            var repunit = Repunit.CreateDivisibleBy(7);

            //use repunit.Length to get length
            Console.WriteLine("{0} has length of {1}", repunit, repunit.Length);
        }

        public static void Sample_Repunit_Factors()
        {
            //this example shows how to use repunit to solve problem

            //Determining sum of the first forty prime factors of a repunit length of 10^9
            //(Question from http://projecteuler.net)
            var repunit = new Repunit(10L.Power(9));
            Console.WriteLine(new Prime().TakeWhile(p => true).Where(repunit.IsDivisibleBy).Take(40).Sum());
        }
    }
}
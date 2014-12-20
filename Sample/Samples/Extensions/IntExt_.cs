using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    //IntExt makes you use integer more powerful
    public static class IntExt_
    {
        public static void Number_Range()
        {
            //this example shows how to create number range

            //number.To(number2) creates a number range
            var result = 1.To(10);

            //show results
            Console.WriteLine(result.ToString(", "));
        }

        public static void Number_Decreasing_Range()
        {
            //this example shows how to create decreasing number range

            //number.DownTo(number2) creates number in decreasing range
            var result = 10.DownTo(1);

            //show results
            Console.WriteLine(result.ToString(", "));
        }

        public static void Number_Stepping_Range()
        {
            //this example shows how to create stepping number range

            //number.StepTo(number2,step) is to create number in stepping range
            var result = 1.StepTo(20, 2);

            //show results
            Console.WriteLine(result.ToString(", "));
        }

        public static void Times()
        {
            //this example shows how easy to run action for specified times

            //n.Times(action) runs action for n times
            10.Times(i => Console.WriteLine("I love you, {0}", i));
        }

        public static void Sample_Add_Number()
        {
            //this example shows how to use number range to solve problem

            //Add all the natural numbers below 1000 that are multiples of 3 or 5
            //(Question from http://projecteuler.net)
            Console.WriteLine((from x in 1.To(999)
                               where (x % 3 == 0) || (x % 5 == 0)
                               select x).Sum()
                );
        }
    }
}
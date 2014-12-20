using System;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Numeric.Sequence
{
    //Fibonacci allows you to generate fibonacci numbers
    public static class Fibonacci_
    {
        public static void Fibonacci()
        {
            //this example shows sample of fibonacci numbers

            //formula for fibonacci number
            //fibo(n) = fibo(n - 1) + fibo(n - 2)
            //fibo(0) = 0
            //fibo(1) = 1
            //here is the first 25 fibonacci numbers
            Console.WriteLine(new Fibonacci().Take(25).ToString(", "));
        }

        public static void Sample_Fibonacci_1()
        {
            //this example show how to use Fibonacci to solve problem

            //Find the sum of all the even-valued terms in the Fibonacci sequence which do not exceed one million
            //(Question from http://projecteuler.net)
            Console.WriteLine(
                new Fibonacci().TakeWhile(x => x < 1000000).Where(x => x % 2 == 0).Aggregate((x, y) => x + y)
                );
        }

        public static void Sample_Fibonacci_2()
        {
            //this example show how to use Fibonacci to solve problem

            //What is the first term in the Fibonacci sequence to contain 1000 digits
            //(Question from http://projecteuler.net)
            Console.WriteLine(
                new Fibonacci().TakeWhile(x => x.ToString().Length < 1000).Count()
                );
        }
    }
}
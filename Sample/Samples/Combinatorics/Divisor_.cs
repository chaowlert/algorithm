using System;
using System.Linq;
using Chaow.Extensions;
using Chaow.Numeric.Sequence;

namespace Chaow.Combinatorics
{
    //Divisor allows you to get divisors of a number
    public static class Divisor_
    {
        public static void Divisor_Create()
        {
            //this example shows how to create divisor

            //use prime.Divisors(number) to get divisor
            var divisors = new Prime().Divisors(36);

            //show results
            Console.WriteLine("Divisors of 36 are {0}", divisors.ToString(", "));
        }

        public static void Divisor_As_Collection()
        {
            //this example shows you can use Count/Contains on divisor

            //create divisor
            var divisors = new Prime().Divisors(36);

            //use divisor.Count to get number of divisors
            //this class has algorithm to do fast counting
            Console.WriteLine("Number of divisors of 36 is {0}", divisors.Count);

            //use divisor.Contains(item) to get whethers item is in the collection
            //this class has algorithm to do fast getting contain
            Console.WriteLine("Does divisors of 36 contain 12?");
            Console.WriteLine(divisors.Contains(12));
        }

        public static void Divisor_As_List()
        {
            //this example shows you can use get_Item/IndexOf on divisor

            //create divisor
            var divisors = new Prime().Divisors(36);

            //use divisor[index] to get item from index
            //this class has algorithm to do fast getting item from index
            Console.WriteLine("4th divisor of 36 is {0}", divisors[4]);

            //use divisor.IndexOf(item) to get index from the item
            //this class has algorithm to get fast index of
            Console.WriteLine("Index of 6 in divisor of 36 is {0}", divisors.IndexOf(6));
        }

        public static void Divisor_Sigma()
        {
            //this example shows how to get divisor sigma

            //create divisor
            var divisors = new Prime().Divisors(36);

            //use divisor.Sigma(k) to get divisor sigma
            Console.WriteLine("Sigma 0 of 36 is {0}", divisors.Sigma(0));
            Console.WriteLine("Sigma 1 of 36 is {0}", divisors.Sigma(1));
            Console.WriteLine("Sigma 2 of 36 is {0}", divisors.Sigma(2));
        }

        public static void Sample_Amicable_Pairs()
        {
            //this example shows how to use divisor to solve problem

            //Evaluate the sum of all the amicable numbers under 10000
            //Question from http://projecteuler.net

            //Amicable pair is pair of n and m, which n != m
            //and sum of divisors of n less than n is m
            //and sum of divisors of m less than m is n

            //create prime
            var prime = new Prime();

            //create function
            Func<long, bool> isAmicable = n =>
            {
                var divisors = prime.Divisors(n);
                var m = divisors.Sum() - n;
                return m != n && prime.Divisors(m).Sum() - m == n;
            };

            //show results
            long result = (from n in 2.To(9999)
                           where isAmicable(n)
                           select n).Sum();
            Console.WriteLine(result);
        }
    }
}
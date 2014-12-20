using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    //FuncExt makes you write functional programming easier
    public static class FuncExt_
    {
        public static void Func_With_Anonymous_Parameters()
        {
            //this example shows how to create function with anonymous type parameters

            //FuncExt.Create will create new function which able to pass anonymous type as parameter
            //In this example, type of function will be Func<AnoymousType_of_Name_and_Age, string>
            var repeatNameByAge = FuncExt.Create(new {Name = default(string), Age = default(int)}, person => person.Name.Repeat(person.Age));

            //pass anonymous type as parameter
            Console.WriteLine(repeatNameByAge(new {Name = "John", Age = 20}));
        }

        public static void Func_Selective()
        {
            //this example shows how to create alternative function based on parameter

            //create recursive function
            Func<int, int> fact = null;

            //assign factorial
            fact = x => x * fact(x - 1);

            //func.When to create different function if condition is met
            //below is, when x equal to 0, return 1
            fact = fact.When(x => x == 0, x => 1);

            //show result
            Console.WriteLine(fact(10));
        }

        public static void Func_Without_Memoize()
        {
            //this example shows how much time is spent for function without memoize

            //create fibo function
            Func<int, int> fibo = null;
            fibo = x => fibo(x - 1) + fibo(x - 2);
            fibo = fibo.When(x => x <= 1, x => x);

            //show result
            Console.WriteLine(fibo(38));

            //please wait for few seconds to see result
        }

        public static void Func_With_Memoize()
        {
            //this example shows how much time is spent for function with memoize

            //create fibo function
            Func<int, int> fibo = null;
            fibo = x => fibo(x - 1) + fibo(x - 2);
            fibo = fibo.When(x => x <= 1, x => x);

            //use func.Memoize to cache the result determine by input parameter
            fibo = fibo.Memoize();

            //show result
            Console.WriteLine(fibo(38));

            //compare to "Func Without Memoize" this function performs much faster because of Memoization
            //Memoize method suits with function which has overlapping sub-function and optimal sub-structure
            //to learn more about overlapping sub-function and optimal sub-structure, study "Dynamic Programming"
        }

        public static void Curry_Uncurry_Partial()
        {
            //this example shows how to curry, uncurry, and apply partial function

            //create diff function
            Func<Func<double, double>, Func<double, double>> diff =
                f => x => (f(x + 0.000001) - f(x)) / 0.000001;

            //diff function is in curry form "func(a)(b)"
            //diff of Sin is Cos
            //Cos(PI) is -1
            var d = diff(Math.Sin)(Math.PI);
            Console.WriteLine(d);

            //use func.Uncurry to change "func(a)(b)" to "func(a, b)"
            var diff2 = diff.Uncurry();
            d = diff2(Math.Sin, Math.PI);
            Console.WriteLine(d);

            //use func.Curry to change "func(a, b)" to "func(a)(b)"
            var diff3 = diff2.Curry();
            d = diff3(Math.Sin)(Math.PI);
            Console.WriteLine(d);

            //use func.Partal(a) to change "func(a, b)" to "func(b)"
            var cos = diff2.Partial(Math.Sin);
            d = cos(Math.PI);
            Console.WriteLine(d);
        }

        public static void And()
        {
            //this example shows how to combine 2 predicates together

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //create 2 predicates
            Func<int, bool> isPrime = x => x.In(primes);
            Func<int, bool> isFibo = x => x.In(fibo);

            //using predicate.And(predicate2) create new predicate
            var isFiboPrime = isPrime.And(isFibo);

            //test with number 1 - 20
            var fiboPrime = 1.To(20).Where(isFiboPrime);

            //show result
            Console.WriteLine(fiboPrime.ToString(", "));
        }

        public static void Or()
        {
            //this example shows how to create new predicate which accepts value from either input predicates

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //create 2 predicates
            Func<int, bool> isPrime = x => x.In(primes);
            Func<int, bool> isFibo = x => x.In(fibo);

            //using predicate.Or(predicate2) create new predicate which accepts value from either input predicates
            var isFiboOrPrime = isPrime.Or(isFibo);

            //test with number 1 - 20
            var fiboOrPrime = 1.To(20).Where(isFiboOrPrime);

            //show result
            Console.WriteLine(fiboOrPrime.ToString(", "));
        }

        public static void PredicateAll()
        {
            //this example shows how to combine all predicates from predicate collection

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //create 3 predicates
            Func<int, bool> isPrime = x => x.In(primes);
            Func<int, bool> isFibo = x => x.In(fibo);
            Func<int, bool> isOdd = x => (x % 2) == 1;

            //create predicate collection
            var predicates = new[] {isPrime, isFibo, isOdd};

            //using predicates.PredicateAll() to combine all predicates
            var isOddFiboPrime = predicates.PredicateAll();

            //test with number 1 - 20
            var oddFiboPrime = 1.To(20).Where(isOddFiboPrime);

            //show result
            Console.WriteLine(oddFiboPrime.ToString(", "));
        }

        public static void PredicateAny()
        {
            //this example shows how to create new predicate which accepts value from any predicates in collection

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //create 3 predicates
            Func<int, bool> isPrime = x => x.In(primes);
            Func<int, bool> isFibo = x => x.In(fibo);
            Func<int, bool> isOdd = x => (x % 2) == 1;

            //create predicate collection
            var predicates = new[] {isPrime, isFibo, isOdd};

            //using predicates.PredicateAny() to create predicate which accepts value from any predicates in collection
            var isOddOrFiboOrPrime = predicates.PredicateAny();

            //test with number 1 - 20
            var oddOrFiboOrPrime = 1.To(20).Where(isOddOrFiboOrPrime);

            //show result
            Console.WriteLine(oddOrFiboOrPrime.ToString(", "));
        }

        public static void Sample_Memoize()
        {
            //this example shows how to use Memoize to solve problem

            //Find the longest sequence using a starting number under one million.
            //Defined n = n/2 (n is even)
            //and n = 3n + 1 (n is odd)
            //Finish when n is 1
            //(Question from http://projecteuler.net)

            //create recursive function for counting sequence
            Func<long, long> count = null;
            count = n => count(n / 2) + 1; //n = n/2 (n is even)
            count = count.When(n => n % 2 == 1,
                n => count(3 * n + 1) + 1); //n = 3n + 1 (n is odd)
            count = count.When(n => n == 1,
                n => 1); //stop when n = 1
            count = count.Memoize(); //remember the result for next numbers

            //show result
            Console.WriteLine(1L.To(999999L).MaxBy(count));
        }
    }
}
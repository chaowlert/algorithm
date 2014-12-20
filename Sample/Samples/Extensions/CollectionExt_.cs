using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    //CollectionExt makes you handle collection easier
    public static class CollectionExt_
    {
        public static void ForEach()
        {
            //this example shows how to pass each element in collection into an action

            //create collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //collection.ForEach iterates each element to an action
            primes.ForEach(Console.WriteLine);
        }

        public static void ForEach_With_Index()
        {
            //this example shows how to pass each element and index into an action

            //create collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //if you pass 2 parameters action, second parameter will be index
            primes.ForEach((prime, index) =>
                Console.WriteLine("Prime No. {0} is {1}", index + 1, prime)
                );
        }

        public static void IndexOf()
        {
            //this example shows how to get index of a value in collection

            //create collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //collection.IndexOf(value) find the index of specified value
            Console.WriteLine("Index of {0} in primes is {1}", 13, primes.IndexOf(13));
        }

        public static void All_With_Index()
        {
            //this example shows how to determine all elements satisfy the condition
            //with passing each element and index

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //if you pass 2 parameters predicate, second parameter will be index
            //condition below is each element in fibo must equal to element in primes
            var result = fibo.All((f, index) => f == primes[index]);

            //the result is false, because not all elements are equal
            Console.WriteLine("Do all elements of primes equal to fibo?");
            Console.WriteLine(result);
        }

        public static void Any_With_Index()
        {
            //this example shows how to determine one or more elements satisfy the condition
            //with passing each element and index

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //if you pass 2 parameters predicate, second parameter will be index
            //condition below is each element in fibo must equal to element in primes
            var result = fibo.Any((f, index) => f == primes[index]);

            //the result it true, because "13" is equal
            Console.WriteLine("Do any elements of primes equal to fibo?");
            Console.WriteLine(result);
        }

        public static void WithIndex()
        {
            //this example shows how to inject index into collection

            //create collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //collection.WithIndex injects index into a tuple
            //item.Item1 will be index, item.Item2 will be value
            primes.WithIndex().ForEach(item =>
                Console.WriteLine("Prime No. {0} is {1}", item.Item1 + 1, item.Item2)
                );
        }

        public static void Zip()
        {
            //this example shows how to map 2 collections together

            //create 2 collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //collection.Zip(collection2) combines 2 collections into a tuple
            primes.Zip(fibo).ForEach(item =>
                Console.WriteLine("Prime {0} is map with Fibo {1}", item.Item1, item.Item2)
                );
        }

        public static void Zip_2()
        {
            //this example shows how to map 2 collections and select the new result

            //create 2 collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //collection.Zip(collection2, selector) combines 2 collections to the new result
            primes.Zip(fibo, (p, f) => p + f).ForEach(item =>
                Console.WriteLine("Sum of prime and fibo is {0}", item)
                );
        }

        public static void In()
        {
            //this example shows how to find a value in collection

            //create 2 collections
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};
            var fibo = new[] {1, 2, 3, 5, 8, 13, 21, 34};

            //value.In(collection) finds a value in collection
            //below is to find primes in fibo
            var fiboPrimes = from p in primes
                                          where p.In(fibo)
                                          select p;

            //show result
            Console.WriteLine("Find any primes equal to fibo");
            Console.WriteLine(fiboPrimes.ToString(", "));
        }

        public static void In_2()
        {
            //this example shows how to find a value in collection by passing variable number of parameters

            //create collection
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //value.In(arg1, arg2, .., argN) finds a value in passing parameters
            //below is to find primes in (9, 11)
            var prime911 = from p in primes
                                        where p.In(9, 11)
                                        select p;

            //show result
            Console.WriteLine("Find any primes equal to 9, 11");
            Console.WriteLine(prime911.ToString(", "));
        }

        public static void Repeat()
        {
            //this example shows how to repeat a collection for spectified times

            //create collection
            var nums = new[] {1, 2, 3};

            //collection.Repeat(times) repeats the collection for n times
            var result = nums.Repeat(3);

            //show result
            Console.WriteLine("Repeat {0} for 3 times", nums.ToString(", "));
            Console.WriteLine(result.ToString(", "));
        }

        public static void ToEnumerable()
        {
            //this example shows how to create a single element collection from a value

            //value.ToEnumerable converts value to collection
            Console.WriteLine("Does type of 13.ToEnumerable equal to IEnumerable?");
            Console.WriteLine(13.ToEnumerable() is IEnumerable<int>);
        }

        public static void MaxBy()
        {
            //this example shows how to get an item which has maximum specified value

            //create person collection
            var persons = new[]
            {
                new {Name = "John", Age = 20},
                new {Name = "Sara", Age = 18},
                new {Name = "Ryan", Age = 25}
            };

            //using collection.MaxBy(selector) to get oldest person
            Console.WriteLine("Oldest person is {0}", persons.MaxBy(p => p.Age));

            //unlike the Max method, Max method will return only maximum value
            //but MaxBy method returns item with maximum value
        }

        public static void MinBy()
        {
            //this example shows how to get an item which has minimum specified value

            //create person collection
            var persons = new[]
            {
                new {Name = "John", Age = 20},
                new {Name = "Sara", Age = 18},
                new {Name = "Ryan", Age = 25}
            };

            //using collection.MinBy(selector) to get youngest person
            Console.WriteLine("Youngest person is {0}", persons.MinBy(p => p.Age));

            //unlike the Min method, Min method will return only minimum value
            //but MinBy method returns item with minimum value
        }

        public static void PairWise()
        {
            //this example shows how to pair item with next item

            //create simple prime
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //use collection.PairWise() to pair item with next item
            Console.WriteLine(primes.PairWise().ToString(", "));
        }
    }
}
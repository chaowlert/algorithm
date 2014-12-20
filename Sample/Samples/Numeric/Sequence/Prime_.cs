using System;
using System.IO;
using System.Linq;
using Chaow.Extensions;
using Chaow.Threading;

namespace Chaow.Numeric.Sequence
{
    //Prime allows you to generate primes, test for primes, and get prime factors
    public static class Prime_
    {
        public static void Prime()
        {
            //this example shows sample of prime numbers

            //a prime number is number which has only 2 divisors, 1 and itself
            //here is the first 25 prime number
            Console.WriteLine(new Prime().Take(25).ToString(", "));
        }

        public static void Prime_Contains()
        {
            //this example shows how to test for a prime

            //create integers
            var a = 234567;
            var b = 234571;

            //you can use prime.Contains(number) to test for a prime number
            var prime = new Prime();
            Console.WriteLine("Is {0} a prime number?", a);
            Console.WriteLine(prime.Contains(a));
            Console.WriteLine();
            Console.WriteLine("Is {0} a prime number?", b);
            Console.WriteLine(prime.Contains(b));
        }

        public static void Prime_Factors()
        {
            //this example shows how to get prime factors

            //you can use prime.Factors(number) to get prime divisors of that number
            var a = 234567;
            Console.WriteLine("Prime factors for {0} are {1}", a, new Prime().Factors(a).ToString(" * "));
        }

        public static void Prime_From_Range()
        {
            //this example shows how to generate large prime from range

            //you can generate prime from range with prime.FromRange(start,end)
            //below is to generate prime from 961748941 to 961749037
            Console.WriteLine(new Prime().FromRange(961748941, 961749037).ToString(" "));
        }

        public static void Prime_EulerPhi()
        {
            //this example shows how to use EulerPhi function

            //EulerPhi(n) is function to get count of numbers which are coprime with n and less than n
            //ie EulerPhi(6) = 2 because in number range 1-5, only 1 and 5 are coprime with 6
            var prime = new Prime();
            Console.WriteLine("EulerPhi of {0} is {1}", 22, prime.EulerPhi(22));
            Console.WriteLine("EulerPhi of {0} is {1} <-- prime is always be coprime with any numbers", 23, prime.EulerPhi(23));
            Console.WriteLine("EulerPhi of {0} is {1}", 24, prime.EulerPhi(24));
        }

        public static void Prime_MoebiusMu()
        {
            //this example shows how to use MoebiusMu function

            //if n is divisible by square number, MoebiusMu(n) will return 0
            //if n has odd number of divisors, it will return -1, otherwise it will return 1
            var prime = new Prime();
            Console.WriteLine("MoebiusMu of {0} is {1} <-- 2*11, 2 divisors = 1", 22, prime.MoebiusMu(22));
            Console.WriteLine("MoebiusMu of {0} is {1} <-- prime is always -1", 23, prime.MoebiusMu(23));
            Console.WriteLine("MoebiusMu of {0} is {1} <-- square divisor of 24 is 4", 24, prime.MoebiusMu(24));
        }

        public static void Prime_A018892()
        {
            //this example shows how to use A018892 function

            //A018892 is function to find number of solutions of 1/x + 1/y = 1/n
            //ie A018892(2) = 2 because 1/2 = 1/4 + 1/4 and 1/2 = 1/3 + 1/6
            var prime = new Prime();
            Console.WriteLine("A018892 of {0} is {1}", 22, prime.A018892(22));
            Console.WriteLine("A018892 of {0} is {1} <-- prime always has only 2 solutions", 23, prime.A018892(23));
            Console.WriteLine("A018892 of {0} is {1}", 24, prime.A018892(24));
        }

        public static void Prime_Load_From_File()
        {
            //this example shows how to load prime store from file

            //Prime itself is quick and small, but it is slow to generate large prime
            //PrimeFile will load prime number upto 2,147,483,647
            //using PrimeFile need prime.dat file. file size is 128MB (zip file 70MB)
            //you can download prime.dat from http://codeplex.com/chaow

            //not allow abort in this code
            Worker.AbortWait();

            //get primeFile
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "prime.dat";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("you need file {0} to run PrimeFile", filePath);
                return;
            }
            var primeFile = new PrimeFile(filePath);
            var prime = new Prime(primeFile);

            //create integer
            var a = int.MaxValue;
            var b = a - 2;

            //show result
            Console.WriteLine("Is {0} a prime number?", a);
            Console.WriteLine(prime.Contains(a));
            Console.WriteLine();
            Console.WriteLine("Is {0} a prime number?", b);
            Console.WriteLine(prime.Contains(b));
        }

        public static void Sample_Prime_1()
        {
            //this example shows how to use prime to solve problem

            //Find the 10001st prime
            //(Question from http://projecteuler.net)
            Console.WriteLine(new Prime().ElementAt(10000));
        }

        public static void Sample_Prime_2()
        {
            //this example shows how to use prime to solve problem

            //Find the largest prime factor of 317584931803
            //(Question from http://projecteuler.net)
            Console.WriteLine(new Prime().Factors(317584931803).Max());
        }
    }
}
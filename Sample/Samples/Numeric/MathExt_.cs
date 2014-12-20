using System;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Numeric
{
    //MathExt provides basic to immediate integer math calculation
    public static class MathExt_
    {
        public static void Abs()
        {
            //this example shows how to use get absolute value from integer

            //create an integer
            var a = -4;

            //use number.Abs to get absolute value
            Console.WriteLine("Absolute value of {0} is {1}", a, a.Abs());
        }

        public static void Gcd()
        {
            //this example shows how to get greatest common divisor (Gcd)

            //create 2 integers
            var a = 18;
            var b = 24;

            //use MathExt.Gcd(num1, num2) to get Gcd
            Console.WriteLine("Gcd of {0} and {1} is {2}", a, b, MathExt.Gcd(a, b));
        }

        public static void Lcm()
        {
            //this example shows how to get least common multiplier (Lcm)

            //create 2 integers
            var a = 18;
            var b = 24;

            //use MathExt.Lcm(num1, num2) to get Lcm
            Console.WriteLine("Lcm of {0} and {1} is {2}", a, b, MathExt.Lcm(a, b));
        }

        public static void Sqrt()
        {
            //this example shows how to get integer square root

            //create 3 integers
            var a = 24;
            var b = 25;
            var c = 26;

            //use number.Sqrt() to get integer square root
            Console.WriteLine("Sqrt of {0} is {1}", a, a.Sqrt());
            Console.WriteLine("Sqrt of {0} is {1}", b, b.Sqrt());
            Console.WriteLine("Sqrt of {0} is {1}", c, c.Sqrt());
        }

        public static void Power()
        {
            //this example shows how to get exponentation

            //create integers
            var a = -9;
            var b = -5;
            var c = -2;

            //use number.Power(exp) to get exponentation
            Console.WriteLine("{0} power {1} is {2}", a, 2, a.Power(2));
            Console.WriteLine("{0} power {1} is {2}", b, 3, b.Power(3));
            Console.WriteLine("{0} power {1} is {2}", c, 4, c.Power(4));
        }

        public static void ModPow()
        {
            //this example shows how to get modular exponentation

            //create integers
            var a = -9;
            var b = -5;
            var c = -2;

            //use number.ModPow(exp,mod) to get modular exponentation
            Console.WriteLine("{0} power {1} % {2} is {3}", a, 2, 4, a.ModPow(2, 4));
            Console.WriteLine("{0} power {1} % {2} is {3}", b, 3, 4, b.ModPow(3, 4));
            Console.WriteLine("{0} power {1} % {2} is {3}", c, 4, 4, c.ModPow(4, 4));
        }

        public static void Mod()
        {
            //this example shows how to get modulo

            //create integers
            var a = 5;
            var b = 3;

            //use number.Mod(mod) to get modulo
            //% operator gives the same sign as dividend
            //modulo always give the same sign as divisor
            Console.WriteLine("{0} % {1} = {2}\t{0} mod {1} = {3}", a, b, a % b, a.Mod(b));
            Console.WriteLine("{0} % {1} = {2}\t{0} mod {1} = {3}", -a, b, -a % b, (-a).Mod(b));
            Console.WriteLine("{0} % {1} = {2}\t{0} mod {1} = {3}", a, -b, a % -b, a.Mod(-b));
            Console.WriteLine("{0} % {1} = {2}\t{0} mod {1} = {3}", -a, -b, -a % -b, (-a).Mod(-b));
        }

        public static void ExtGcd()
        {
            //this example shows how to get extended gcd "ax + by = gcd(a,b)"

            //create integers
            var a = 120;
            var b = 23;

            //use MathExt.ExtGcd(a,b) to find extended gcd
            //the result will return integer array { gcd, x, y }
            var result = MathExt.ExtGcd(a, b);
            Console.WriteLine("Result is {0}*{1} + {2}*{3} = {4}", a, result[1], b, result[2], result[0]);
        }

        public static void ModInverse()
        {
            //this example shows how to get modular multiplicative inverse "ax mod b = 1"

            //create integers
            var a = 120;
            var b = 23;

            //use MathExt.ModInverse(a,b) to find modular multiplicative inverse
            try
            {
                Console.WriteLine("Result is {0}*{1} mod {2} = 1", a, MathExt.ModInverse(a, b), b);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("There is no solution for {0}*X mod {1} = 1", a, b);
            }
        }

        public static void ChineseRemainder()
        {
            //this example shows how to find Chinese remainder "x mod n[i] = a[i]"

            //create lists of integers
            var a = new[] {2, 3, 1};
            var n = new[] {3, 4, 5};

            //use MathExt.ChineseRemainder(a[],n[]) to find Chinese remainder
            try
            {
                var result = MathExt.ChineseRemainder(a, n);
                Console.WriteLine("Result is {0} mod {{{1}}} = {{{2}}}", result, n.ToString(", "), a.ToString(", "));
                Console.WriteLine("Or");
                for (var i = 0; i < a.Length; i++)
                    Console.WriteLine("{0} mod {1} = {2}", result, n[i], a[i]);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("There is no solution for X mod {{{0}}} = {{{1}}}", n.ToString(", "), a.ToString(", "));
            }
        }

        public static void Random()
        {
            //this example shows how to get random

            //use MathExt.Random to get a static Random
            //construct a new random is time-dependent, you may get duplicate results
            Console.WriteLine("These random numbers are generate from new Random");
            for (var i = 0; i < 5; i++)
                Console.WriteLine(new Random().NextDouble());
            Console.WriteLine();
            Console.WriteLine("These random numbers are generate from MathExt.Random");
            for (var i = 0; i < 5; i++)
                Console.WriteLine(MathExt.Random.NextDouble());
        }

        public static void Sample_Lcd()
        {
            //this example shows how to use Lcd to solve problem

            //What is the smallest number divisible by each of the numbers 1 to 20?
            //(Question from http://projecteuler.net)
            Console.WriteLine(1.To(20).Aggregate(MathExt.Lcm));
        }
    }
}
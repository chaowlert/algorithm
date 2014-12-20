using System;
using System.Globalization;

namespace Chaow.Numeric
{
    //Rational represents arbitrary precision fraction
    public static class Rational_
    {
        public static void Rational_Create()
        {
            //this example shows how to create rational

            //Rational is struct, it never been null
            //BUT its default value is NaN, not Zero!
            var a = default(Rational);
            Console.WriteLine("Default Rational value is {0}", a);

            //Rational can be assign from integer
            //Rational always keep numerator and denominator in reduced form,
            //it will not keep 3/12 but it will keep 1/4
            var b = (Rational)3 / 12;
            Console.WriteLine("Value of Rational b is {0}", b);

            //Rational can be created from constructor(num, denom)
            var c = new Rational(-15, 20);
            Console.WriteLine("Value of Rational c is {0}", c);

            //Rational can guess value from decimal
            Rational d = 0.142857142857M;
            Console.WriteLine("Value of Rational d is {0}", d);
        }

        public static void Rational_Numerator_Denominator()
        {
            //this example shows accessing the rational's numerator and denominator

            //create Rational
            var a = (Rational)100 / 81;

            //Rational has 2 properties, which are numerator and denominator
            //their types are BigInteger
            Console.WriteLine(a.Numerator);
            Console.WriteLine(a.Denominator);
        }

        public static void Rational_Arithmetic()
        {
            //this example shows how to use arithmetic and comparison operators

            //create Rationals
            var a = (Rational)12 / 20;
            var b = (Rational)6 / 30;
            var c = (Rational)(-1) / 50;

            //all arithmetic and comparison operators are the same as real number
            Console.WriteLine("{0} + {1} = {2}", a, b, a + b);
            Console.WriteLine("{0} - {1} = {2}", c, b, c - b);
            Console.WriteLine("{0} * {1} = {2}", c, a, c * a);
            Console.WriteLine("{0} / {1} = {2}", b, a, b / a);
            Console.WriteLine("{0} % {1} = {2}", b, c, b % c);
            Console.WriteLine("{0} >= {1} is {2}", a, c, a >= c);
            Console.WriteLine("{0} < {1} is {2}", a, b, a < b);
        }

        public static void Rational_CastTo()
        {
            //this example shows what types, Rational can cast to

            //create Rational
            var a = (Rational)100 / 81;

            //BigInteger can cast to bunch of integer types
            Console.WriteLine((int)a);
            Console.WriteLine((uint)a);
            Console.WriteLine((long)a);
            Console.WriteLine((ulong)a);
            Console.WriteLine((float)a);
            Console.WriteLine((double)a);
            Console.WriteLine((decimal)a);
        }

        public static void Rational_Math()
        {
            //this example shows various math methods of Rational

            //create BigIntegers
            var a = (Rational)12 / 20;
            var b = (Rational)6 / 30;
            var c = (Rational)(-1) / 50;

            //using various math methods with BigInteger
            Console.WriteLine("Abs of {0} = {1}", c, c.Abs());
            Console.WriteLine("{0} power 3 = {1}", b, b.Power(3));
            Console.WriteLine("Invert of {0} = {1}", a, a.Invert());
        }

        public static void Rational_Parse()
        {
            //this example shows how to parse to Rational

            //Rational can parse from decimal and heximal
            var a = Rational.Parse("12345/67890");
            var b = Rational.Parse("12345678/90ABCDEF", NumberStyles.HexNumber);

            //show results
            Console.WriteLine(a);
            Console.WriteLine(b);

            //please note that NumberStyles which Rational supported are 
            //NumberStyles.Integer and NumberStyles.HexNumber
        }

        public static void Rational_ToString()
        {
            //this example shows how to use BigInteger.ToString

            //create BigIntegers
            var a = (Rational)12345 / 67890;
            var b = (Rational)101806632 / 809059493;

            //you can format string to decimal and heximal
            Console.WriteLine(a.ToString());
            Console.WriteLine(b.ToString("X"));

            //please note that format which Rational supported are
            //"G" for General, and "X" for hex number
        }

        public static void Rational_Arbitrary()
        {
            //this example shows Rational is an arbitrary precision fraction

            //create Rational
            var a = (Rational)1 / 20;
            var b = (Rational)1 / 15;
            var c = (Rational)1 / 12;

            //show results
            Console.WriteLine("Rational: a^2 + b^2 - c^2 = {0}", a.Power(2) + b.Power(2) - c.Power(2));

            //compare with double
            var d = 1.0 / 20.0;
            var e = 1.0 / 15.0;
            var f = 1.0 / 12.0;

            //show results
            Console.WriteLine("double: d^2 + e^2 - f^2 = {0} <-- double is not precise enough!", Math.Pow(d, 2.0) + Math.Pow(e, 2.0) - Math.Pow(f, 2.0));
        }

        public static void Rational_Extended_Real()
        {
            //this example shows Rational is also included extended real numbers
            //extened real included Infinity, NegativeInfinity, NaN (Not a Number), and Negative Zero

            //some arithmatic operation of extended real numbers
            Console.WriteLine("1/0 = {0}", (Rational)1 / 0);
            Console.WriteLine("inf*-1 = {0}", Rational.PositiveInfinity * -1);
            Console.WriteLine("0%0 = {0}", (Rational)0 % 0);
            Console.WriteLine("1+inf = {0}", 1 + Rational.PositiveInfinity);
            Console.WriteLine("1/-inf = {0} <-- this is NegativeZero", 1 / Rational.NegativeInfinity);
            Console.WriteLine("Invert of -0 = {0}", (-Rational.Zero).Invert());

            //comparison of extended real numbers
            //-Inf < -num < -0 == 0 < num < Inf
            Console.WriteLine();
            Console.WriteLine("Inf > 1.0 is {0}", Rational.PositiveInfinity > 1);
            Console.WriteLine("1.0 > 0.0 is {0}", (Rational)1 > 0);
            Console.WriteLine("0.0 == -0.0 is {0}", Rational.Zero == -Rational.Zero);
            Console.WriteLine("0.0 > -1.0 is {0}", Rational.Zero > -1);
            Console.WriteLine("-1.0 > -Inf is {0}", -1 > Rational.NegativeInfinity);
        }

        public static void Rational_NaN()
        {
            //this example shows operation of NaN (Not a Number)

            //using arithmatic operation with NaN always results NaN
            Console.WriteLine("NaN + 1 = {0}", Rational.NaN + 1);
            Console.WriteLine("NaN * Inf = {0}", Rational.NaN * Rational.PositiveInfinity);
            Console.WriteLine("NaN / 0 = {0}", Rational.NaN / 0);

            //NaN is unknown value, it cannot be compare with anything, INCLUDED ITSELF
            Console.WriteLine();
            Console.WriteLine("NaN > Inf is {0}", Rational.NaN > Rational.PositiveInfinity);
            Console.WriteLine("NaN <= Inf is {0}", Rational.NaN == Rational.PositiveInfinity);
            Console.WriteLine("NaN == NaN is {0} <-- NaN is not equal to itself!", Rational.NaN == Rational.NaN);
            Console.WriteLine("NaN != NaN is {0}", Rational.NaN != Rational.NaN);

            //to compare NaN using Ratinal.IsNaN(), Rational.CompareTo() and Rational.Equals()
            Console.WriteLine();
            Console.WriteLine("NaN.IsNaN is {0}", Rational.IsNaN(Rational.NaN));
            Console.WriteLine("NaN.CompareTo(-Inf) is {0} <-- when using CompareTo, NaN always less than others", Rational.NaN.CompareTo(Rational.NegativeInfinity));
            Console.WriteLine("NaN.CompareTo(NaN) is {0}", Rational.NaN.CompareTo(Rational.NaN));
            Console.WriteLine("NaN.Equals(NaN) is {0}", Rational.NaN.Equals(Rational.NaN));
        }
    }
}
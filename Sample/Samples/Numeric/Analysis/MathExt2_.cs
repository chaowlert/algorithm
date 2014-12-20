using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Chaow.Numeric.Analysis
{
    //MathExt2 is collection of math analysis functions
    public static class MathExt2_
    {
        public static void Rewrite()
        {
            //this example shows how to rewrite formula

            //you can rewrite formula of BigInteger, Rational, and double by using Rewrite
            Expression<Func<double, double>> func = x => (x + 1.0) * (x + 2.0) - 3.0 * x;

            //show result
            //use func.Rewrite() to rewrite formula
            Console.WriteLine(func.Rewrite());
        }

        public static void PolynomialInterpolation()
        {
            //this example shows how to find formula from numbers

            //using MathExt2.PolynomialInterpolation(results, inputs) to do polynomial interpolation
            //inputs can be omit, if you omit inputs, 1 to result count will be assumed
            var formula = MathExt2.PolynomialInterpolation(new Rational[] {1, 8, 27, 64});

            //guess next number
            Console.WriteLine(formula.Compile()(5));
            Console.WriteLine();

            //you can get the formula
            Console.WriteLine(formula);
            Console.WriteLine();

            //you can rewrite the formula with PolynomialRewriter
            Console.WriteLine(formula.Rewrite());
        }

        public static void DeriveQuadratic()
        {
            //this example shows how to find integer solution for aXX + bX + c = 0

            //example -2XX + X + 15
            //use MathExt2.DeriveQuadratic(a,b,c) to derive quadratic formula
            BigInteger[] xs = MathExt2.DeriveQuadratic(-2, 1, 15);

            //Show results
            Console.WriteLine(xs[0]);
            Console.WriteLine();

            //Recheck
            Console.WriteLine("-2*{0}*{0} + {0} + 15 = {1}", xs[0], -2 * xs[0] * xs[0] + xs[0] + 15);
        }

        public static void Differentiation()
        {
            //this example shows how to find derivative of a real formula

            //example x^4 + sin(x^2) - ln(x)e^x + 7
            Expression<Func<double, double>> func = x => Math.Pow(x, 4.0) + Math.Sin(Math.Pow(x, 2.0)) - Math.Log(x) * Math.Exp(x) + 7.0;

            //show result
            //use func.Differentiation to find derivative
            Console.WriteLine(func.Differentiation());
        }

        public static void NewtonsMethod()
        {
            //this example shows how to use Newton's method to solve problem

            //example solve x^2 = 612
            //then x^2 - 612 = 0
            Expression<Func<double, double>> func = x => Math.Pow(x, 2.0) - 612;

            //use func.NewtonsMehtod(startNumber, precision) to use Newton's method
            //if you omit startNumber, randon number will be assumed
            //if you omit precision, 12 will be assumed
            Console.WriteLine(func.NewtonsMethod());
        }
    }
}
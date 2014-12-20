using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Chaow.Numeric.Analysis
{
    public static class MathExt2
    {
        //fields
        static DiophantineRewriter _diophantineRewriter;
        static PolynomialRewriter _polynomialRewriter;
        static RealRewriter _realRewriter;

        //properties
        static DiophantineRewriter DiophantineRewriter
        {
            get
            {
                if (_diophantineRewriter == null)
                    _diophantineRewriter = new DiophantineRewriter();
                return _diophantineRewriter;
            }
        }

        static PolynomialRewriter PolynomialRewriter
        {
            get
            {
                if (_polynomialRewriter == null)
                    _polynomialRewriter = new PolynomialRewriter();
                return _polynomialRewriter;
            }
        }

        static RealRewriter RealRewriter
        {
            get
            {
                if (_realRewriter == null)
                    _realRewriter = new RealRewriter();
                return _realRewriter;
            }
        }

        //methods
        public static Expression<Func<BigInteger, BigInteger>> Rewrite(this Expression<Func<BigInteger, BigInteger>> exp)
        {
            return DiophantineRewriter.Rewrite(exp);
        }

        public static Expression<Func<BigInteger, BigInteger, BigInteger>> Rewrite(this Expression<Func<BigInteger, BigInteger, BigInteger>> exp)
        {
            return DiophantineRewriter.Rewrite(exp);
        }

        public static Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> Rewrite(this Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> exp)
        {
            return DiophantineRewriter.Rewrite(exp);
        }

        public static Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger, BigInteger>> Rewrite(this Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger, BigInteger>> exp)
        {
            return DiophantineRewriter.Rewrite(exp);
        }

        public static Expression<Func<Rational, Rational>> Rewrite(this Expression<Func<Rational, Rational>> exp)
        {
            return PolynomialRewriter.Rewrite(exp);
        }

        public static Expression<Func<Rational, Rational, Rational>> Rewrite(this Expression<Func<Rational, Rational, Rational>> exp)
        {
            return PolynomialRewriter.Rewrite(exp);
        }

        public static Expression<Func<Rational, Rational, Rational, Rational>> Rewrite(this Expression<Func<Rational, Rational, Rational, Rational>> exp)
        {
            return PolynomialRewriter.Rewrite(exp);
        }

        public static Expression<Func<Rational, Rational, Rational, Rational, Rational>> Rewrite(this Expression<Func<Rational, Rational, Rational, Rational, Rational>> exp)
        {
            return PolynomialRewriter.Rewrite(exp);
        }

        public static Expression<Func<double, double>> Rewrite(this Expression<Func<double, double>> exp)
        {
            return RealRewriter.Rewrite(exp);
        }

        public static Expression<Func<double, double, double>> Rewrite(this Expression<Func<double, double, double>> exp)
        {
            return RealRewriter.Rewrite(exp);
        }

        public static Expression<Func<double, double, double, double>> Rewrite(this Expression<Func<double, double, double, double>> exp)
        {
            return RealRewriter.Rewrite(exp);
        }

        public static Expression<Func<double, double, double, double, double>> Rewrite(this Expression<Func<double, double, double, double, double>> exp)
        {
            return RealRewriter.Rewrite(exp);
        }

        public static Expression<Func<Rational, Rational>> PolynomialInterpolation(IEnumerable<Rational> values)
        {
            return PolynomialInterpolation(values, Enumerable.Range(1, values.Count()).Select(x => (Rational)x).ToList());
        }

        public static Expression<Func<Rational, Rational>> PolynomialInterpolation(IEnumerable<Rational> values, IEnumerable<Rational> inputs)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (inputs == null)
                throw new ArgumentNullException("inputs");
            var c = values.Count();
            if (inputs.Count() != c)
                throw new ArgumentException("Length of inputs should match with values", "inputs");
            if (c == 0)
                throw new ArgumentException("Values should contain element", "values");

            var p = Expression.Parameter(typeof(Rational), "x");
            if (c == 1)
                return Expression.Lambda<Func<Rational, Rational>>(Expression.Constant(values.First(), typeof(Rational)), p);

            var inputEx = inputs.Select(x => Expression.Constant(x, typeof(Rational))).ToList();
            var input2 = inputs as IList<Rational>;
            if (input2 == null)
                input2 = inputs.ToList();

            return Expression.Lambda<Func<Rational, Rational>>(
                values.Select((x, i) =>
                    Expression.Multiply(
                        Expression.Constant(
                            x / input2.Where((y, j) => j != i)
                                      .Select(y => input2[i] - y)
                                      .Aggregate((a, b) => a * b),
                            typeof(Rational)),
                        inputEx.Where((y, j) => j != i)
                               .Select(y => Expression.Subtract(p, y))
                               .Aggregate(Expression.Multiply)
                        )
                    ).Aggregate(Expression.Add),
                p
                );
        }

        public static BigInteger[] DeriveQuadratic(BigInteger a, BigInteger b, BigInteger c)
        {
            return DeriveQuadratic(a, b, c, true);
        }

        public static BigInteger[] DeriveQuadratic(BigInteger a, BigInteger b, BigInteger c, bool exact)
        {
            if (a.IsZero)
                throw new ArgumentException("a cannot be zero", "a");

            var s = b * b - 4 * a * c;
            BigInteger r1, r2;

            if (s.Sign < 0)
                return new BigInteger[0];

            var t = s.Sqrt();
            var isPerfect = t * t == s;
            if (exact && !isPerfect)
                return new BigInteger[0];

            var b2 = -b;
            var a2 = 2 * a;

            var x1 = b2 - t;
            if (!isPerfect && x1.Sign > 0)
                x1 -= BigInteger.One;
            x1 = BigInteger.DivRem(x1, a2, out r1);

            var x2 = b2 + t;
            if (!isPerfect && x2.Sign < 0)
                x1 += BigInteger.One;
            x2 = BigInteger.DivRem(x2, a2, out r2);

            if (!exact)
                return new[] {x1, x2};
            if (r1.IsZero)
            {
                if (r2.IsZero)
                    return new[] {x1, x2};
                return new[] {x1};
            }
            if (r2.IsZero)
                return new[] {x2};
            return new BigInteger[0];
        }

        public static Expression<Func<double, double>> Differentiation(this Expression<Func<double, double>> exp)
        {
            var method = typeof(MathExt2).GetMethod("Diff", BindingFlags.Static | BindingFlags.NonPublic);
            exp = Expression.Lambda<Func<double, double>>(Expression.Call(method, exp.Body), exp.Parameters);
            return exp.Rewrite();
        }

        internal static double Diff(double d)
        {
            throw new NotSupportedException();
        }

        public static double NewtonsMethod(this Expression<Func<double, double>> exp)
        {
            return NewtonsMethod(exp, MathExt.Random.NextDouble(), 12);
        }

        public static double NewtonsMethod(this Expression<Func<double, double>> exp, double startNumber)
        {
            return NewtonsMethod(exp, startNumber, 12);
        }

        public static double NewtonsMethod(this Expression<Func<double, double>> exp, double startNumber, int precision)
        {
            exp = Expression.Lambda<Func<double, double>>(
                Expression.Subtract(
                    exp.Parameters[0],
                    Expression.Divide(exp.Body, exp.Differentiation().Body)), exp.Parameters);

            var func = exp.Compile();
            var maxDiff = Math.Pow(10.0, -precision);
            var previous = startNumber;
            var current = func(previous);

            while (Math.Abs(previous - current) > maxDiff)
            {
                previous = current;
                current = func(previous);
            }
            return current;
        }
    }
}
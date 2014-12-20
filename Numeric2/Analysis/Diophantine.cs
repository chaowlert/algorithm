using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using Chaow.Combinatorics;
using Chaow.Numeric.Sequence;

namespace Chaow.Numeric.Analysis
{
    public enum DiophantineType
    {
        Static,
        Linear,
        Parabolic,
        Hyperbolic,
    }

    public struct DiophantinePair
    {
        //fields
        readonly BigInteger _x;
        readonly BigInteger _y;

        public DiophantinePair(BigInteger x, BigInteger y)
        {
            _x = x;
            _y = y;
        }

        //properties
        public BigInteger X
        {
            get { return _x; }
        }

        public BigInteger Y
        {
            get { return _y; }
        }

        //constructors

        //methods
        public override string ToString()
        {
            return string.Format("{{X = {0}, Y = {1}}}", _x, _y);
        }
    }

    public class DiophantineSolution : BaseSequence<DiophantinePair>
    {
        //fields
        readonly Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> _funcX1;
        readonly Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> _funcX2;
        readonly Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> _funcY1;
        readonly Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> _funcY2;
        readonly DiophantineType _type;
        readonly BigInteger _x0, _y0;
        bool _reverse;

        internal DiophantineSolution(DiophantineType type, BigInteger x0, BigInteger y0,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcX1,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcY1)
            : this(type, x0, y0, funcX1, funcY1, funcX1, funcY1)
        {
        }

        internal DiophantineSolution(DiophantineType type, BigInteger x0, BigInteger y0,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcX1,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcY1,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcX2,
                                     Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> funcY2)
        {
            _type = type;
            _x0 = x0;
            _y0 = y0;
            _funcX1 = funcX1.Rewrite();
            _funcY1 = funcY1.Rewrite();
            if (funcX2 != null)
            {
                _funcX2 = funcX2.Rewrite();
                _funcY2 = funcY2.Rewrite();
            }
        }

        //properties
        public DiophantineType Type
        {
            get { return _type; }
        }

        public bool IsReversible
        {
            get { return _funcX2 != null; }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        public BigInteger X0
        {
            get { return _x0; }
        }

        public BigInteger Y0
        {
            get { return _y0; }
        }

        public Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> FuncX1
        {
            get { return _funcX1; }
        }

        public Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> FuncY1
        {
            get { return _funcY1; }
        }

        public Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> FuncX2
        {
            get { return _funcX2; }
        }

        public Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>> FuncY2
        {
            get { return _funcY2; }
        }

        //constructors

        //protected methods
        protected override IEnumerable<DiophantinePair> enumerate()
        {
            var x = _x0;
            var y = _y0;

            yield return new DiophantinePair(x, y);
            if (_type == DiophantineType.Static)
                yield break;

            var t = BigInteger.Zero;
            BigInteger one;
            Func<BigInteger, BigInteger, BigInteger, BigInteger> funcX;
            Func<BigInteger, BigInteger, BigInteger, BigInteger> funcY;

            if (_reverse)
            {
                one = -BigInteger.One;
                funcX = _funcX2.Compile();
                funcY = _funcY2.Compile();
            }
            else
            {
                one = BigInteger.One;
                funcX = _funcX1.Compile();
                funcY = _funcY1.Compile();
            }

            while (true)
            {
                t += one;
                var z = funcX(t, x, y);
                y = funcY(t, x, y);
                x = z;
                yield return new DiophantinePair(x, y);
            }
        }
    }

    public class Diophantine
    {
        //fields
        readonly List<DiophantineSolution> _solutions = new List<DiophantineSolution>();

        //properties

        //constructors
        public Diophantine(BigInteger a, BigInteger b, BigInteger c, BigInteger d, BigInteger e, BigInteger f)
        {
            //reduce & precheck
            var g = new[] {a, b, c, d, e}.Aggregate(BigIntegerExt.Gcd);
            if (!g.IsZero && g != BigInteger.One)
            {
                BigInteger r;
                f = BigInteger.DivRem(f, g, out r);
                if (!r.IsZero)
                    return;
                a /= g;
                b /= g;
                c /= g;
                d /= g;
                e /= g;
            }

            //select solutions
            if (a.IsZero && c.IsZero)
            {
                if (b.IsZero)
                    linear(d, e, f);
                else
                    simpleHyperbolic(b, d, e, f);
            }
            else
            {
                var p = b * b - 4 * a * c;
                if (p.Sign < 0)
                    elliptical(a, b, c, d, e, f);
                else if (p.IsZero)
                    parabolic(a, b, c, d, e, f);
                else if (d.IsZero && e.IsZero)
                    hyperbolic(a, b, c, f, p);
                else
                    complexHyperbolic(a, b, c, d, e, f);
            }
        }

        public ReadOnlyCollection<DiophantineSolution> Solutions
        {
            get { return new ReadOnlyCollection<DiophantineSolution>(_solutions); }
        }

        //public static method
        public static Diophantine Parse(Expression<Func<BigInteger, BigInteger, BigInteger>> exp)
        {
            exp = exp.Rewrite();
            var list = new DiophantineParser().Parse(exp.Parameters[0], exp.Parameters[1], exp.Body);
            return new Diophantine(list[0], list[1], list[2], list[3], list[4], list[5]);
        }

        //private methods
        void addStaticSolution(Rational r1, Rational r2)
        {
            if (r1.Denominator > BigInteger.One)
                return;
            if (r2.Denominator > BigInteger.One)
                return;
            var x0 = r1.Numerator;
            var y0 = r2.Numerator;

            _solutions.Add(
                new DiophantineSolution(
                    DiophantineType.Static, x0, y0,
                    (t, x, y) => x0,
                    (t, x, y) => y0));
        }

        void linearWithGcd(BigInteger d, BigInteger e, BigInteger f)
        {
            var g = BigIntegerExt.Gcd(d, e);
            if (!g.IsZero && g != BigInteger.One)
            {
                d /= g;
                e /= g;
                f /= g;
            }
            linear(d, e, f);
        }

        void linear(BigInteger d, BigInteger e, BigInteger f)
        {
            f = -f;

            if (d.IsZero)
            {
                if (e.IsZero)
                {
                    if (f.IsZero)
                        addStaticSolution(Rational.Zero, Rational.Zero);
                }
                else
                    addStaticSolution(Rational.Zero, (Rational)f / e);
            }
            else if (e.IsZero)
                addStaticSolution((Rational)f / d, Rational.Zero);
            else
                linear2(d, e, f);
        }

        void linear2(BigInteger d, BigInteger e, BigInteger f)
        {
            var g = BigIntegerExt.ExtGcd(d, e);
            g[1] *= f;
            g[2] *= f;

            d = -d;
            var k = (g[1] / e + g[2] / d) / 2;
            if (!k.IsZero)
            {
                g[1] -= k * e;
                g[2] -= k * d;
            }

            _solutions.Add(
                new DiophantineSolution(
                    DiophantineType.Linear, g[1], g[2],
                    (t, x, y) => g[1] + e * t,
                    (t, x, y) => g[2] + d * t));
        }

        void simpleHyperbolic(BigInteger b, BigInteger d, BigInteger e, BigInteger f)
        {
            var n = d * e - b * f;

            if (n.IsZero)
            {
                addStaticSolution((Rational)(-e) / b, Rational.Zero);
                addStaticSolution(Rational.Zero, (Rational)(-d) / b);
            }
            else
                simpleHyperbolic2(n, b, d, e);
        }

        void simpleHyperbolic2(BigInteger n, BigInteger b, BigInteger d, BigInteger e)
        {
            foreach (var p in new BigDivisor(n.Abs()))
            {
                addStaticSolution((Rational)(p - e) / b, (Rational)(n / p - d) / b);
                addStaticSolution((Rational)(-p - e) / b, (Rational)(n / -p - d) / b);
            }
        }

        void elliptical(BigInteger a, BigInteger b, BigInteger c, BigInteger d, BigInteger e, BigInteger f)
        {
            var m = 4 * a;
            var i = m * c - b * b;
            var j = m * e - 2 * b * d;
            var k = m * f - d * d;
            var n = MathExt2.DeriveQuadratic(i, j, k, false);

            if (n.Length == 0)
                return;
            m = 2 * a;
            for (var u = n[0]; u <= n[1]; u += BigInteger.One)
            {
                var v = -(i * u * u + j * u + k);
                var w = v.Sqrt();
                if (w * w == v)
                {
                    var s = d + (b * u);
                    addStaticSolution((Rational)(w - s) / m, u);
                    addStaticSolution((Rational)(-w - s) / m, u);
                }
            }
        }

        void parabolic(BigInteger a, BigInteger b, BigInteger c, BigInteger d, BigInteger e, BigInteger f)
        {
            var g = BigIntegerExt.Gcd(a, c);
            c = (c / g).Abs().Sqrt();
            if (b.Sign > 0 == a.Sign < 0)
                c = -c;
            a = (a / g).Abs().Sqrt();

            var m = c * d - a * e;
            if (m.IsZero)
            {
                var u = MathExt2.DeriveQuadratic(a * g, d, a * f, true);
                if (u.Length > 0)
                {
                    linearWithGcd(a, c, -u[0]);
                    if (u.Length > 1 && u[0] != u[1])
                        linearWithGcd(a, c, -u[1]);
                }
            }
            else
                parabolic2(a, -c, m, a * g, d, a * f);
        }

        void parabolic2(BigInteger a, BigInteger c, BigInteger m, BigInteger i, BigInteger d, BigInteger j)
        {
            foreach (var yset in quadraticDivisor(i, d, j, m))
            {
                foreach (var xset in quadraticDivisor(c * yset[2], c * yset[3] + yset[1], c * yset[4] + yset[0], a))
                {
                    yset[4] = yset[2] * xset[0] * xset[0] + yset[3] * xset[0] + yset[4];
                    yset[3] = xset[1] * (yset[3] + 2 * xset[0] * yset[2]);
                    yset[2] = yset[2] * xset[1] * xset[1];
                    _solutions.Add(
                        new DiophantineSolution(
                            DiophantineType.Parabolic, xset[4], yset[4],
                            (t, x, y) => xset[2] * t * t + xset[3] * t + xset[4],
                            (t, x, y) => yset[2] * t * t + yset[3] * t + yset[4]));
                }
            }
        }

        void hyperbolic(BigInteger a, BigInteger b, BigInteger c, BigInteger f, BigInteger p)
        {
            var q = p.Sqrt();
            var selectors = new Func<Rational[], Rational[]>[] {r => r};

            if (f.IsZero)
            {
                if (q * q == p)
                {
                    a *= 2;
                    linearWithGcd(a, b + q, BigInteger.Zero);
                    linearWithGcd(a, b - q, BigInteger.Zero);
                }
                else
                    addStaticSolution(Rational.Zero, Rational.Zero);
            }
            else if (q * q == p)
                hyperbolic2(a * 2, b + q, 4 * a * f, q * 2, selectors);
            else
                hyperbolic3(a, b, c, f, createFunction(a, b, c), selectors);
        }

        void hyperbolic2(BigInteger a, BigInteger b, BigInteger f, BigInteger q, Func<Rational[], Rational[]>[] selectors)
        {
            foreach (var u in new BigDivisor(f.Abs()))
            {
                var r = (Rational)(u + f / u) / q;
                var s = (u - b * r) / a;
                addStaticSolution(selectors, s, r);
                addStaticSolution(selectors, -s, -r);
            }
        }

        void hyperbolic3(BigInteger a, BigInteger b, BigInteger c, BigInteger f,
                         List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]> explist,
                         Func<Rational[], Rational[]>[] selectors)
        {
            if (explist.Count == 0)
                return;
            var funcList = explist.Select(exp =>
                new[]
                {
                    (exp[0] != null) ? exp[0].Compile() : null,
                    (exp[1] != null) ? exp[1].Compile() : null
                }).ToList();

            foreach (var sd in squareDivisor(f.Abs()))
            {
                var fnew = -f / (sd * sd);
                var g = BigIntegerExt.Gcd(a, fnew).Abs();
                foreach (var d in new BigDivisor(g))
                {
                    foreach (var set in quadraticDivisor(a / d, b, c * d, fnew / d))
                    {
                        foreach (var selector in selectors)
                        {
                            foreach (var rt in computeQuadratic(set[4], set[3], set[2], 1, (s1, s2) =>
                                selector(new[]
                                {
                                    (set[0] * s1 + set[1] * s2) * sd,
                                    s1 * d * sd
                                })))
                                addQuadraticSolutions(rt[0], rt[1], explist, funcList);
                        }
                    }
                }
            }
        }

        void addQuadraticSolutions(BigInteger x0, BigInteger y0, List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]> explist, List<Func<BigInteger, BigInteger, BigInteger, BigInteger>[]> funcList)
        {
            BigInteger x1, y1;

            if (funcList.Count == 4)
            {
                var x2 = x0;
                var y2 = y0;
                do
                {
                    x1 = x2;
                    y1 = y2;
                    x2 = funcList[0][0](BigInteger.Zero, x1, y1);
                    y2 = funcList[0][1](BigInteger.Zero, x1, y1);
                } while (x1.IsZero || x2.IsZero);
                var x3 = funcList[2][0](BigInteger.Zero, x2, y2);
                var y3 = funcList[2][1](BigInteger.Zero, x2, y2);
                if (x1.Sign < 0 == x2.Sign < 0)
                {
                    if (x3 == x1)
                    {
                        funcList.RemoveAt(3);
                        funcList.RemoveAt(1);
                        explist.RemoveAt(3);
                        explist.RemoveAt(1);
                    }
                    else
                    {
                        funcList.RemoveAt(2);
                        funcList.RemoveAt(1);
                        explist.RemoveAt(2);
                        explist.RemoveAt(1);
                    }
                }
                else if (x3 == x1)
                {
                    funcList.RemoveAt(2);
                    funcList.RemoveAt(0);
                    explist.RemoveAt(2);
                    explist.RemoveAt(0);
                }
                else
                {
                    funcList.RemoveAt(3);
                    funcList.RemoveAt(0);
                    explist.RemoveAt(3);
                    explist.RemoveAt(0);
                }
            }

            foreach (var func in funcList)
            {
                if (func[0] != null)
                {
                    x1 = func[0](BigInteger.Zero, x0, y0);
                    y1 = func[1](BigInteger.Zero, x0, y0);
                    while (y1.Abs() < y0.Abs() || (y1.Abs() == y0.Abs() && x1.Abs() < x0.Abs()))
                    {
                        x0 = x1;
                        y0 = y1;
                        x1 = func[0](BigInteger.Zero, x0, y0);
                        y1 = func[1](BigInteger.Zero, x0, y0);
                    }
                }
            }

            if (!_solutions.Any(s => s.X0 == x0 && s.Y0 == y0))
            {
                _solutions.Add(
                    new DiophantineSolution(
                        DiophantineType.Hyperbolic, x0, y0,
                        explist[0][0], explist[0][1],
                        explist[1][0], explist[1][1]));
            }
        }

        void addStaticSolution(Func<Rational[], Rational[]>[] selectors, Rational r1, Rational r2)
        {
            Rational[] p = {r1, r2};

            foreach (var selector in selectors)
            {
                var r = selector(p);
                if (r[0].Denominator > BigInteger.One)
                    return;
                if (r[1].Denominator > BigInteger.One)
                    return;

                var x0 = r[0].Numerator;
                var y0 = r[1].Numerator;

                if (!_solutions.Any(s => s.X0 == x0 && s.Y0 == y0))
                {
                    _solutions.Add(
                        new DiophantineSolution(
                            DiophantineType.Static, x0, y0,
                            (t, x, y) => x0,
                            (t, x, y) => y0));
                }
            }
        }

        void complexHyperbolic(BigInteger a, BigInteger b, BigInteger c, BigInteger d, BigInteger e, BigInteger f)
        {
            //refactor to form ax^2 + cy^2 + f = 0
            var m = 4 * a;
            var ba = m * c - b * b;
            var bb = m * e - 2 * b * d;
            var bc = m * f - d * d;
            var c2 = BigIntegerExt.Gcd(ba, bb).Abs();
            ba /= c2;
            bb /= c2;
            var a2 = ba;
            if ((bb % 2).IsZero)
                bb /= 2;
            else
            {
                c2 /= 4;
                ba *= 2;
            }
            m = 2 * a;
            var f2 = bc * a2 - c2 * bb * bb;

            //reduce & precheck
            var g = BigIntegerExt.Gcd(a2, c2);
            if (g != BigInteger.One)
            {
                BigInteger r;
                f2 = BigInteger.DivRem(f2, g, out r);
                if (!r.IsZero)
                    return;
                a2 /= g;
                c2 /= g;
            }

            //declare
            var selectors = new Func<Rational[], Rational[]>[]
            {
                r =>
                {
                    var y0 = (r[1] - bb) / ba;
                    return new[]
                    {
                        (r[0] - y0 * b - d) / m,
                        y0
                    };
                },
                r =>
                {
                    var y0 = (r[1] - bb) / ba;
                    return new[]
                    {
                        (-r[0] - y0 * b - d) / m,
                        y0
                    };
                },
                r =>
                {
                    var y0 = (-r[1] - bb) / ba;
                    return new[]
                    {
                        (r[0] - y0 * b - d) / m,
                        y0
                    };
                },
                r =>
                {
                    var y0 = (-r[1] - bb) / ba;
                    return new[]
                    {
                        (-r[0] - y0 * b - d) / m,
                        y0
                    };
                }
            };
            var p = -4 * a2 * c2;
            var q = p.Sqrt();

            if (f2.IsZero)
            {
                if (q * q == p)
                {
                    a2 *= 2;
                    var m2 = a2 * m;
                    var q2 = a2 * d + a2 * b * bb;
                    var set = new[]
                    {
                        Tuple.Create(m2, ba * (a2 * b + q), q2 + q * bb),
                        Tuple.Create(m2, ba * (a2 * b - q), q2 - q * bb),
                        Tuple.Create(-m2, ba * (a2 * b + q), q2 + q * bb),
                        Tuple.Create(-m2, ba * (a2 * b - q), q2 - q * bb),
                        Tuple.Create(m2, -ba * (a2 * b + q), q2 + q * bb),
                        Tuple.Create(m2, -ba * (a2 * b - q), q2 - q * bb),
                        Tuple.Create(-m2, -ba * (a2 * b + q), q2 + q * bb),
                        Tuple.Create(-m2, -ba * (a2 * b - q), q2 - q * bb)
                    };
                    foreach (var item in set.Distinct())
                        linearWithGcd(item.Item1, item.Item2, item.Item3);
                }
                else
                    addStaticSolution(selectors, BigInteger.Zero, BigInteger.Zero);
            }
            else if (q * q == p)
                hyperbolic2(a2 * 2, q, 4 * a2 * f2, q * 2, selectors);
            else
                hyperbolic3(a2, BigInteger.Zero, c2, f2, createFunction2(a, b, c, d, e), selectors);
        }

        //static methods
        static IEnumerable<BigInteger[]> quadraticDivisor(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
        {
            var g = new[] {a, b, d}.Aggregate(BigIntegerExt.Gcd).Abs();
            if (g != BigInteger.One)
            {
                a /= g;
                b /= g;
                c /= g;
                d /= g;
            }
            var dplus = d.Abs();
            for (var i = dplus - BigInteger.One; i.Sign >= 0; i -= BigInteger.One)
            {
                BigInteger r;
                var q = BigInteger.DivRem(a * i * i + b * i + c, d, out r);
                if (r.IsZero)
                    yield return new[] {i, dplus, a * d, (b + 2 * i * a) * ((d.Sign < 0) ? -BigInteger.One : BigInteger.One), q};
            }
        }

        static IEnumerable<BigInteger> squareDivisor(BigInteger num)
        {
            var solutions = new BigPrime().Factors(num)
                                                                          .ToLookup(p => p)
                                                                          .SelectMany(x => Enumerable.Repeat(x.Key, x.Count() / 2))
                                                                          .ToList()
                                                                          .Backtrack(
                                                                              BigInteger.One,
                                                                              (prod, item, i) => prod * item,
                                                                              prod => true
                );
            solutions.BacktrackingModel = BacktrackingModel.Combination;
            solutions.Distinct = true;
            solutions.ContinueOnYielded = true;
            return solutions.SelectResults();
        }

        static List<BigInteger[]> computeQuadratic(BigInteger a, BigInteger b, BigInteger c, int times, Func<Rational, Rational, Rational[]> selector)
        {
            var a2 = 2 * a;
            var s = b * b - 4 * a * c;
            Rational[] rt;
            var list = new List<BigInteger[]>();

            var cf = new ContinuedFraction(s, -b, a2);
            foreach (var r in cf.Take(1 + cf.SubQuotients.Count + ((cf.PeriodicLength % 2 == 0) ? 1 : 2) * times * cf.PeriodicLength))
            {
                if (a * r.Numerator * r.Numerator + b * r.Numerator * r.Denominator + c * r.Denominator * r.Denominator == BigInteger.One)
                {
                    rt = selector(r.Numerator, r.Denominator);
                    if (rt[0].Denominator == BigInteger.One && rt[1].Denominator == BigInteger.One)
                    {
                        list.Add(new[] {rt[0].Numerator, rt[1].Numerator});
                        break;
                    }
                }
            }
            foreach (var r in cf.Take(1 + cf.SubQuotients.Count + ((cf.PeriodicLength % 2 == 0) ? 1 : 2) * times * cf.PeriodicLength))
            {
                if (a * r.Numerator * r.Numerator + b * r.Numerator * r.Denominator + c * r.Denominator * r.Denominator == BigInteger.One)
                {
                    rt = selector(-r.Numerator, -r.Denominator);
                    if (rt[0].Denominator == BigInteger.One && rt[1].Denominator == BigInteger.One)
                    {
                        list.Add(new[] {rt[0].Numerator, rt[1].Numerator});
                        break;
                    }
                }
            }
            cf = new ContinuedFraction(s, b, -a2);
            foreach (var r in cf.Take(1 + cf.SubQuotients.Count + ((cf.PeriodicLength % 2 == 0) ? 1 : 2) * times * cf.PeriodicLength))
            {
                if (a * r.Numerator * r.Numerator + b * r.Numerator * r.Denominator + c * r.Denominator * r.Denominator == BigInteger.One)
                {
                    rt = selector(r.Numerator, r.Denominator);
                    if (rt[0].Denominator == BigInteger.One && rt[1].Denominator == BigInteger.One)
                    {
                        list.Add(new[] {rt[0].Numerator, rt[1].Numerator});
                        break;
                    }
                }
            }
            foreach (var r in cf.Take(1 + cf.SubQuotients.Count + ((cf.PeriodicLength % 2 == 0) ? 1 : 2) * times * cf.PeriodicLength))
            {
                if (a * r.Numerator * r.Numerator + b * r.Numerator * r.Denominator + c * r.Denominator * r.Denominator == BigInteger.One)
                {
                    rt = selector(-r.Numerator, -r.Denominator);
                    if (rt[0].Denominator == BigInteger.One && rt[1].Denominator == BigInteger.One)
                    {
                        list.Add(new[] {rt[0].Numerator, rt[1].Numerator});
                        break;
                    }
                }
            }
            return list;
        }

        static List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]> createFunction(BigInteger a, BigInteger b, BigInteger c)
        {
            var list = new List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]>();
            var rtList = computeQuadratic(BigInteger.One, b, a * c, 1, (s1, s2) => new[] {s1, s2});

            foreach (var rt in rtList)
            {
                list.Add(new[]
                {
                    ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => rt[0] * x + -c * rt[1] * y)).Rewrite(),
                    ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => a * rt[1] * x + (rt[0] + b * rt[1]) * y)).Rewrite()
                });
            }
            if (list.Count == 2)
                list[1] = new Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[] {null, null};
            return list;
        }

        static List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]> createFunction2(BigInteger a, BigInteger b, BigInteger c, BigInteger d, BigInteger e)
        {
            var list = new List<Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[]>();
            BigInteger t1 = 4 * a * c - b * b;
            BigInteger p, q, r, s, k, l;
            var rtList = computeQuadratic(BigInteger.One, b, a * c, 2, (s1, s2) => new[] {s1, s2});

            foreach (var rt in rtList)
            {
                p = rt[0];
                q = -c * rt[1];
                r = a * rt[1];
                s = rt[0] + b * rt[1];
                var t2 = p + s - 2;
                var t3 = b - b * rt[0] - 2 * a * c * rt[1];
                BigInteger t4;
                k = BigInteger.DivRem(c * d * t2 + e * t3, t1, out t4);
                if (t4.IsZero)
                {
                    l = BigInteger.DivRem(d * t3 + a * e * t2, t1, out t4) + d * rt[1];
                    if (t4.IsZero)
                    {
                        list.Add(new[]
                        {
                            ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => p * x + q * y + k)).Rewrite(),
                            ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => r * x + s * y + l)).Rewrite()
                        });
                    }
                }
            }
            if (list.Count == 0)
            {
                for (var i = 0; i < rtList.Count; i += 2)
                {
                    var rt = rtList[i];
                    p = rt[0] * rt[0] - a * c * rt[1] * rt[1];
                    q = -c * rt[1] * (2 * rt[0] + b * rt[1]);
                    k = -rt[1] * (e * rt[0] + c * d * rt[1]);
                    r = a * rt[1] * (2 * rt[0] + b * rt[1]);
                    s = rt[0] * rt[0] + 2 * b * rt[0] * rt[1] + (b * b - a * c) * rt[1] * rt[1];
                    l = rt[1] * (d * rt[0] + (b * d - a * e) * rt[1]);
                    list.Add(new[]
                    {
                        ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => p * x + q * y + k)).Rewrite(),
                        ((Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>)((t, x, y) => r * x + s * y + l)).Rewrite()
                    });
                }
            }
            if (list.Count == 1)
                list.Add(new Expression<Func<BigInteger, BigInteger, BigInteger, BigInteger>>[] {null, null});
            return list;
        }
    }

    class DiophantineParser : ExpressionVisitor
    {
        static readonly ConstantExpression one = Expression.Constant(BigInteger.One);
        BigInteger[] _list;
        ParameterExpression _x;
        ParameterExpression _y;

        public BigInteger[] Parse(ParameterExpression x, ParameterExpression y, Expression exp)
        {
            _list = new BigInteger[6];
            _x = x;
            _y = y;
            Visit(exp);
            return _list;
        }

        public override Expression Visit(Expression exp)
        {
            if (exp != null && exp.NodeType != ExpressionType.Add)
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.Constant:
                        if (setF((ConstantExpression)exp))
                            return exp;
                        break;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        if (setABCDE((BinaryExpression)exp))
                            return exp;
                        break;
                    case ExpressionType.Call:
                        if (setAC(one, (MethodCallExpression)exp))
                            return exp;
                        break;
                    case ExpressionType.Parameter:
                        if (setDE(one, (ParameterExpression)exp))
                            return exp;
                        break;
                    default:
                        break;
                }
                return exp;
            }
            return base.Visit(exp);
        }

        bool setF(ConstantExpression exp)
        {
            _list[5] = (BigInteger)exp.Value;
            return true;
        }

        bool setABCDE(BinaryExpression mult)
        {
            if (mult.Left.NodeType == ExpressionType.Constant)
                return setABCDE((ConstantExpression)mult.Left, mult.Right);
            return setB(one, mult);
        }

        bool setABCDE(ConstantExpression left, Expression right)
        {
            switch (right.NodeType)
            {
                case ExpressionType.Parameter:
                    return setDE(left, (ParameterExpression)right);
                case ExpressionType.Call:
                    return setAC(left, (MethodCallExpression)right);
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return setB(left, (BinaryExpression)right);
                default:
                    return false;
            }
        }

        bool setDE(ConstantExpression left, ParameterExpression right)
        {
            if (_x.Equals(right))
            {
                _list[3] = (BigInteger)left.Value;
                return true;
            }
            if (_y.Equals(right))
            {
                _list[4] = (BigInteger)left.Value;
                return true;
            }
            return false;
        }

        bool setAC(ConstantExpression left, MethodCallExpression methodCall)
        {
            if (methodCall.Method.Name == "Power")
            {
                Expression param;
                Expression cons;
                if (methodCall.Object == null)
                {
                    param = methodCall.Arguments[0];
                    cons = methodCall.Arguments[1];
                }
                else
                {
                    param = methodCall.Object;
                    cons = methodCall.Arguments[0];
                }
                if (cons.NodeType == ExpressionType.Constant && ((int)((ConstantExpression)cons).Value) == 2)
                {
                    if (_x.Equals(param))
                    {
                        _list[0] = (BigInteger)left.Value;
                        return true;
                    }
                    if (_y.Equals(param))
                    {
                        _list[2] = (BigInteger)left.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        bool setB(ConstantExpression left, BinaryExpression mult)
        {
            if ((_x.Equals(mult.Left) && _y.Equals(mult.Right))
                || (_y.Equals(mult.Left) && _x.Equals(mult.Right)))
            {
                _list[1] = (BigInteger)left.Value;
                return true;
            }
            return false;
        }
    }
}
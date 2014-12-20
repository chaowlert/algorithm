using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using Chaow.Numeric.Sequence;

namespace Chaow.Numeric.Analysis
{
    public class ContinuedFraction : BaseSequence<Rational>
    {
        //fields

        //public fields
        public static readonly ContinuedFraction Phi = new ContinuedFraction(BigInteger.One, Enumerable.Empty<BigInteger>(), new[] {BigInteger.One});
        static readonly BigInteger Two = 2;
        public static readonly ContinuedFraction E = new ContinuedFraction(Two, Enumerable.Empty<BigInteger>(), (from x in new GenericSequence<BigInteger>(Two, x => x + Two).TakeWhile(x => true)
                                                                                                                            from y in new[] {false, true, false}
                                                                                                                            select (y) ? x : BigInteger.One).ToSequence());
        readonly int _periodicLength;
        readonly BaseSequence<BigInteger> _periodics;
        readonly BigInteger _quotient;
        readonly List<BigInteger> _subQuotients = new List<BigInteger>();

        //properties

        //constructors
        public ContinuedFraction(BigInteger sqrt) : this(sqrt, BigInteger.Zero, BigInteger.One)
        {
        }

        public ContinuedFraction(BigInteger sqrt, BigInteger num, BigInteger denom)
        {
            if (denom.IsZero)
                throw new ArgumentException("denom cannot be zero", "denom");
            if (sqrt.Sign < 0)
                throw new ArgumentOutOfRangeException("sqrt", "sqrt cannot be negative");

            var s = sqrt.Sqrt();
            if ((sqrt - s * s).IsZero)
            {
                _quotient = computeFinite(num + s, denom, _subQuotients);
                _periodics = Enumerable.Empty<BigInteger>().ToSequence();
                _periodicLength = 0;
            }
            else
            {
                var periodics = new List<BigInteger>();
                _quotient = computePeriodic(sqrt, num, denom, _subQuotients, periodics);
                _periodics = periodics.Cycle();
                _periodicLength = periodics.Count;
            }
        }

        public ContinuedFraction(BigInteger quotient, IEnumerable<BigInteger> subQuotients) : this(quotient, subQuotients, Enumerable.Empty<BigInteger>())
        {
        }

        public ContinuedFraction(BigInteger quotient, IEnumerable<BigInteger> subQuotients, IEnumerable<BigInteger> periodics)
        {
            _quotient = quotient;
            _subQuotients.AddRange(subQuotients);
            _periodics = periodics.Cycle();
            _periodicLength = periodics.Count();
        }

        public ContinuedFraction(BigInteger quotient, IEnumerable<BigInteger> subQuotients, BaseSequence<BigInteger> periodics)
        {
            _quotient = quotient;
            _subQuotients.AddRange(subQuotients);
            _periodics = periodics;
            _periodicLength = (periodics.Take(1).Any()) ? -1 : 0;
        }

        public BigInteger Quotient
        {
            get { return _quotient; }
        }

        public ReadOnlyCollection<BigInteger> SubQuotients
        {
            get { return new ReadOnlyCollection<BigInteger>(_subQuotients); }
        }

        public BaseSequence<BigInteger> Periodics
        {
            get { return _periodics; }
        }

        public int PeriodicLength
        {
            get { return _periodicLength; }
        }

        //static methods
        static BigInteger computeFinite(BigInteger num, BigInteger denom, List<BigInteger> subQuotients)
        {
            var g = BigIntegerExt.Gcd(num, denom);

            if (g != BigInteger.One)
            {
                num /= g;
                denom /= g;
            }
            if (denom.Sign < 0)
            {
                num = -num;
                denom = -denom;
            }

            var t = num.Mod(denom);
            var quotient = (num - t) / denom;
            num = t;

            while (num.Sign > 0 && denom.Sign > 0)
            {
                denom = BigInteger.DivRem(denom, num, out t);
                subQuotients.Add(denom);
                denom = num;
                num = t;
            }

            return quotient;
        }

        static BigInteger computePeriodic(BigInteger sqrt, BigInteger num, BigInteger denom, List<BigInteger> subQuotients, List<BigInteger> periodics)
        {
            BigInteger quotient;

            if (!((sqrt - num * num) % denom).IsZero)
            {
                sqrt *= denom * denom;
                num *= denom.Abs();
                denom *= denom.Abs();
            }
            var s = sqrt.Sqrt();

            var t = s + num;
            if (t.IsZero)
            {
                if (denom.Sign > 0)
                    quotient = BigInteger.Zero;
                else
                    quotient = -BigInteger.One;
            }
            else
            {
                if (t.Sign < 0)
                    t += BigInteger.One;
                if (t.Sign > 0 == denom.Sign > 0)
                    quotient = t / denom;
                else
                    quotient = (t - denom) / denom;
            }

            num = quotient * denom - num;
            while (denom.Sign < 0 || num.Sign < 0 || num > s || denom > s + num)
            {
                denom = (sqrt - num * num) / denom;
                if (denom.Sign > 0)
                    t = (s + num) / denom;
                else
                    t = (s + num + BigInteger.One) / denom;
                num = t * denom - num;
                subQuotients.Add(t);
            }

            var exDenom = denom;
            var exNum = num;
            do
            {
                denom = (sqrt - num * num) / denom;
                t = (s + num) / denom;
                num = t * denom - num;
                periodics.Add(t);
            } while (exDenom != denom || exNum != num);

            return quotient;
        }

        //protected methods
        protected override IEnumerable<Rational> enumerate()
        {
            var previous = Rational.PositiveInfinity;
            Rational current = _quotient;
            Rational result;

            yield return current;
            foreach (var subQuotient in _subQuotients)
            {
                result = Rational.NextConvergent(current, previous, subQuotient);
                previous = current;
                current = result;
                yield return current;
            }

            foreach (var periodic in _periodics.TakeWhile(x => true))
            {
                result = Rational.NextConvergent(current, previous, periodic);
                previous = current;
                current = result;
                yield return current;
            }
        }

        //public methods
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[').Append(_quotient);
            if (_subQuotients.Count > 0)
            {
                sb.Append("; ");
                sb.Append(string.Join(", ", _subQuotients.Select(x => x.ToString()).ToArray()));
            }
            if (_periodicLength != 0)
            {
                if (_subQuotients.Count == 0)
                    sb.Append(';');
                sb.Append(" (");
                if (_periodicLength > 0)
                    sb.Append(string.Join(", ", _periodics.Take(_periodicLength).Select(x => x.ToString()).ToArray()));
                else
                {
                    sb.Append(string.Join(", ", _periodics.Take(10).Select(x => x.ToString()).ToArray()));
                    sb.Append(", ...");
                }
                sb.Append(')');
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
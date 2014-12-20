using System;
using System.Globalization;
using System.Numerics;

namespace Chaow.Numeric
{
    public struct Rational : IFormattable, IComparable, IComparable<Rational>, IEquatable<Rational>
    {
        //constants

        //static field
        public static readonly Rational One = new Rational(BigInteger.One, BigInteger.One);
        public static readonly Rational Zero = new Rational(BigInteger.Zero, BigInteger.One);
        public static readonly Rational PositiveInfinity = new Rational(BigInteger.One, BigInteger.Zero);
        public static readonly Rational NegativeInfinity = new Rational(-BigInteger.One, BigInteger.Zero);
        public static readonly Rational NaN = new Rational(BigInteger.Zero, BigInteger.Zero);
        readonly BigInteger _denominator;
        readonly BigInteger _numerator;

        public Rational(BigInteger numerator, BigInteger denominator)
        {
            //reduce
            var minus = false;
            if (numerator.Sign < 0)
            {
                minus = true;
                numerator = -numerator;
            }
            if (denominator.Sign < 0)
            {
                minus = !minus;
                denominator = -denominator;
            }
            var gcd = BigIntegerExt.Gcd(numerator, denominator);
            if (gcd > BigInteger.One)
            {
                numerator /= gcd;
                denominator /= gcd;
            }
            if (minus)
                if (numerator.IsZero)
                    denominator = -denominator;
                else
                    numerator = -numerator;

            //assign
            _numerator = numerator;
            _denominator = denominator;
        }

        //properties
        public BigInteger Numerator
        {
            get { return _numerator; }
        }

        public BigInteger Denominator
        {
            get { return _denominator; }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is Rational))
                throw new ArgumentException("obj's type must be Rational", "obj");
            return Compare(this, (Rational)obj);
        }

        public int CompareTo(Rational other)
        {
            return Compare(this, other);
        }

        public bool Equals(Rational other)
        {
            if (_numerator != other._numerator)
                return false;
            if (_numerator.IsZero)
                return (_denominator.IsZero) == (other._denominator.IsZero);
            return _denominator == other._denominator;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var info = NumberFormatInfo.GetInstance(formatProvider);
            if (_denominator.IsZero)
            {
                if (_numerator.IsZero)
                    return info.NaNSymbol;
                if (_numerator.Sign > 0)
                    return info.PositiveInfinitySymbol;
                return info.NegativeInfinitySymbol;
            }
            if (_numerator.IsZero)
                return 0.ToString(format, formatProvider);
            if (_denominator == BigInteger.One)
                return _numerator.ToString(format, formatProvider);
            return string.Format("{0}/{1}", _numerator.ToString(format, formatProvider), _denominator.ToString(format, formatProvider));
        }

        //constructors

        //static methods
        static rationalType getRationalType(Rational r)
        {
            if (r._numerator.IsZero)
                return (r._denominator.IsZero) ? rationalType.NaN : rationalType.ZeroRational;
            if (r._numerator.Sign > 0)
                return (r._denominator.IsZero) ? rationalType.PositiveInfinity : rationalType.PositiveRational;
            return (r._denominator.IsZero) ? rationalType.NegativeInfinity : rationalType.NegativeRational;
        }

        //operators
        public static Rational operator +(Rational x, Rational y)
        {
            if (x._denominator == y._denominator)
            {
                if (x._denominator.IsZero && (x._numerator.IsZero || y._numerator.IsZero))
                    return NaN;
                return new Rational(x._numerator + y._numerator, x._denominator);
            }
            if (x._denominator.Sign < 0)
                x = Zero;
            if (y._denominator.Sign < 0)
                y = Zero;
            return new Rational(x._numerator * y._denominator + y._numerator * x._denominator, x._denominator * y._denominator);
        }

        public static Rational operator -(Rational x)
        {
            if (x._numerator.IsZero)
                return new Rational(x._numerator, -x._denominator);
            return new Rational(-x._numerator, x._denominator);
        }

        public static Rational operator -(Rational x, Rational y)
        {
            return x + -y;
        }

        public static Rational operator ++(Rational x)
        {
            return x + One;
        }

        public static Rational operator --(Rational x)
        {
            return x + -One;
        }

        public static Rational operator *(Rational x, Rational y)
        {
            var num = x._numerator * y._numerator;
            if (num.IsZero && (x._numerator.Sign < 0 || y._numerator.Sign < 0))
                return new Rational(num, -x._denominator * y._denominator);
            return new Rational(num, x._denominator * y._denominator);
        }

        public static Rational operator /(Rational x, Rational y)
        {
            return x * y.Invert();
        }

        public static Rational operator %(Rational x, Rational y)
        {
            if (y._numerator.IsZero || x._denominator.IsZero)
                return NaN;
            if (y._denominator.IsZero)
                return x;
            var num = (x._numerator * y._denominator) % (y._numerator * x._denominator);
            if (num.IsZero && (x._numerator.Sign < 0 || x._denominator.Sign < 0))
                return -Zero;
            return new Rational(num, x._denominator * y._denominator);
        }

        public static bool operator ==(Rational x, Rational y)
        {
            if (IsNaN(x))
                return false;
            return x.Equals(y);
        }

        public static bool operator !=(Rational x, Rational y)
        {
            if (IsNaN(x))
                return true;
            return !x.Equals(y);
        }

        public static bool operator <(Rational x, Rational y)
        {
            if (IsNaN(x))
                return false;
            return Compare(x, y) < 0;
        }

        public static bool operator <=(Rational x, Rational y)
        {
            if (IsNaN(x))
                return false;
            return Compare(x, y) <= 0;
        }

        public static bool operator >(Rational x, Rational y)
        {
            if (IsNaN(y))
                return false;
            return Compare(x, y) > 0;
        }

        public static bool operator >=(Rational x, Rational y)
        {
            if (IsNaN(y))
                return false;
            return Compare(x, y) >= 0;
        }

        //cast from
        public static implicit operator Rational(int x)
        {
            return new Rational(x, BigInteger.One);
        }

        public static implicit operator Rational(long x)
        {
            return new Rational(x, BigInteger.One);
        }

        public static implicit operator Rational(uint x)
        {
            return new Rational(x, BigInteger.One);
        }

        public static implicit operator Rational(ulong x)
        {
            return new Rational(x, BigInteger.One);
        }

        public static implicit operator Rational(BigInteger x)
        {
            return new Rational(x, BigInteger.One);
        }

        public static implicit operator Rational(decimal d)
        {
            var minus = false;
            var previous = PositiveInfinity;
            var point = 7;

            var scale = (decimal.GetBits(d)[3] >> 16) & 31;
            if (scale < 7)
            {
                var denominator = (decimal)Math.Pow(10.0, scale);
                return new Rational((BigInteger)(d * denominator), (BigInteger)denominator);
            }

            if (d < 0)
            {
                minus = true;
                d = -d;
            }

            var num = decimal.Truncate(d);
            var result = new Rational((BigInteger)num, BigInteger.One);
            var current = result;
            d -= num;

            while (point > 0 && decimal.Round(d, point--) > 0)
            {
                d = 1.0M / d;
                num = decimal.Truncate(d);
                result = NextConvergent(current, previous, (BigInteger)num);
                previous = current;
                current = result;
                d -= num;
            }
            if (minus)
                result = -result;
            return result;
        }

        //cast to
        public static explicit operator int(Rational x)
        {
            return (int)(x._numerator / x._denominator);
        }

        public static explicit operator uint(Rational x)
        {
            return (uint)(x._numerator / x._denominator);
        }

        public static explicit operator long(Rational x)
        {
            return (long)(x._numerator / x._denominator);
        }

        public static explicit operator ulong(Rational x)
        {
            return (ulong)(x._numerator / x._denominator);
        }

        public static explicit operator float(Rational x)
        {
            return (float)x._numerator / (float)x._denominator;
        }

        public static explicit operator double(Rational x)
        {
            return (double)x._numerator / (double)x._denominator;
        }

        public static explicit operator decimal(Rational x)
        {
            BigInteger r;
            var d = (decimal)BigInteger.DivRem(x._numerator, x._denominator, out r);
            d += (decimal)r / (decimal)x._denominator;
            return d;
        }

        //public methods
        public override bool Equals(object obj)
        {
            return (obj is Rational) && Equals((Rational)obj);
        }

        public override int GetHashCode()
        {
            return _numerator.GetHashCode();
        }

        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString("G", formatProvider);
        }

        public Rational Invert()
        {
            return new Rational(_denominator, _numerator);
        }

        public Rational Abs()
        {
            if (_numerator.Sign < 0 || _denominator.Sign < 0)
                return -this;
            return this;
        }

        public Rational Power(int exp)
        {
            if (IsNaN(this))
                return this;

            var minus = false;

            if (exp == 0)
                return One;
            if (exp < 0)
            {
                minus = true;
                exp = -exp;
            }
            var result = new Rational(_numerator.Power(exp), _denominator.Power(exp));
            if (minus)
                result = result.Invert();
            return result;
        }

        //public static methods
        public static int Compare(Rational x, Rational y)
        {
            //compare between number type
            var ntX = getRationalType(x);
            var ntY = getRationalType(y);
            if (ntX < ntY)
                return -1;
            if (ntX > ntY)
                return 1;
            if ((ntX & rationalType.Comparable) != rationalType.Comparable)
                return 0;

            //compare value
            var n = x._numerator.CompareTo(y._numerator);
            var d = x._denominator.CompareTo(y._denominator);
            if (d == 0)
                return n;
            if ((ntX & rationalType.Negative) == rationalType.Negative)
                d = -d;
            if (n == 0)
                return -d;
            if (n != d)
                return n;

            return (x._numerator * y._denominator).CompareTo(y._numerator * x._denominator);
        }

        public static Rational NextConvergent(Rational current, Rational previous, BigInteger num)
        {
            return new Rational(current._numerator * num + previous._numerator,
                current._denominator * num + previous._denominator);
        }

        public static bool IsNaN(Rational r)
        {
            return r._denominator.IsZero && r._numerator.IsZero;
        }

        public static bool IsInfinity(Rational r)
        {
            return r._denominator.IsZero && !r._numerator.IsZero;
        }

        public static bool IsPositiveInfinity(Rational r)
        {
            return r._denominator.IsZero && r._numerator.Sign > 0;
        }

        public static bool IsNegativeInfinity(Rational r)
        {
            return r._denominator.IsZero && r._numerator.Sign < 0;
        }

        public static Rational Parse(string s)
        {
            return Parse(s, NumberStyles.Integer, null);
        }

        public static Rational Parse(string s, IFormatProvider provider)
        {
            return Parse(s, NumberStyles.Integer, provider);
        }

        public static Rational Parse(string s, NumberStyles style)
        {
            return Parse(s, style, null);
        }

        public static Rational Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            var ss = s.Split('/');
            switch (ss.Length)
            {
                case 1:
                    return new Rational(BigInteger.Parse(ss[0], style, provider), BigInteger.One);
                case 2:
                    return new Rational(BigInteger.Parse(ss[0], style, provider), BigInteger.Parse(ss[1], style, provider));
                default:
                    throw new FormatException();
            }
        }

        public static bool TryParse(string s, out Rational result)
        {
            return TryParse(s, NumberStyles.Integer, null, out result);
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Rational result)
        {
            var ss = s.Split('/');
            BigInteger num;
            switch (ss.Length)
            {
                case 1:
                    if (BigInteger.TryParse(ss[0], style, provider, out num))
                    {
                        result = num;
                        return true;
                    }
                    break;
                case 2:
                    BigInteger denom;
                    if (BigInteger.TryParse(ss[0], style, provider, out num) && BigInteger.TryParse(ss[1], style, provider, out denom))
                    {
                        result = new Rational(num, denom);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            result = NaN;
            return false;
        }

        [Flags]
        enum rationalType
        {
            NaN = 0,
            Infinity = 1,
            Comparable = 2,
            Negative = 4,
            Positive = 8,

            NegativeInfinity = Negative | Infinity | 256,
            NegativeRational = Negative | Comparable | 512,
            ZeroRational = 1024,
            PositiveRational = Positive | Comparable | 2048,
            PositiveInfinity = Positive | Infinity | 4096,
        }
    }
}
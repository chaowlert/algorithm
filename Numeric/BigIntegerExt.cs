using System;
using System.Numerics;

namespace Chaow.Numeric
{
    public static class BigIntegerExt
    {
        public static BigInteger Abs(this BigInteger num)
        {
            if (num.Sign < 0)
                return -num;
            return num;
        }

        public static BigInteger Power(this BigInteger num, int pow)
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = BigInteger.One;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                {
                    result *= num;
                    if (pow == 1)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static BigInteger Power(this BigInteger num, long pow)
        {
            if (pow < 0L)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = BigInteger.One;
            while (pow != 0L)
            {
                if ((pow & 1L) == 1L)
                {
                    result *= num;
                    if (pow == 1L)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static BigInteger Power(this BigInteger num, BigInteger pow)
        {
            if (pow.Sign < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = BigInteger.One;
            while (!pow.IsZero)
            {
                if (!pow.IsEven)
                {
                    result *= num;
                    if (pow == BigInteger.One)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static BigInteger ModPow(this BigInteger num, BigInteger pow, BigInteger mod)
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = BigInteger.One;
            while (!pow.IsZero)
            {
                num %= mod;
                if (!pow.IsEven)
                {
                    result = (result * num) % mod;
                    if (pow == BigInteger.One)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            while (!b.IsZero)
            {
                var r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        public static BigInteger Lcm(BigInteger a, BigInteger b)
        {
            return (b / Gcd(a, b)) * a;
        }

        static readonly BigInteger Two = 2;
        public static BigInteger Sqrt(this BigInteger num)
        {
            if (num.Sign < 0)
                throw new ArgumentOutOfRangeException("num", "num cannot be negative");
            if (num.IsZero)
                return BigInteger.Zero;

            var x = num;
            var y = (x + BigInteger.One) / Two;
            while (y < x)
            {
                x = y;
                y = (x + (num / x)) / Two;
            }
            return x;
        }

        public static BigInteger Mod(this BigInteger num, BigInteger mod)
        {
            var num2 = num % mod;
            if (num2.Sign < 0 == mod.Sign > 0)
                return num2 + mod;
            return num2;
        }

        public static BigInteger[] ExtGcd(BigInteger m, BigInteger n)
        {
            var ma = new[] {m, BigInteger.One, BigInteger.Zero};
            var na = new[] {n, BigInteger.Zero, BigInteger.One};

            while (!na[0].IsZero)
            {
                var q = ma[0] / na[0];
                for (var i = 0; i < 3; i++)
                {
                    var r = ma[i] - q * na[i];
                    ma[i] = na[i];
                    na[i] = r;
                }
            }
            return ma;
        }
    }
}
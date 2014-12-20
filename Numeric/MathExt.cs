using System;
using System.Numerics;

namespace Chaow.Numeric
{
    public static class MathExt
    {
        public static readonly Random Random = new Random();

        public static int Abs(this int x)
        {
            return Math.Abs(x);
        }

        public static long Abs(this long x)
        {
            return Math.Abs(x);
        }

        public static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        public static long Gcd(long a, long b)
        {
            while (b != 0L)
            {
                var r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        public static int Lcm(int a, int b)
        {
            b /= Gcd(a, b);
            return checked(b * a);
        }

        public static long Lcm(long a, long b)
        {
            b /= Gcd(a, b);
            return checked(b * a);
        }

        public static int Sqrt(this int num)
        {
            if (num < 0)
                throw new ArgumentOutOfRangeException("num", "num cannot be negative");

            var x = num;
            var y = x / 2 + (x & 1);
            while (y < x)
            {
                x = y;
                y = (x + (num / x)) / 2;
            }
            return x;
        }

        public static long Sqrt(this long num)
        {
            if (num < 0L)
                throw new ArgumentOutOfRangeException("num", "num cannot be negative");

            var x = num;
            var y = x / 2L + (x & 1L);
            while (y < x)
            {
                x = y;
                y = (x + (num / x)) / 2;
            }
            return x;
        }

        public static int Power(this int num, int pow)
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                {
                    result = checked(result * num);
                    if (pow == 1)
                        return result;
                }
                num = checked(num * num);
                pow >>= 1;
            }
            return result;
        }

        public static long Power(this long num, int pow)
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");

            var result = 1L;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                {
                    result = checked(result * num);
                    if (pow == 1)
                        return result;
                }
                num = checked(num * num);
                pow >>= 1;
            }
            return result;
        }

        public static int ModPow(this int num, int pow, int mod)
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");
            if (mod < -46341 || mod > 46341)
                return (int)((long)num).ModPow(pow, mod);

            var result = 1;
            while (pow != 0)
            {
                num %= mod;
                if ((pow & 1) == 1)
                {
                    result = (result * num) % mod;
                    if (pow == 1L)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static long ModPow(this long num, long pow, long mod)
        {
            if (pow < 0L)
                throw new ArgumentOutOfRangeException("pow", "pow cannot be negative");
            if (mod < -3037000500L || mod > 3037000500)
                return (long)((BigInteger)num).ModPow(pow, mod);

            var result = 1L;
            while (pow != 0L)
            {
                num %= mod;
                if ((pow & 1L) == 1L)
                {
                    result = (result * num) % mod;
                    if (pow == 1L)
                        return result;
                }
                num *= num;
                pow >>= 1;
            }
            return result;
        }

        public static int Mod(this int num, int mod)
        {
            var num2 = num % mod;
            if (num2 < 0 == mod > 0)
                return num2 + mod;
            return num2;
        }

        public static long Mod(this long num, long mod)
        {
            var num2 = num % mod;
            if (num2 < 0L == mod > 0L)
                return num2 + mod;
            return num2;
        }

        public static int[] ExtGcd(int m, int n)
        {
            var ma = new[] {m, 1, 0};
            var na = new[] {n, 0, 1};

            while (na[0] != 0)
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

        public static long[] ExtGcd(long m, long n)
        {
            var ma = new[] {m, 1, 0};
            var na = new[] {n, 0, 1};

            while (na[0] != 0)
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

        public static int ModInverse(int num, int mod)
        {
            var g = ExtGcd(mod, num);
            if (g[0] != 1)
                throw new InvalidOperationException("num and mod must be coprime");
            return g[2].Mod(mod);
        }

        public static long ModInverse(long num, long mod)
        {
            var g = ExtGcd(mod, num);
            if (g[0] != 1L)
                throw new InvalidOperationException("num and mod must be coprime");
            return g[2].Mod(mod);
        }

        public static int ChineseRemainder(int[] x, int[] n)
        {
            if (x.Length != n.Length)
                throw new ArgumentException("n must be same size with x", "n");
            var y = x[0];
            var p = n[0];
            for (var i = 1; i < x.Length; i++)
            {
                y = checked(y + ModInverse(p % n[i], n[i]) * p * (x[i] - y));
                p = checked(p * n[i]);
                y %= p;
            }
            return y.Mod(p);
        }

        public static long ChineseRemainder(long[] x, long[] n)
        {
            if (x.Length != n.Length)
                throw new ArgumentException("n must be same size with x", "n");
            var y = x[0];
            var p = n[0];
            for (var i = 1; i < x.Length; i++)
            {
                y = checked(y + ModInverse(p % n[i], n[i]) * p * (x[i] - y));
                p = checked(p * n[i]);
                y %= p;
            }
            return y.Mod(p);
        }

        public static long NextLong(this Random random)
        {
            return random.Next() * 2147483648L + random.Next();
        }

        public static long NextLong(this Random random, long maxValue)
        {
            return random.NextLong().Mod(maxValue);
        }

        public static long NextLong(this Random random, long minValue, long maxValue)
        {
            long result;
            if (minValue < 0L == maxValue > 0L)
            {
                result = random.NextLong(minValue);
                minValue = 0L;
            }
            else
                result = minValue;
            return result + random.NextLong(maxValue - minValue);
        }
    }
}
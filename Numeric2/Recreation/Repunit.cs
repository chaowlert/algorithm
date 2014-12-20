using System;
using System.Linq;
using Chaow.Combinatorics;
using Chaow.Numeric.Sequence;

namespace Chaow.Numeric.Recreation
{
    public struct Repunit
    {
        //fields
        readonly long _length;

        //properties

        //constructors
        public Repunit(long length)
        {
            _length = length;
        }

        public long Length
        {
            get { return _length; }
        }

        //public static methods
        public static Repunit CreateDivisibleBy(long num)
        {
            return CreateDivisibleBy(num, new Prime());
        }

        public static Repunit CreateDivisibleBy(long num, Prime prime)
        {
            if (MathExt.Gcd(num, 10L) != 1L)
                throw new ArgumentException("num must not be divisible by 2 and 5", "num");
            return new Repunit(
                prime.Factors(num)
                     .ToLookup(x => x)
                     .Select(x => x.Key.Power(x.Count() - 1) * fromPrime(x.Key, prime))
                     .Aggregate(MathExt.Lcm)
                );
        }

        //static methods
        static long fromPrime(long num, Prime prime)
        {
            if (num == 3L)
                return 3L;
            return prime.Divisors(num - 1L).OrderBy(x => x).First(x => 10L.ModPow(x, num) == 1L);
        }

        //public methods
        public bool IsDivisibleBy(long num)
        {
            return 10L.ModPow(_length, num * 9L) == 1L;
        }

        public override bool Equals(object obj)
        {
            return obj is Repunit && _length == ((Repunit)obj)._length;
        }

        public override int GetHashCode()
        {
            return (int)_length;
        }

        public override string ToString()
        {
            if (_length <= 16L)
                return new string('1', (int)_length);
            return string.Format("11111111...1{{Length={0}}}", _length);
        }
    }
}
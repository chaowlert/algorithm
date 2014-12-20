using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Chaow.Numeric.Sequence;

namespace Chaow.Numeric.Analysis
{
    class BigPrime : BaseSequence<BigInteger>
    {
        //fields
        static readonly BigInteger maxValue = 2147483647L;
        static readonly BigInteger maxValue2 = 2147483646L;
        readonly IPrimeStore _primeStore;

        //constructors
        public BigPrime()
        {
            _primeStore = new PrimeSieve();
        }

        public BigPrime(IPrimeStore primeStore)
        {
            _primeStore = primeStore;
        }

        //protect methods
        protected override IEnumerable<BigInteger> enumerate()
        {
            return _primeStore.FromRange(0, int.MaxValue).Select(p => (BigInteger)p).Concat(bigEnumerate());
        }

        //private methods
        IEnumerable<BigInteger> bigEnumerate()
        {
            BigInteger start = 2147483648L;
            var end = start + maxValue2;

            while (true)
            {
                foreach (var prime in fromRange(start, end))
                    yield return prime;
                start = end + BigInteger.One;
                end = start + maxValue2;
            }
        }

        IEnumerable<BigInteger> fromRange(BigInteger start, BigInteger end)
        {
            while (end - start > maxValue2)
            {
                var temp = start + maxValue2;
                foreach (long value in fromRange(start, temp))
                    yield return value;
                start = temp + BigInteger.One;
            }

            var limit = end.Sqrt();
            start >>= 1;
            end = (end - BigInteger.One) >> 1;
            var length = (int)(end - start + BigInteger.One);
            var store = new BitArray(length, true);

            Action<int, int> applyPrime = (p, index) =>
            {
                for (; index < length && index >= 0; index += p)
                    store.Set(index, false);
            };

            BigInteger a;
            foreach (long p in TakeWhile(p => p < limit).Skip(1))
            {
                a = p >> 1;
                var b = -((start - a) % p);
                if (b.Sign < 0)
                    b += p;
                applyPrime((int)p, (int)b);
            }

            for (var j = 0; j < length; j++)
            {
                if (store.Get(j))
                {
                    a = ((start + j) << 1) + BigInteger.One;
                    if (a < limit)
                        applyPrime((int)a, j + (int)a);
                    yield return a;
                }
            }
        }

        //public methods
        public bool Contains(BigInteger value)
        {
            if (value <= maxValue)
                return _primeStore.IsPrime((int)value);

            return Factors(value).First() == value;
        }

        static readonly BigInteger Two = 2;
        public IEnumerable<BigInteger> Factors(BigInteger value)
        {
            if (value < Two)
                yield break;

            using (var enumerator = enumerate().GetEnumerator())
            {
                enumerator.MoveNext();

                var factor = enumerator.Current;
                var pow = factor * factor;
                var num = value;

                while (num >= pow)
                {
                    BigInteger mod;
                    var num2 = BigInteger.DivRem(num, factor, out mod);
                    if (mod.IsZero)
                    {
                        num = num2;
                        yield return factor;
                    }
                    else
                    {
                        enumerator.MoveNext();
                        factor = enumerator.Current;
                        pow = factor * factor;
                    }
                }
                yield return num;
            }
        }

        public IEnumerable<BigInteger> FromRange(BigInteger start, BigInteger end)
        {
            var result = Enumerable.Empty<BigInteger>();

            if (end < start)
                return result;
            if (end < Two)
                return result;
            BigInteger threshold = _primeStore.Threshold;
            if (start <= threshold)
            {
                if (threshold > end)
                    threshold = end;
                result = result.Concat(_primeStore.FromRange((int)start, (int)threshold).Select(p => (BigInteger)p));
                start = threshold + BigInteger.One;
            }
            if (end < start)
                return result;
            return result.Concat(fromRange(start, end));
        }
    }
}
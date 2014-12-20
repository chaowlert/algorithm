using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Chaow.Combinatorics;
using Chaow.Extensions;

namespace Chaow.Numeric.Analysis
{
    class BigDivisor : BaseCombinatoric<BigInteger>
    {
        //fields
        readonly ILookup<BigInteger, BigInteger> _factors;
        readonly BigInteger _num;
        readonly BigPrime _prime;

        //properties

        //constructors
        public BigDivisor(BigInteger num) : this(num, new BigPrime())
        {
        }

        public BigDivisor(BigInteger num, BigPrime prime)
        {
            _factors = prime.Factors(num).ToLookup(x => x);
            _num = num;
            _prime = prime;
        }

        public override BigInteger this[long index]
        {
            get
            {
                if (index < 0L || index >= Count)
                    throw new ArgumentOutOfRangeException("index");
                if (index == 0L)
                    return 1L;

                var result = BigInteger.One;
                foreach (var x in _factors)
                {
                    long rem;
                    index = Math.DivRem(index, x.Count() + 1, out rem);
                    result *= x.Key.Power(rem);
                    if (index == 0)
                        break;
                }
                return result;
            }
        }

        public override long LongCount
        {
            get { return _factors.Aggregate(1L, (n, m) => n * (m.Count() + 1L)); }
        }

        //public methods
        public override bool Contains(BigInteger item)
        {
            if (item.Sign <= 0)
                return false;
            if (item == BigInteger.One)
                return true;
            if (_num.Sign <= 0)
                return false;
            return (_num % item).IsZero;
        }

        public override long LongIndexOf(BigInteger item)
        {
            if (item.Sign <= 0)
                return -1L;
            if (item == BigInteger.One)
                return 0L;
            if (_num.Sign <= 0)
                return -1L;
            if (!(_num % item).IsZero)
                return -1L;
            long index = 0L, mult = 1L;
            using (var x = _factors.GetEnumerator())
            {
                using (var y = _prime.Factors(item).ToLookup(z => z).GetEnumerator())
                {
                    y.MoveNext();
                    while (x.MoveNext())
                    {
                        if (x.Current.Key == y.Current.Key)
                        {
                            index += y.Current.Count() * mult;
                            if (!y.MoveNext())
                                break;
                        }
                        mult *= x.Current.Count() + 1L;
                    }
                }
            }
            return index;
        }

        public override IEnumerator<BigInteger> GetEnumerator()
        {
            if (_factors.Count == 0)
                return BigInteger.One.ToEnumerable().GetEnumerator();

            var factors = (from x in _factors
                                          select (from y in 0.To(x.Count())
                                                  select x.Key.Power(y)).ToList()).ToArray();
            Func<int, IEnumerable<BigInteger>> divid = null;
            divid = i => from a in factors[i]
                         from b in divid(i - 1)
                         select a * b;
            divid = divid.When(i => i == 0, i => factors[0]);
            return divid(factors.Length - 1).GetEnumerator();
        }
    }
}
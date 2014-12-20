using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;
using Chaow.Numeric;
using Chaow.Numeric.Sequence;

namespace Chaow.Combinatorics
{
    public sealed class Divisor : BaseCombinatoric<long>
    {
        //fields
        readonly ILookup<long, long> _factors;
        readonly long _num;
        readonly Prime _prime;

        public Divisor(long num) : this(num, new Prime())
        {
        }

        public Divisor(long num, Prime prime)
        {
            _factors = prime.Factors(num).ToLookup(x => x);
            _num = num;
            _prime = prime;
        }

        //properties
        public override long this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");
                if (index == 0)
                    return 1;

                var result = 1L;
                foreach (var x in _factors)
                {
                    int rem;
                    index = Math.DivRem(index, x.Count() + 1, out rem);
                    result *= x.Key.Power(rem);
                    if (index == 0)
                        break;
                }
                return result;
            }
            set { throw new NotSupportedException(); }
        }

        public override long this[long index]
        {
            get
            {
                if (index < 0L || index >= LongCount)
                    throw new ArgumentOutOfRangeException("index");
                return this[(int)index];
            }
        }

        public override int Count
        {
            get { return _factors.Aggregate(1, (n, m) => n * (m.Count() + 1)); }
        }

        public override long LongCount
        {
            get { return Count; }
        }

        //constructors

        //public methods
        public override bool Contains(long item)
        {
            if (item <= 0L)
                return false;
            if (item == 1L)
                return true;
            if (_num <= 0L)
                return false;
            return _num % item == 0L;
        }

        public override int IndexOf(long item)
        {
            if (item <= 0L)
                return -1;
            if (item == 1L)
                return 0;
            if (_num <= 0L)
                return -1;
            if (_num % item != 0L)
                return -1;
            int index = 0, mult = 1;
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
                        mult *= x.Current.Count() + 1;
                    }
                }
            }
            return index;
        }

        public override long LongIndexOf(long item)
        {
            return IndexOf(item);
        }

        public override IEnumerator<long> GetEnumerator()
        {
            if (_factors.Count == 0)
                return 1L.ToEnumerable().GetEnumerator();

            var factors = (from x in _factors
                                    select (from y in 0.To(x.Count())
                                            select x.Key.Power(y)).ToList()).ToArray();
            Func<int, IEnumerable<long>> divid = null;
            divid = i => from a in factors[i]
                         from b in divid(i - 1)
                         select a * b;
            divid = divid.When(i => i == 0, i => factors[0]);
            return divid(factors.Length - 1).GetEnumerator();
        }
    }
}
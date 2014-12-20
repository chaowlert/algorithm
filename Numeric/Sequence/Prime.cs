using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chaow.Numeric.Sequence
{
    public interface IPrimeStore
    {
        int Threshold { get; }
        bool IsPrime(int num);
        IEnumerable<int> FromRange(int start, int end);
    }

    public class PrimeFile : IPrimeStore
    {
        //fields
        readonly string _filePath;

        //properties

        //constructors
        public PrimeFile() : this(AppDomain.CurrentDomain.BaseDirectory + "prime.dat")
        {
        }

        public PrimeFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Cannot find prime file", filePath);
            _filePath = filePath;
        }

        public int Threshold
        {
            get { return int.MaxValue; }
        }

        //public methods
        public bool IsPrime(int num)
        {
            if (num < 2)
                return false;
            if (num == 2)
                return true;
            if ((num & 1) == 0)
                return false;
            using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                int rem;
                fs.Seek(Math.DivRem(num, 16, out rem), SeekOrigin.Begin);
                var value = fs.ReadByte();
                return (value & (1 << (rem >> 1))) > 0;
            }
        }

        public IEnumerable<int> FromRange(int start, int end)
        {
            if (end < start)
                yield break;
            if (end < 2)
                yield break;
            if (start <= 2)
            {
                yield return 2;
                start = 3;
            }
            if ((start & 1) == 0)
                start += 1;
            if (end < start)
                yield break;

            using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                int rem;
                fs.Seek(Math.DivRem(start, 16, out rem), SeekOrigin.Begin);
                var value = fs.ReadByte();
                rem = 1 << (rem >> 1);
                while (start <= end)
                {
                    if ((value & rem) > 0)
                        yield return start;
                    start += 2;
                    if (rem == 128)
                    {
                        rem = 1;
                        value = fs.ReadByte();
                        if (value == -1)
                            break;
                    }
                    else
                        rem <<= 1;
                }
            }
        }
    }

    public class PrimeSieve : IPrimeStore
    {
        //constants
        const int defaultCapacity = 1024;

        //fields
        int _lastIndex = 1;
        BitArray _store;
        int _threshold = 9;

        //properties

        //constructors
        public PrimeSieve()
        {
            _store = new BitArray(defaultCapacity, true);
        }

        public PrimeSieve(int capacity)
        {
            if (capacity < defaultCapacity)
                capacity = defaultCapacity;
            capacity = ((capacity >> 1) + 31) / 32;
            _store = new BitArray(capacity * 32, true);
        }

        public int Threshold
        {
            get { return _threshold; }
        }

        //public methods
        public bool IsPrime(int num)
        {
            if (num < 2L)
                return false;
            if (num == 2L)
                return true;
            if ((num & 1L) == 0L)
                return false;

            var index = num >> 1;
            testPrime(num);
            return _store.Get(index);
        }

        public IEnumerable<int> FromRange(int start, int end)
        {
            if (end < start)
                yield break;
            if (end < 2)
                yield break;
            if (start <= 2)
            {
                yield return 2;
                start = 3;
                if (end < 3)
                    yield break;
            }
            if (start == 3)
            {
                yield return 3;
                start = 5;
            }
            if ((start & 1) == 0)
                start += 1;
            if (end < start)
                yield break;
            var mod = start % 6;
            if (mod == 3)
                start += 2;
            else if (mod == 1)
            {
                if (IsPrime(start))
                    yield return start;
                start += 4;
            }

            while (start <= end)
            {
                if (IsPrime(start))
                    yield return start;
                start += 2;
                if (start > end)
                    break;
                if (IsPrime(start))
                    yield return start;
                start += 4;
            }
        }

        //private methods
        void testPrime(int num)
        {
            if (num <= _threshold)
                return;

            int index;
            if (num >= 2147302921)
            {
                index = 23170;
                _threshold = int.MaxValue;
            }
            else
            {
                index = (num.Sqrt() + 1) >> 1;
                _threshold = ((index << 1) + 1).Power(2) - 1;
            }
            ensureCapacity(_threshold >> 1);

            for (; _lastIndex < index; _lastIndex++)
            {
                if (_store.Get(_lastIndex))
                {
                    var prime = (_lastIndex << 1) + 1;
                    applyPrime(prime, (prime * prime) >> 1);
                }
            }
        }

        void ensureCapacity(int nextCapacity)
        {
            var maxIndex = _store.Count - 1;
            if (nextCapacity <= maxIndex)
                return;

            var newCapacity = _store.Count;
            while (nextCapacity > newCapacity)
                newCapacity *= 2;
            if (newCapacity > 1073741823)
                newCapacity = 1073741823;

            var newSize = (newCapacity + 31) / 32;
            var array = new int[newSize];
            _store.CopyTo(array, 0);
            for (var i = _store.Count / 32; i < newSize; i++)
                array[i] = -1;
            _store = new BitArray(array);

            for (var index = 1; index < _lastIndex; index++)
            {
                if (_store.Get(index))
                {
                    var prime = (index << 1) + 1;
                    applyPrime(prime, maxIndex - ((maxIndex - index) % prime) + prime);
                }
            }
        }

        void applyPrime(int prime, int index)
        {
            var capacity = _store.Count;
            for (; index < capacity; index += prime)
                _store.Set(index, false);
        }
    }

    public sealed class Prime : BaseSequence<long>
    {
        //fields
        readonly IPrimeStore _primeStore;

        //constructors
        public Prime()
        {
            _primeStore = new PrimeSieve();
        }

        public Prime(IPrimeStore primeStore)
        {
            _primeStore = primeStore;
        }

        //protect methods
        protected override IEnumerable<long> enumerate()
        {
            return _primeStore.FromRange(0, int.MaxValue).Select(p => (long)p).Concat(FromRange(2147483648L, long.MaxValue));
        }

        //private methods
        IEnumerable<long> fromRange(long start, long end)
        {
            while (end - start > 2147483646L)
            {
                var temp = start + 2147483646L;
                foreach (var value in fromRange(start, temp))
                    yield return value;
                start = temp + 1L;
            }

            var limit = Math.Min(start, end.Sqrt());
            start >>= 1;
            end = (end - 1L) >> 1;
            var length = (int)(end - start + 1L);
            var store = new BitArray(length, true);

            Action<int, int> applyPrime = (p, index) =>
            {
                for (; index < length && index >= 0; index += p)
                    store.Set(index, false);
            };

            long a;
            foreach (var p in TakeWhile(p => p < limit).Skip(1))
            {
                a = p >> 1;
                var b = -((start - a) % p);
                if (b < 0)
                    b += p;
                applyPrime((int)p, (int)b);
            }

            for (var j = 0; j < length; j++)
            {
                if (store.Get(j))
                {
                    a = ((start + j) << 1) + 1L;
                    if (a >= limit)
                        applyPrime((int)a, j + (int)a);
                    yield return a;
                }
            }
        }

        //public methods
        public bool Contains(long value)
        {
            if (value <= 2147483647L)
                return _primeStore.IsPrime((int)value);

            return Factors(value).First() == value;
        }

        public IEnumerable<long> Factors(long value)
        {
            if (value < 2L)
                yield break;

            using (var enumerator = enumerate().GetEnumerator())
            {
                enumerator.MoveNext();

                var factor = enumerator.Current;
                var pow = factor * factor;
                var num = value;

                while (num >= pow)
                {
                    long mod;
                    var num2 = Math.DivRem(num, factor, out mod);
                    if (mod == 0L)
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

        public IEnumerable<long> FromRange(long start, long end)
        {
            var result = Enumerable.Empty<long>();

            if (end < start)
                return result;
            if (end < 2L)
                return result;
            var threshold = Math.Max(_primeStore.Threshold, end.Sqrt());
            if (start <= threshold)
            {
                if (threshold > end)
                    threshold = end;
                result = result.Concat(_primeStore.FromRange((int)start, (int)threshold).Select(p => (long)p));
                start = threshold + 1L;
            }
            if (end < start)
                return result;
            return result.Concat(fromRange(start, end));
        }
    }
}
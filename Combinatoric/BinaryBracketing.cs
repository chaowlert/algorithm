using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    public sealed class BinaryBracketing<T> : BaseCombinatoric<T>
    {
        //fields
        readonly IEnumerable<Func<T, T, T>> _operations;
        readonly IEnumerable<T> _source;
        IEqualityComparer<T> _comparer;

        public BinaryBracketing(IEnumerable<T> source, Func<T, T, T> operation) : this(source, Enumerable.Repeat(operation, source.Count()))
        {
        }

        public BinaryBracketing(IEnumerable<T> source, IEnumerable<Func<T, T, T>> operations)
        {
            _source = source;
            _operations = operations;
        }

        //properties
        public override T this[long index]
        {
            get
            {
                if (index < 0L || index >= count(_source))
                    throw new ArgumentOutOfRangeException("index");
                return elementAt(_source, _operations, _source.Count(), index);
            }
        }

        public override long LongCount
        {
            get { return count(_source); }
        }

        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }

        //constructors

        //static methods
        static T elementAt(IEnumerable<T> source, IEnumerable<Func<T, T, T>> operations, int lenght, long index)
        {
            if (lenght == 1)
                return source.First();

            var item = default(T);

            for (var i = 1; i < lenght; i++)
            {
                var leftCount = count(source.Take(i));
                var rightCount = count(source.Skip(i));
                var product = leftCount * rightCount;
                if (index < product)
                {
                    long rightIndex;
                    var leftIndex = Math.DivRem(index, rightCount, out rightIndex);
                    var leftItem = elementAt(source.Take(i), operations.Skip(1), i, leftIndex);
                    var rightItem = elementAt(source.Skip(i), operations.Skip(i), lenght - i, rightIndex);
                    item = operations.First()(leftItem, rightItem);
                    break;
                }
                index -= product;
            }
            return item;
        }

        static long count(IEnumerable<T> source)
        {
            var length = source.Count() - 1L;
            return Combinatoric.Choose(2L * length, length) / (length + 1L);
        }

        static IEnumerable<T> binaryBracketing(IEnumerable<T> source, IEnumerable<Func<T, T, T>> operations, int length)
        {
            if (length == 1)
            {
                yield return source.First();
                yield break;
            }

            for (var i = 1; i < length; i++)
            {
                foreach (var left in binaryBracketing(source.Take(i), operations.Skip(1), i))
                {
                    foreach (var right in binaryBracketing(source.Skip(i), operations.Skip(i), length - i))
                        yield return operations.First()(left, right);
                }
            }
        }

        //public methods
        public override bool Contains(T item)
        {
            return this.Contains(item, _comparer);
        }

        public override long LongIndexOf(T item)
        {
            return this.LongIndexOf(item, _comparer);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            var length = _source.Count();
            var opLength = _operations.Count();

            if (opLength < length - 1)
                throw new OverflowException("Operation length cannot be less than source length - 1");

            return binaryBracketing(_source, _operations, length).GetEnumerator();
        }
    }
}
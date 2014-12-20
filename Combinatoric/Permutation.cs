using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Numeric;

namespace Chaow.Combinatorics
{
    public class Permutation<T> : BaseCombinatoric<IEnumerable<T>>
    {
        //fields
        readonly int _length;
        readonly IEnumerable<T> _source;
        IEqualityComparer<T> _comparer;
        CombinatoricModel _model;

        public Permutation(IEnumerable<T> source) : this(source, source.Count(), CombinatoricModel.Normal)
        {
        }

        public Permutation(IEnumerable<T> source, int length) : this(source, length, CombinatoricModel.Normal)
        {
        }

        public Permutation(IEnumerable<T> source, int length, CombinatoricModel model)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length must be greater than or equal to 0");
            _source = source;
            _length = length;
            _model = model;
            _comparer = EqualityComparer<T>.Default;
        }

        //properties
        public override IEnumerable<T> this[long index]
        {
            get
            {
                if (index < 0L || index >= count(_source, _length, _model))
                    throw new ArgumentOutOfRangeException("index");
                var backtracker = getBacktracker();
                var remaining = _source;
                var func = backtracker.SourceSelector;
                backtracker.SourceSelector = (source, current, item, i) => remaining = func(source, current, item, i);
                backtracker.AppendBreak(current =>
                {
                    var c = count(remaining, current, _model);
                    if (index < c)
                        return false;
                    index -= c;
                    return true;
                });
                return backtracker.First();
            }
        }

        public override long LongCount
        {
            get { return count(_source, _length, _model); }
        }

        public CombinatoricModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }

        //constructors

        //static methods
        static long count(IEnumerable<T> source, int length, CombinatoricModel model)
        {
            switch (model)
            {
                case CombinatoricModel.Normal:
                    return Combinatoric.FallingFactorial((long)source.Count(), length);
                case CombinatoricModel.Repetition:
                    return ((long)source.Count()).Power(length);
                case CombinatoricModel.Distinct:
                    var lookup = source.ToLookup(x => x);
                    return distinctCount(lookup.Select(x => x.LongCount()).OrderBy(x => x).ToList(), length, lookup.Count, source.Count() - length);
                case CombinatoricModel.RepetitionDistinct:
                    return source.Distinct().LongCount().Power(length);
                default:
                    return 0L;
            }
        }

        static long distinctCount(IEnumerable<long> source, long length, long remaining, long skippable)
        {
            if (skippable < 0L)
                return 0L;
            if (skippable == 0L)
                return Combinatoric.Multinomial(source);

            var count = source.First();
            if (count >= length)
                return remaining.Power((int)length);
            var others = source.Skip(1);

            var result = distinctCount(others, length, remaining - 1L, skippable - count);
            for (var x = 1L; x <= count; x++)
            {
                var temp = distinctCount(others, length - x, remaining - 1L, skippable + x - count);
                result = checked(result + Combinatoric.Choose(length, x) * temp);
            }
            return result;
        }

        //private methods
        Backtracker<T, int> getBacktracker()
        {
            var backtracker = _source.Backtrack(
                _length,
                (len, item, i) => len - 1,
                len => len == 0
                );
            if ((_model & CombinatoricModel.Repetition) == CombinatoricModel.Repetition)
                backtracker.BacktrackingModel = BacktrackingModel.PermutationWithRepetition;
            else
                backtracker.BacktrackingModel = BacktrackingModel.Permutation;
            if ((_model & CombinatoricModel.Distinct) == CombinatoricModel.Distinct)
                backtracker.Distinct = true;
            return backtracker;
        }

        //public methods
        public override bool Contains(IEnumerable<T> item)
        {
            return LongIndexOf(item) >= 0;
        }

        public override long LongIndexOf(IEnumerable<T> item)
        {
            if (item.Count() != _length)
                return -1;
            var backtracker = getBacktracker();
            long index = 0;
            var remaining = _source;
            var @break = false;
            var func = backtracker.SourceSelector;
            backtracker.SourceSelector = (source, current, t, i) => remaining = func(source, current, t, i);
            backtracker.AppendBreak(current =>
            {
                if (@break)
                    index += count(remaining, current, _model);
                return @break;
            });
            backtracker.AppendConstraint(current => t =>
            {
                if (_comparer.Equals(t, item.First()))
                {
                    item = item.Skip(1);
                    @break = false;
                }
                else
                    @break = true;
                return true;
            });
            if (backtracker.Any())
                return index;
            return -1;
        }

        public override IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            if (_source.Count() < _length && (_model & CombinatoricModel.Repetition) != CombinatoricModel.Repetition)
                return Enumerable.Empty<IEnumerable<T>>().GetEnumerator();

            return getBacktracker().GetEnumerator();
        }
    }
}
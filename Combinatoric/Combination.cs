using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Combinatorics
{
    public sealed class Combination<T> : BaseCombinatoric<IEnumerable<T>>
    {
        //fields
        readonly int _length;
        readonly IEnumerable<T> _source;
        IEqualityComparer<T> _comparer;
        CombinatoricModel _model;

        public Combination(IEnumerable<T> source, int length) : this(source, length, CombinatoricModel.Normal)
        {
        }

        public Combination(IEnumerable<T> source, int length, CombinatoricModel model)
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
                    var c = count(remaining, current.Item1, _model);
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
                    return Combinatoric.Choose((long)source.Count(), length);
                case CombinatoricModel.Repetition:
                    return Combinatoric.Choose((long)source.Count() + length - 1L, length);
                case CombinatoricModel.Distinct:
                    var lookup = source.ToLookup(x => x);
                    return distinctCount(lookup.Select(x => x.LongCount()).OrderBy(x => x).ToList(), length, lookup.Count, source.Count() - length);
                case CombinatoricModel.RepetitionDistinct:
                    return Combinatoric.Choose(source.Distinct().LongCount() + length - 1, length);
                default:
                    return 0L;
            }
        }

        static long distinctCount(IEnumerable<long> source, long length, long remaining, long skippable)
        {
            if (skippable < 0L)
                return 0L;
            if (skippable == 0L)
                return 1L;

            var count = source.First();
            if (count >= length)
                return Combinatoric.Choose(remaining + length - 1L, length);
            var others = source.Skip(1);

            var result = distinctCount(others, length, remaining - 1L, skippable - count);
            for (var x = 1L; x <= count; x++)
            {
                var temp = distinctCount(others, length - x, remaining - 1L, skippable + x - count);
                result = checked(result + temp);
            }
            return result;
        }

        //private method
        Backtracker<T, Tuple<int, int>> getBacktracker()
        {
            var backtracker = _source.Backtrack(
                Tuple.Create(_length, _source.Count() - _length),
                (t, item, i) => Tuple.Create(t.Item1 - 1, t.Item2 - i),
                t => t.Item1 == 0
                );
            if ((_model & CombinatoricModel.Repetition) == CombinatoricModel.Repetition)
                backtracker.BacktrackingModel = BacktrackingModel.CombinationWithRepetition;
            else
            {
                backtracker.AppendBreak(current => current.Item2 < 0);
                backtracker.BacktrackingModel = BacktrackingModel.Combination;
            }
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
                    index += count(remaining, current.Item1, _model);
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
            return getBacktracker().GetEnumerator();
        }
    }
}
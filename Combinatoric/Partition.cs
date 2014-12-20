using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    public sealed class Partition : BaseCombinatoric<IEnumerable<int>>
    {
        //fields
        readonly IEnumerable<int> _source;
        readonly int _sum;

        public Partition(IEnumerable<int> source, int sum)
        {
            if (source.Any(x => x <= 0))
                throw new ArgumentException("source cannot contain items value less than 1", "source");
            _source = source;
            _sum = sum;
        }

        //properties
        public override IEnumerable<int> this[long index]
        {
            get
            {
                if (index < 0L || index >= count(_source, _sum))
                    throw new ArgumentOutOfRangeException("index");
                var backtracker = getBacktracker();
                var remaining = _source;
                var func = backtracker.SourceSelector;
                backtracker.SourceSelector = (source, current, item, i) => remaining = func(source, current, item, i);
                backtracker.AppendBreak(current =>
                {
                    var c = count(remaining, current);
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
            get { return count(_source, _sum); }
        }

        //constructors

        //static methods
        static long count(IEnumerable<int> source, int sum)
        {
            Func<int, int, long> func = null;
            func = (skip, remaining) => source.Skip(skip).Select((x, i) => (x > remaining) ? 0L : func(skip + i, remaining - x)).Sum();
            func = func.Memoize();
            func = func.When((skip, remaining) => remaining == 0, (maxUnit, remaining) => 1L);
            return func(0, sum);
        }

        //private methods
        Backtracker<int, int> getBacktracker()
        {
            var backtracker = _source.Backtrack(
                _sum,
                (remaining, item, i) => remaining - item,
                remaining => remaining == 0
                );
            backtracker.AppendConstraint(remaining => item => item <= remaining);
            backtracker.BacktrackingModel = BacktrackingModel.CombinationWithRepetition;
            return backtracker;
        }

        //public methods
        public override bool Contains(IEnumerable<int> item)
        {
            return LongIndexOf(item) >= 0L;
        }

        public override long LongIndexOf(IEnumerable<int> item)
        {
            if (item.Sum() != _sum)
                return -1L;
            var backtracker = getBacktracker();
            long index = 0;
            var remaining = _source;
            var @break = false;
            var func = backtracker.SourceSelector;
            backtracker.SourceSelector = (source, current, t, i) => remaining = func(source, current, t, i);
            backtracker.AppendBreak(current =>
            {
                if (@break)
                    index += count(remaining, current);
                return @break;
            });
            backtracker.AppendConstraint(current => t =>
            {
                if (t == item.First())
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

        public override IEnumerator<IEnumerable<int>> GetEnumerator()
        {
            return getBacktracker().GetEnumerator();
        }
    }
}
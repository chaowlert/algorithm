using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    public enum BacktrackingModel
    {
        PermutationWithRepetition,
        Permutation,
        CombinationWithRepetition,
        Combination,
        Custom,
    }

    public class Backtracker<T1, T2> : IEnumerable<IEnumerable<T1>>
    {
        //fields
        readonly List<Tuple<Func<T2, bool>, Func<T2, Func<T1, bool>>>>
            _constraints = new List<Tuple<Func<T2, bool>, Func<T2, Func<T1, bool>>>>();
        readonly Func<T2, T1, int, T2> _func;
        readonly T2 _seed;
        readonly IEnumerable<T1> _source;
        readonly Func<T2, bool> _yield;
        BacktrackingModel _backtrackingModel;
        Func<T2, bool> _break;
        IEqualityComparer<T1> _comparer;
        bool _continueOnYielded;
        bool _distinct;
        Func<IEnumerable<T1>, T2, T1, int, IEnumerable<T1>> _sourceSelector;

        //constructors
        public Backtracker(IEnumerable<T1> source, T2 seed, Func<T2, T1, int, T2> func, Func<T2, bool> yield)
        {
            _source = source;
            _seed = seed;
            _func = func;
            _yield = yield;
            _break = FuncExt.False;
            BacktrackingModel = BacktrackingModel.PermutationWithRepetition;
            _comparer = EqualityComparer<T1>.Default;
        }

        //properties
        protected IEnumerable<T1> Source
        {
            get { return _source; }
        }

        protected T2 Seed
        {
            get { return _seed; }
        }

        protected Func<T2, T1, int, T2> Func
        {
            get { return _func; }
        }

        protected Func<T2, bool> Yield
        {
            get { return _yield; }
        }

        protected Func<T2, bool> Break
        {
            get { return _break; }
        }

        public bool ContinueOnYielded
        {
            get { return _continueOnYielded; }
            set { _continueOnYielded = value; }
        }

        public BacktrackingModel BacktrackingModel
        {
            get { return _backtrackingModel; }
            set
            {
                switch (value)
                {
                    case BacktrackingModel.PermutationWithRepetition:
                        _sourceSelector = (source, current, item, i) => source;
                        break;
                    case BacktrackingModel.Permutation:
                        _sourceSelector = (source, current, item, i) => source.Where((x, y) => y != i);
                        break;
                    case BacktrackingModel.CombinationWithRepetition:
                        _sourceSelector = (source, current, item, i) => source.Skip(i);
                        break;
                    case BacktrackingModel.Combination:
                        _sourceSelector = (source, current, item, i) => source.Skip(i + 1);
                        break;
                    default:
                        break;
                }
                _backtrackingModel = value;
            }
        }

        public bool Distinct
        {
            get { return _distinct; }
            set { _distinct = value; }
        }

        public IEqualityComparer<T1> Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }

        public Func<IEnumerable<T1>, T2, T1, int, IEnumerable<T1>> SourceSelector
        {
            get { return _sourceSelector; }
            set
            {
                _backtrackingModel = BacktrackingModel.Custom;
                _sourceSelector = value;
            }
        }

        public virtual IEnumerator<IEnumerable<T1>> GetEnumerator()
        {
            if (_distinct)
            {
                IEnumerable<T1> tmpSource = _source.ToLookup(x => x, _comparer).SelectMany(x => x).ToList();
                return backtrackDistinct(tmpSource, _seed, GetPredicate(_seed)).GetEnumerator();
            }
            return backtrack(_source, _seed, GetPredicate(_seed)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //public methods
        public Backtracker<T1, T2> AppendBreak(Func<T2, bool> @break)
        {
            _break = _break.Equals((Func<T2, bool>)FuncExt.False) ? @break : _break.Or(@break);
            return this;
        }

        public Backtracker<T1, T2> AppendConstraint(Func<T2, Func<T1, bool>> constraint)
        {
            return AppendConstraint(x => true, constraint);
        }

        public Backtracker<T1, T2> AppendConstraint(Func<T2, bool> predicate, Func<T2, Func<T1, bool>> constraint)
        {
            _constraints.Add(Tuple.Create(predicate, constraint));
            return this;
        }

        public virtual IEnumerable<T2> SelectResults()
        {
            if (_distinct)
            {
                IEnumerable<T1> tmpSource = _source.ToLookup(x => x, _comparer).SelectMany(x => x).ToList();
                return selectResultsDistinct(tmpSource, _seed, GetPredicate(_seed));
            }
            return selectResults(_source, _seed, GetPredicate(_seed));
        }

        public IEnumerable<TResult> SelectResults<TResult>(Func<T2, TResult> selector)
        {
            return SelectResults().Select(selector);
        }

        //protected methods
        protected Func<T1, bool> GetPredicate(T2 condition)
        {
            return (from x in _constraints
                    where x.Item1(condition)
                    select x.Item2(condition)).PredicateAll();
        }

        //private methods
        IEnumerable<IEnumerable<T1>> backtrack(IEnumerable<T1> source, T2 current, Func<T1, bool> predicate)
        {
            if (_break(current))
                yield break;
            if (_yield(current))
            {
                yield return Enumerable.Empty<T1>();
                if (!_continueOnYielded)
                    yield break;
            }

            var i = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    var next = _func(current, item, i);
                    foreach (var others in backtrack(_sourceSelector(source, next, item, i), next, GetPredicate(next)))
                        yield return item.ToEnumerable().Concat(others);
                }
                i++;
            }
        }

        IEnumerable<IEnumerable<T1>> backtrackDistinct(IEnumerable<T1> source, T2 current, Func<T1, bool> predicate)
        {
            if (_break(current))
                yield break;
            if (_yield(current))
            {
                yield return Enumerable.Empty<T1>();
                if (!_continueOnYielded)
                    yield break;
            }

            var i = 0;
            var last = default(T1);
            foreach (var item in source)
            {
                if (!_comparer.Equals(last, item) || i == 0)
                {
                    last = item;
                    if (predicate(item))
                    {
                        var next = _func(current, item, i);
                        foreach (var others in backtrackDistinct(_sourceSelector(source, next, item, i), next, GetPredicate(next)))
                            yield return item.ToEnumerable().Concat(others);
                    }
                }
                i++;
            }
        }

        IEnumerable<T2> selectResults(IEnumerable<T1> source, T2 current, Func<T1, bool> predicate)
        {
            if (_break(current))
                yield break;
            if (_yield(current))
            {
                yield return current;
                if (!_continueOnYielded)
                    yield break;
            }

            var i = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    var next = _func(current, item, i);
                    foreach (var others in selectResults(_sourceSelector(source, next, item, i), next, GetPredicate(next)))
                        yield return others;
                }
                i++;
            }
        }

        IEnumerable<T2> selectResultsDistinct(IEnumerable<T1> source, T2 current, Func<T1, bool> predicate)
        {
            if (_break(current))
                yield break;
            if (_yield(current))
            {
                yield return current;
                if (!_continueOnYielded)
                    yield break;
            }

            var i = 0;
            var last = default(T1);
            foreach (var item in source)
            {
                if (!_distinct || !_comparer.Equals(last, item) || i == 0)
                {
                    last = item;
                    if (predicate(item))
                    {
                        var next = _func(current, item, i);
                        foreach (var others in selectResultsDistinct(_sourceSelector(source, next, item, i), next, GetPredicate(next)))
                            yield return others;
                    }
                }
                i++;
            }
        }
    }
}
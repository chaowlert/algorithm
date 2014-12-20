using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.LINCON
{
    public class LookAheadSolver<T>
    {
        //variables
        static readonly IEqualityComparer<T> tComparer = EqualityComparer<T>.Default;
        static readonly Func<List<List<List<T>>>, bool> emptyPredicate = Enumerable.Empty<Func<List<List<List<T>>>, bool>>().PredicateAll();
        readonly Dictionary<long, List<HashSet<long>>> _allDifferents
            = new Dictionary<long, List<HashSet<long>>>();
        readonly Dictionary<long, List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>> _arcConstraints
            = new Dictionary<long, List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>>();
        readonly Dictionary<long, List<Func<List<List<List<T>>>, bool>>> _nodeConstraints
            = new Dictionary<long, List<Func<List<List<List<T>>>, bool>>>();
        readonly List<HashSet<long>> _occurrents
            = new List<HashSet<long>>();
        readonly Dictionary<long, List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>> _pairConstraints
            = new Dictionary<long, List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>>();
        readonly List<List<IEnumerable<T>>> _variables = new List<List<IEnumerable<T>>>();
        bool _noSolution;

        //constructor
        public LookAheadSolver()
        {
            _variables.Add(new List<IEnumerable<T>>());
        }

        public LookAheadSolver<T> AppendVariable(IEnumerable<T> domains)
        {
            _variables[0].Add(domains);
            return this;
        }

        public LookAheadSolver<T> AppendList(IEnumerable<T> domains, int count)
        {
            var id = _variables.Count;
            _variables.Add(new List<IEnumerable<T>>(count));
            for (var i = 0; i < count; i++)
                _variables[id].Add(domains);
            return this;
        }

        public LookAheadSolver<T> AppendNodeConstraint(long id, Func<List<List<List<T>>>, bool> nodeConstraint)
        {
            if (!_nodeConstraints.ContainsKey(id))
                _nodeConstraints.Add(id, new List<Func<List<List<List<T>>>, bool>>());
            _nodeConstraints[id].Add(nodeConstraint);
            return this;
        }

        public LookAheadSolver<T> AppendArcConstraint(HashSet<long> ids, Func<List<List<List<T>>>, bool> arcConstraint)
        {
            if (ids.Count == 2)
            {
                foreach (var kvp in ids)
                {
                    if (!_pairConstraints.ContainsKey(kvp))
                        _pairConstraints.Add(kvp, new List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>());
                    _pairConstraints[kvp].Add(Tuple.Create(ids, arcConstraint));
                }
            }
            else
            {
                foreach (var kvp in ids)
                {
                    if (!_arcConstraints.ContainsKey(kvp))
                        _arcConstraints.Add(kvp, new List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>>());
                    _arcConstraints[kvp].Add(Tuple.Create(ids, arcConstraint));
                }
            }
            return this;
        }

        internal LookAheadSolver<T> AppendAllDifferent(HashSet<long> ids)
        {
            foreach (var kvp in ids)
            {
                if (!_allDifferents.ContainsKey(kvp))
                    _allDifferents.Add(kvp, new List<HashSet<long>>());
                _allDifferents[kvp].Add(ids);
            }
            var set = ids.Select(id => _variables[id.GetHighInt()][id.GetLowInt()]).Aggregate((a, b) => a.Union(b, tComparer)).ToArray();
            if (set.Length < ids.Count)
                _noSolution = true;
            else if (set.Length == ids.Count)
                _occurrents.Add(ids);
            return this;
        }

        //local methods
        IEnumerable<IEnumerable<Tuple<long, T>>> lookAhead(List<List<List<T>>> data, long[] unsolved, HashSet<long> solved)
        {
            //all solved, return empty
            if (unsolved.Length == 0)
            {
                yield return Tuple.Create(IntExt.CreateLong(0, _variables[0].Count), default(T)).ToEnumerable();
                yield break;
            }

            //do on first time
            if (solved.Count == 0)
            {
                var newData = data.Select(d => d.ToList()).ToList();
                foreach (var node in _nodeConstraints)
                    filterData(newData, node.Key, node.Value.PredicateAll(), null);
                unsolved = unsolved.OrderBy(x => getData(newData, x).Count).ToArray();
                data = newData;
            }

            //pull out first unsolve
            var index = unsolved[0];
            var candidate = getData(data, index);
            var newUnsolve = new long[unsolved.Length - 1];
            Array.Copy(unsolved, 1, newUnsolve, 0, newUnsolve.Length);
            var newSolved = new HashSet<long>(solved) {index};

            //get predicate
            List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>> arc;
            _arcConstraints.TryGetValue(index, out arc);
            List<Tuple<HashSet<long>, Func<List<List<List<T>>>, bool>>> pair;
            _pairConstraints.TryGetValue(index, out pair);
            List<HashSet<long>> diff;
            _allDifferents.TryGetValue(index, out diff);

            foreach (var t in candidate)
            {
                //create new data set
                var newData = data.Select(d => d.ToList()).ToList();
                setData(newData, index, new List<T> {t});

                //find minimum scope
                var minValue = int.MaxValue;
                var minUnSolve = 0;

                //set occurrents
                List<Dictionary<T, int>> valueCheck = null;
                if (_occurrents.Count > 0)
                {
                    valueCheck = new List<Dictionary<T, int>>(_occurrents.Count);
                    for (var i = 0; i < _occurrents.Count; i++)
                        valueCheck.Add(new Dictionary<T, int>(tComparer));
                }

                //for each unsolve
                for (var j = 0; j < newUnsolve.Length; j++)
                {
                    var prediList = Enumerable.Empty<Func<List<List<List<T>>>, bool>>();
                    var k = newUnsolve[j];

                    //apply predicate
                    if (diff != null)
                        prediList = diff.Where(a => a.Contains(k))
                                        .Take(1)
                                        .Select(a => new Func<List<List<List<T>>>, bool>(x => !tComparer.Equals(x[k.GetHighInt()][k.GetLowInt()][0], t)));
                    if (pair != null)
                        prediList = prediList.Concat(pair.Where(a => a.Item1.Contains(k))
                                                         .Take(1)
                                                         .Select(a => a.Item2));
                    if (arc != null)
                        prediList = prediList.Concat(arc.Where(a => a.Item1.All(b => b == k || newSolved.Contains(b)))
                                                        .Select(a => a.Item2));
                    var predicate = prediList.PredicateAll();

                    //if predicate is not empty
                    if (!Equals(predicate, emptyPredicate))
                        filterData(newData, k, predicate, val => setValueCheck(valueCheck, val, j, newUnsolve[j]));
                    else if (_occurrents.Count > 0)
                    {
                        var oldList = getData(newData, k);
                        foreach (var item in oldList)
                            setValueCheck(valueCheck, item, j, newUnsolve[j]);
                    }

                    //set minimum scope
                    if (j == 0 || getData(newData, k).Count < minValue)
                    {
                        minUnSolve = j;
                        minValue = getData(newData, k).Count;
                    }
                }

                //apply occurents
                if (_occurrents.Count > 0)
                {
                    foreach (var c in valueCheck)
                    {
                        foreach (var kvp in c)
                        {
                            if (kvp.Value == -1)
                                continue;
                            var k = newUnsolve[kvp.Value];
                            setData(newData, k, new List<T> {kvp.Key});
                            if (minValue > 1)
                            {
                                minValue = 1;
                                minUnSolve = kvp.Value;
                            }
                        }
                    }
                }

                //swop item based on minimum scope
                if (minUnSolve > 0)
                {
                    var tmp = newUnsolve[0];
                    newUnsolve[0] = newUnsolve[minUnSolve];
                    newUnsolve[minUnSolve] = tmp;
                }

                //iterate next item
                foreach (var r in lookAhead(newData, newUnsolve, newSolved))
                    yield return Tuple.Create(index, t).ToEnumerable().Concat(r);
            }
        }

        void setValueCheck(List<Dictionary<T, int>> valueCheck, T key, int j, long unsolve)
        {
            for (var m = 0; m < _occurrents.Count; m++)
            {
                if (!_occurrents[m].Contains(unsolve))
                    continue;
                int value;
                if (!valueCheck[m].TryGetValue(key, out value))
                    valueCheck[m].Add(key, j);
                else if (value != -1)
                    valueCheck[m][key] = -1;
            }
        }

        List<T> getData(List<List<List<T>>> data, long k)
        {
            return data[k.GetHighInt()][k.GetLowInt()];
        }

        void setData(List<List<List<T>>> data, long k, List<T> value)
        {
            data[k.GetHighInt()][k.GetLowInt()] = value;
        }

        void filterData(List<List<List<T>>> data, long k, Func<List<List<List<T>>>, bool> predicate, Action<T> action)
        {
            //create new list of values
            var oldList = getData(data, k);
            var newList = new List<T>(oldList.Count) {default(T)};
            setData(data, k, newList);

            //for each value
            foreach (var item in oldList)
            {
                newList[0] = item;
                if (predicate(data))
                {
                    newList.Add(item);
                    if (action != null)
                        action(item);
                }
            }

            //remove default value
            newList[0] = newList[newList.Count - 1];
            newList.RemoveAt(newList.Count - 1);
        }

        //public methods
        public virtual IEnumerable<List<ReadOnlyCollection<T>>> LookAhead()
        {
            if (_noSolution)
                return Enumerable.Empty<List<ReadOnlyCollection<T>>>();

            var data = _variables.Select(a =>
                a.Select(b => b.ToList()).ToList()
                ).ToList();
            return lookAhead(
                data,
                _variables.SelectMany((a, i) =>
                    a.Select((b, j) => IntExt.CreateLong(i, j))
                    ).ToArray(),
                new HashSet<long>()
                ).Select(s =>
                    s.GroupBy(a => a.Item1.GetHighInt()).OrderBy(a => a.Key).Select(a =>
                        a.OrderBy(b => b.Item1.GetLowInt()).Select(b => b.Item2).ToList().AsReadOnly()
                        ).ToList()
                );
        }
    }
}
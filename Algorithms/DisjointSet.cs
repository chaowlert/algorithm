using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaow.Algorithms
{
    public class DisjointSet<T> : IEnumerable<IEnumerable<T>>
    {
        //fields
        readonly IEqualityComparer<T> _comparer;
        readonly Dictionary<T, List<T>> _groups;
        readonly Dictionary<T, T> _set;

        //properties

        //constructors
        public DisjointSet() : this(0, null)
        {
        }

        public DisjointSet(int capacity) : this(capacity, null)
        {
        }

        public DisjointSet(IEqualityComparer<T> comparer) : this(0, comparer)
        {
        }

        public DisjointSet(int capacity, IEqualityComparer<T> comparer)
        {
            _set = new Dictionary<T, T>(capacity, comparer);
            _groups = new Dictionary<T, List<T>>(comparer);
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public int GroupCount
        {
            get { return _groups.Count; }
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return _groups.Values.Select(list => new ReadOnlyCollection<T>(list).AsEnumerable()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //public methods
        public T FindParent(T v)
        {
            var k = _set[v];
            if (_comparer.Equals(k, v))
                return k;
            return _set[v] = FindParent(k);
        }

        public bool IsUnion(T u, T v)
        {
            if (_set.ContainsKey(u) && _set.ContainsKey(v))
            {
                T k1 = FindParent(u), k2 = FindParent(v);
                return _comparer.Equals(k1, k2);
            }
            return _comparer.Equals(u, v);
        }

        public bool Union(T u, T v)
        {
            if (_set.ContainsKey(u))
            {
                if (_set.ContainsKey(v))
                {
                    T k1 = FindParent(u), k2 = FindParent(v);
                    if (_comparer.Equals(k1, k2))
                        return false;
                    if (_groups[k1].Count >= _groups[k2].Count)
                    {
                        _set[k2] = k1;
                        _groups[k1].AddRange(_groups[k2]);
                        _groups.Remove(k2);
                    }
                    else
                    {
                        _set[k1] = k2;
                        _groups[k2].AddRange(_groups[k1]);
                        _groups.Remove(k1);
                    }
                }
                else
                {
                    var k = FindParent(u);
                    _set.Add(v, k);
                    _groups[k].Add(v);
                }
            }
            else if (_set.ContainsKey(v))
            {
                var k = FindParent(v);
                _set.Add(u, k);
                _groups[k].Add(u);
            }
            else if (_comparer.Equals(u, v))
                return false;
            else
            {
                _set.Add(u, u);
                _set.Add(v, u);
                _groups.Add(u, new List<T> {u, v});
            }
            return true;
        }

        public int SizeOf(T v)
        {
            if (!_set.ContainsKey(v))
                return 0;
            return _groups[FindParent(v)].Count;
        }

        public IEnumerable<T> GroupOf(T v)
        {
            if (!_set.ContainsKey(v))
                return Enumerable.Empty<T>();
            return new ReadOnlyCollection<T>(_groups[FindParent(v)]);
        }
    }
}
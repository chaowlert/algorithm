using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    public static class CollectionExt
    {
        //static fields
        public static readonly IEnumerable<bool> Booleans = new[] {false, true};

        //static methods
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;

            foreach (var item in source)
                action(item, i++);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            var list = source as IList<T>;
            if (list != null)
                return list.IndexOf(item);
            return source.IndexOf(item, null);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            var i = 0;
            foreach (var x in source)
            {
                if (comparer.Equals(x, item))
                    return i;
                i++;
            }
            return -1;
        }

        public static long LongIndexOf<T>(this IEnumerable<T> source, T item)
        {
            return source.LongIndexOf(item, null);
        }

        public static long LongIndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            var i = 0L;
            foreach (var x in source)
            {
                if (comparer.Equals(x, item))
                    return i;
                i++;
            }
            return -1L;
        }

        public static T ElementAt<T>(this IEnumerable<T> source, long index)
        {
            if (index < 0L)
                throw new ArgumentOutOfRangeException("index", "index cannot be less than zero");
            foreach (var item in source)
            {
                if (index == 0L)
                    return item;
                index--;
            }
            throw new ArgumentOutOfRangeException("index", "index is more than source's length");
        }

        public static bool In<T>(this T item, params T[] source)
        {
            return source.Contains(item);
        }

        public static bool In<T>(this T item, IEnumerable<T> source)
        {
            return source.Contains(item);
        }

        public static bool In<T>(this T item, IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return source.Contains(item, comparer);
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int num)
        {
            for (var x = 0; x < num; x++)
            {
                foreach (var item in source)
                    yield return item;
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static bool All<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            var i = 0;

            foreach (var item in source)
            {
                if (!predicate(item, i++))
                    return false;
            }
            return true;
        }

        public static bool Any<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            var i = 0;

            foreach (var item in source)
            {
                if (predicate(item, i++))
                    return true;
            }
            return false;
        }

        public static IEnumerable<Tuple<int, T>> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => new Tuple<int, T>(index, item));
        }

        public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> source2)
        {
            return Zip(source, source2, (a, b) => new Tuple<T1, T2>(a, b));
        }

        public static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> source, IEnumerable<T2> source2, Func<T1, T2, TResult> selector)
        {
            using (var item = source.GetEnumerator())
            using (var item2 = source2.GetEnumerator())
            {
                while (item.MoveNext() && item2.MoveNext())
                    yield return selector(item.Current, item2.Current);
            }
        }

        public static IEnumerable<Tuple<T1, T2, T3>> Zip<T1, T2, T3>(this IEnumerable<T1> source, IEnumerable<T2> source2, IEnumerable<T3> source3)
        {
            return Zip(source, source2, source3, (a, b, c) => new Tuple<T1, T2, T3>(a, b, c));
        }

        public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(this IEnumerable<T1> source, IEnumerable<T2> source2, IEnumerable<T3> source3, Func<T1, T2, T3, TResult> selector)
        {
            using (var item = source.GetEnumerator())
            using (var item2 = source2.GetEnumerator())
            using (var item3 = source3.GetEnumerator())
            {
                while (item.MoveNext() && item2.MoveNext() && item3.MoveNext())
                    yield return selector(item.Current, item2.Current, item3.Current);
            }
        }

        public static IEnumerable<Tuple<T1, T2, T3, T4>> Zip<T1, T2, T3, T4>(this IEnumerable<T1> source, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4)
        {
            return Zip(source, source2, source3, source4, (a, b, c, d) => new Tuple<T1, T2, T3, T4>(a, b, c, d));
        }

        public static IEnumerable<TResult> Zip<T1, T2, T3, T4, TResult>(this IEnumerable<T1> source, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4, Func<T1, T2, T3, T4, TResult> selector)
        {
            using (var item = source.GetEnumerator())
            using (var item2 = source2.GetEnumerator())
            using (var item3 = source3.GetEnumerator())
            using (var item4 = source4.GetEnumerator())
            {
                while (item.MoveNext() && item2.MoveNext() && item3.MoveNext() && item4.MoveNext())
                    yield return selector(item.Current, item2.Current, item3.Current, item4.Current);
            }
        }

        public static T1 MaxBy<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> selector)
        {
            using (var item = source.GetEnumerator())
            {
                if (!item.MoveNext())
                    throw new InvalidOperationException("source contains no element");
                IComparer<T2> comparer = Comparer<T2>.Default;
                var candidate = item.Current;
                var score = selector(candidate);
                var winner = candidate;
                var maxScore = score;
                while (item.MoveNext())
                {
                    candidate = item.Current;
                    score = selector(candidate);
                    if (comparer.Compare(score, maxScore) > 0)
                    {
                        winner = candidate;
                        maxScore = score;
                    }
                }
                return winner;
            }
        }

        public static T1 MinBy<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> selector)
        {
            using (var item = source.GetEnumerator())
            {
                if (!item.MoveNext())
                    throw new InvalidOperationException("source contains no element");
                IComparer<T2> comparer = Comparer<T2>.Default;
                var candidate = item.Current;
                var score = selector(candidate);
                var loser = candidate;
                var minScore = score;
                while (item.MoveNext())
                {
                    candidate = item.Current;
                    score = selector(candidate);
                    if (comparer.Compare(score, minScore) < 0)
                    {
                        loser = candidate;
                        minScore = score;
                    }
                }
                return loser;
            }
        }

        public static IEnumerable<T> Excludes<T>(this IEnumerable<T> source, T item)
        {
            return source.Excludes(item, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Excludes<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            return source.Where(s => !comparer.Equals(s, item));
        }

        public static IEnumerable<Tuple<T, T>> PairWise<T>(this IEnumerable<T> source)
        {
            using (var item = source.GetEnumerator())
            {
                if (!item.MoveNext())
                    yield break;

                for (var previous = item.Current; item.MoveNext(); previous = item.Current)
                {
                    yield return Tuple.Create(previous, item.Current);
                }
            }
        } 
    }
}
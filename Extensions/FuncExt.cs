using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    public static class FuncExt
    {
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
        {
            var dict = new Dictionary<T, TResult>();
            return arg =>
            {
                TResult value;
                if (!dict.TryGetValue(arg, out value))
                {
                    value = func(arg);
                    dict.Add(arg, value);
                }
                return value;
            };
        }

        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            var curried = Memoize<T1, Func<T2, TResult>>(arg1 =>
                Memoize<T2, TResult>(arg2 => func(arg1, arg2)));
            return (arg1, arg2) => curried(arg1)(arg2);
        }

        public static Func<T1, T2, T3, TResult> Memoize<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            var curried = Memoize<T1, Func<T2, Func<T3, TResult>>>(arg1 =>
                Memoize<T2, Func<T3, TResult>>(arg2 =>
                    Memoize<T3, TResult>(arg3 => func(arg1, arg2, arg3))));
            return (arg1, arg2, arg3) => curried(arg1)(arg2)(arg3);
        }

        public static Func<T1, T2, T3, T4, TResult> Memoize<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            var curried = Memoize<T1, Func<T2, Func<T3, Func<T4, TResult>>>>(arg1 =>
                Memoize<T2, Func<T3, Func<T4, TResult>>>(arg2 =>
                    Memoize<T3, Func<T4, TResult>>(arg3 =>
                        Memoize<T4, TResult>(arg4 => func(arg1, arg2, arg3, arg4)))));
            return (arg1, arg2, arg3, arg4) => curried(arg1)(arg2)(arg3)(arg4);
        }

        public static Func<T1, TResult> Memoize<T1, TResult, TKey>(this Func<T1, TResult> func, Func<T1, TKey> keySelector)
        {
            var dict = new Dictionary<TKey, TResult>();
            return arg1 =>
            {
                TResult value;
                var key = keySelector(arg1);
                if (!dict.TryGetValue(key, out value))
                {
                    value = func(arg1);
                    dict.Add(key, value);
                }
                return value;
            };
        }

        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult, TKey>(this Func<T1, T2, TResult> func, Func<T1, T2, TKey> keySelector)
        {
            var dict = new Dictionary<TKey, TResult>();
            return (arg1, arg2) =>
            {
                TResult value;
                var key = keySelector(arg1, arg2);
                if (!dict.TryGetValue(key, out value))
                {
                    value = func(arg1, arg2);
                    dict.Add(key, value);
                }
                return value;
            };
        }

        public static Func<T1, T2, T3, TResult> Memoize<T1, T2, T3, TResult, TKey>(this Func<T1, T2, T3, TResult> func, Func<T1, T2, T3, TKey> keySelector)
        {
            var dict = new Dictionary<TKey, TResult>();
            return (arg1, arg2, arg3) =>
            {
                TResult value;
                var key = keySelector(arg1, arg2, arg3);
                if (!dict.TryGetValue(key, out value))
                {
                    value = func(arg1, arg2, arg3);
                    dict.Add(key, value);
                }
                return value;
            };
        }

        public static Func<T1, T2, T3, T4, TResult> Memoize<T1, T2, T3, T4, TResult, TKey>(this Func<T1, T2, T3, T4, TResult> func, Func<T1, T2, T3, T4, TKey> keySelector)
        {
            var dict = new Dictionary<TKey, TResult>();
            return (arg1, arg2, arg3, arg4) =>
            {
                TResult value;
                var key = keySelector(arg1, arg2, arg3, arg4);
                if (!dict.TryGetValue(key, out value))
                {
                    value = func(arg1, arg2, arg3, arg4);
                    dict.Add(key, value);
                }
                return value;
            };
        }

        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            return arg1 => arg2 => func(arg1, arg2);
        }

        public static Func<T1, Func<T2, T3, TResult>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            return arg1 => (arg2, arg3) => func(arg1, arg2, arg3);
        }

        public static Func<T1, Func<T2, T3, T4, TResult>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            return arg1 => (arg2, arg3, arg4) => func(arg1, arg2, arg3, arg4);
        }

        public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> func)
        {
            return (arg1, arg2) => func(arg1)(arg2);
        }

        public static Func<T1, T2, T3, TResult> Uncurry<T1, T2, T3, TResult>(this Func<T1, Func<T2, T3, TResult>> func)
        {
            return (arg1, arg2, arg3) => func(arg1)(arg2, arg3);
        }

        public static Func<T1, T2, T3, T4, TResult> Uncurry<T1, T2, T3, T4, TResult>(this Func<T1, Func<T2, T3, T4, TResult>> func)
        {
            return (arg1, arg2, arg3, arg4) => func(arg1)(arg2, arg3, arg4);
        }

        public static Func<T2, TResult> Partial<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1)
        {
            return arg2 => func(arg1, arg2);
        }

        public static Func<T2, T3, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1)
        {
            return (arg2, arg3) => func(arg1, arg2, arg3);
        }

        public static Func<T2, T3, T4, TResult> Partial<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1)
        {
            return (arg2, arg3, arg4) => func(arg1, arg2, arg3, arg4);
        }

        public static Func<T, bool> And<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> Or<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) || predicate2(arg);
        }

        public static Func<T, TResult> When<T, TResult>(this Func<T, TResult> func, Func<T, bool> predicate, Func<T, TResult> alternative)
        {
            return arg => predicate(arg) ? alternative(arg) : func(arg);
        }

        public static Func<T1, T2, TResult> When<T1, T2, TResult>(this Func<T1, T2, TResult> func, Func<T1, T2, bool> predicate, Func<T1, T2, TResult> alternative)
        {
            return (arg1, arg2) => predicate(arg1, arg2) ? alternative(arg1, arg2) : func(arg1, arg2);
        }

        public static Func<T1, T2, T3, TResult> When<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TResult> alternative)
        {
            return (arg1, arg2, arg3) => predicate(arg1, arg2, arg3) ? alternative(arg1, arg2, arg3) : func(arg1, arg2, arg3);
        }

        public static Func<T1, T2, T3, T4, TResult> When<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, Func<T1, T2, T3, T4, bool> predicate, Func<T1, T2, T3, T4, TResult> alternative)
        {
            return (arg1, arg2, arg3, arg4) => predicate(arg1, arg2, arg3, arg4) ? alternative(arg1, arg2, arg3, arg4) : func(arg1, arg2, arg3, arg4);
        }

        public static Func<T, bool> PredicateAll<T>(this IEnumerable<Func<T, bool>> source)
        {
            return item => source.All(predicate => predicate(item));
        }

        public static Func<T, bool> PredicateAny<T>(this IEnumerable<Func<T, bool>> source)
        {
            return item => source.Any(predicate => predicate(item));
        }

        public static Func<TResult> Create<TResult>(Func<TResult> tResult)
        {
            return tResult;
        }

        public static Func<T, TResult> Create<T, TResult>(T t, Func<T, TResult> tResult)
        {
            return tResult;
        }

        public static Func<T1, T2, TResult> Create<T1, T2, TResult>(T1 t1, T2 t2, Func<T1, T2, TResult> tResult)
        {
            return tResult;
        }

        public static Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(T1 t1, T2 t2, T3 t3, Func<T1, T2, T3, TResult> tResult)
        {
            return tResult;
        }

        public static Func<T1, T2, T3, T4, TResult> Create<T1, T2, T3, T4, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, Func<T1, T2, T3, T4, TResult> tResult)
        {
            return tResult;
        }

        public static bool False<T>(T arg)
        {
            return false;
        }

        public static bool True<T>(T arg)
        {
            return true;
        }

        public static T Chain<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}
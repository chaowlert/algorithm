using System;
using System.Collections.Generic;

namespace Chaow.Extensions
{
    public delegate bool TryFunc<in T, TResult>(T arg, out TResult result);
    public delegate bool TryFunc<in T1, in T2, TResult>(T1 arg1, T2 arg2, out TResult result);

    public interface IMaybe<out T>
    {
        bool HasValue { get; }
        T Value { get; }
        T GetValueOrDefault();
        T GetValueOrThrow<TException>() where TException : Exception, new();
        T GetValueOrThrow<TException>(Func<TException> func) where TException : Exception;
    }

    struct Maybe<T> : IMaybe<T>
    {
        public static readonly Maybe<T> Nothing = new Maybe<T>();

        public Maybe(T obj)
        {
            value = obj;
            hasValue = obj != null;
        }

        readonly bool hasValue;
        public bool HasValue
        {
            get { return hasValue; }
        }

        readonly T value;
        public T Value
        {
            get { return value; }
        }

        public T GetValueOrDefault()
        {
            return value;
        }

        public T GetValueOrThrow<TException>() where TException : Exception, new()
        {
            if (!hasValue)
                throw new TException();
            return value;
        }

        public T GetValueOrThrow<TException>(Func<TException> func) where TException : Exception
        {
            if (!hasValue)
                throw func();
            return value;
        }
    }

    public static class MaybeExtensions
    {
        public static IMaybe<T> AsMaybe<T>(this T? value) where T : struct
        {
            if (value.HasValue)
                return new Maybe<T>(value.Value);
            else
                return Maybe<T>.Nothing;
        } 

        public static IMaybe<T> AsMaybe<T>(this T value)
        {
            return new Maybe<T>(value);
        }

        public static IMaybe<U> SelectMany<T, U>(this IMaybe<T> m, Func<T, IMaybe<U>> k) where U : struct
        {
            if (!m.HasValue)
                return Maybe<U>.Nothing;
            return k(m.Value);
        }

        public static IMaybe<V> SelectMany<T, U, V>(this IMaybe<T> m, Func<T, IMaybe<U>> k, Func<T, U, V?> r) where V : struct 
        {
            if (!m.HasValue)
                return Maybe<V>.Nothing;
            var n = k(m.Value);
            if (!n.HasValue)
                return Maybe<V>.Nothing;
            return r(m.Value, n.Value).AsMaybe();
        }

        public static IMaybe<V> SelectMany<T, U, V>(this IMaybe<T> m, Func<T, IMaybe<U>> k, Func<T, U, V> r)
        {
            if (!m.HasValue)
                return Maybe<V>.Nothing;
            var n = k(m.Value);
            if (!n.HasValue)
                return Maybe<V>.Nothing;
            return r(m.Value, n.Value).AsMaybe();
        }

        public static IMaybe<T> Where<T>(this IMaybe<T> m, Func<T, bool> p)
        {
            if (!m.HasValue)
                return Maybe<T>.Nothing;
            return p(m.Value) ? m : Maybe<T>.Nothing;
        }

        public static IMaybe<U> Select<T, U>(this IMaybe<T> m, Func<T, U?> k) where U : struct 
        {
            if (!m.HasValue)
                return Maybe<U>.Nothing;
            return k(m.Value).AsMaybe();
        }

        public static IMaybe<U> Select<T, U>(this IMaybe<T> m, Func<T, U> k)
        {
            if (!m.HasValue)
                return Maybe<U>.Nothing;
            return k(m.Value).AsMaybe();
        }

        public static U TryWith<T, U>(this T arg, TryFunc<T, U> t, U def)
        {
            U r;
            return t(arg, out r) ? r : def;
        }

        public static U TryWith<T1, T2, U>(this T1 arg1, T2 arg2, TryFunc<T1, T2, U> t, U def)
        {
            U r;
            return t(arg1, arg2, out r) ? r : def;
        }

        public static T? AsNullable<T>(this IMaybe<T> m) where T : struct
        {
            return m.HasValue ? (T?)m.Value : null;
        }

        public static IEnumerable<U> Unfold<T, U>(this T item, Func<T, IMaybe<Tuple<U, T>>> g)
        {
            for (var maybe = g(item); maybe.HasValue; maybe = g(maybe.Value.Item2))
            {
                yield return maybe.Value.Item1;
            }
        } 
    }
}
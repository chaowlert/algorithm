using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chaow.Extensions
{
    public static class DictionaryExt
    {
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;
            else
                return default (TValue);
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            TValue result;
            if (dict.TryGetValue(key, out result))
                return result;

            lock (dict)
            {
                if (dict.TryGetValue(key, out result))
                    return result;

                dict.Add(key, value);
                return value;
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> func)
        {
            TValue result;
            if (dict.TryGetValue(key, out result))
                return result;

            lock (dict)
            {
                if (dict.TryGetValue(key, out result))
                    return result;

                result = func(key);
                dict.Add(key, result);
                return result;
            }
        }
    }
}

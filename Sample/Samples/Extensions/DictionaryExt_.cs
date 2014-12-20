using System;
using System.Collections.Generic;

namespace Chaow.Extensions
{
    //DictionaryExt is helper for dictionary
    public static class DictionaryExt_
    {
        public static void AsReadOnly()
        {
            //dictionary.AsReadOnly turns dictionary to readonly dictionary

            //create dict
            var dict = new Dictionary<string, int>
            {
                {"a", 1},
            };

            var readonlyDict = dict.AsReadOnly();

            //error is expected
            Console.WriteLine("Change in readonlyDict, error is expected");
            ((IDictionary<string, int>)readonlyDict).Add("b", 2);
        }

        public static void GetValueOrDefault()
        {
            //dictionary.GetValueOrDefault(key) is shorthand for getting dict value

            //create dict
            var dict = new Dictionary<string, int>
            {
                {"a", 1},
            };

            Console.WriteLine("Value for a is {0}", dict.GetValueOrDefault("a"));
            Console.WriteLine("Value for b is {0}", dict.GetValueOrDefault("b"));
        }

        public static void GetOrAdd()
        {
            //dictionary.GetOrAdd is thread-safe check key exists before add

            //create dict
            var dict = new Dictionary<string, int>();

            //can add by dictionary.GetOrAdd(key, value)
            var item1 = dict.GetOrAdd("a", 0);

            //or add by dictionary.GetOrAdd(key, key => value)
            var item2 = dict.GetOrAdd("a", key => 1);

            Console.WriteLine("Value for a is {0}", dict.GetValueOrDefault("a"));
        }
    }
}

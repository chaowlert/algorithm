using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Numeric.Sequence
{
    public static class SequenceExt
    {
        public static BaseSequence<T> Create<T>(Func<T, T> selector)
        {
            return new GenericSequence<T>(selector);
        }

        public static BaseSequence<T> Create<T>(T seed, Func<T, T> selector)
        {
            return new GenericSequence<T>(seed, selector);
        }

        public static BaseSequence<T> ToSequence<T>(this IEnumerable<T> source)
        {
            return new IdentitySequence<T>(source);
        }

        public static BaseSequence<T> Cycle<T>(this IEnumerable<T> source)
        {
            return new IdentitySequence<T>(cycle(source));
        }

        static IEnumerable<T> cycle<T>(IEnumerable<T> source)
        {
            if (!source.Any())
                yield break;
            while (true)
            {
                foreach (var item in source)
                    yield return item;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Chaow.Numeric.Sequence
{
    public sealed class GenericSequence<T> : BaseSequence<T>
    {
        //fields
        readonly T _seed;
        readonly Func<T, T> _selector;

        //constructors
        public GenericSequence(Func<T, T> selector)
        {
            _seed = default(T);
            _selector = selector;
        }

        public GenericSequence(T seed, Func<T, T> selector)
        {
            _seed = seed;
            _selector = selector;
        }

        //protect methods
        protected override IEnumerable<T> enumerate()
        {
            var value = _seed;

            yield return value;
            while (true)
            {
                value = _selector(value);
                yield return value;
            }
        }
    }
}
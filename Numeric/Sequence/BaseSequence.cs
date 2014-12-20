using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Numeric.Sequence
{
    public abstract class BaseSequence<T>
    {
        //protect methods
        protected abstract IEnumerable<T> enumerate();

        //public methods
        public T ElementAt(int index)
        {
            return enumerate().ElementAt(index);
        }

        public T First()
        {
            return enumerate().First();
        }

        public T First(Func<T, bool> predicate)
        {
            return enumerate().First(predicate);
        }

        public IEnumerable<T> Take(int count)
        {
            return enumerate().Take(count);
        }

        public IEnumerable<T> TakeWhile(Func<T, bool> predicate)
        {
            return enumerate().TakeWhile(predicate);
        }

        public IEnumerable<T> TakeWhile(Func<T, int, bool> predicate)
        {
            return enumerate().TakeWhile(predicate);
        }
    }
}
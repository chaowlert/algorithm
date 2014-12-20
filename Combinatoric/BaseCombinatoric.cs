using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    public abstract class BaseCombinatoric<T> : IList<T>
    {
        //properties

        public virtual T this[long index]
        {
            get { return this.ElementAt(index); }
        }

        public virtual long LongCount
        {
            get { return this.LongCount(); }
        }

        public virtual T this[int index]
        {
            get { return this[(long)index]; }
            set { throw new NotSupportedException(); }
        }

        public virtual int Count
        {
            get
            {
                var result = LongCount;
                return checked((int)result);
            }
        }

        //public methods
        public virtual bool Contains(T item)
        {
            return this.Contains(item, null);
        }

        public virtual int IndexOf(T item)
        {
            var result = LongIndexOf(item);
            if (result > int.MaxValue)
                throw new OverflowException("returned value is more than int.MaxValue");
            return (int)result;
        }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
                array[i++] = item;
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IList<T> Members

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        public virtual long LongIndexOf(T item)
        {
            return this.LongIndexOf(item, null);
        }
    }
}
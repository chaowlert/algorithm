using System.Collections.Generic;

namespace Chaow.Numeric.Sequence
{
    public sealed class IdentitySequence<T> : BaseSequence<T>
    {
        //fields
        readonly IEnumerable<T> _source;

        //properties

        //constructors
        public IdentitySequence(IEnumerable<T> source)
        {
            _source = source;
        }

        public IEnumerable<T> Source
        {
            get { return _source; }
        }

        //protect methods
        protected override IEnumerable<T> enumerate()
        {
            return _source;
        }
    }
}
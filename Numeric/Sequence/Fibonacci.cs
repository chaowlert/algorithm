using System.Collections.Generic;
using System.Numerics;

namespace Chaow.Numeric.Sequence
{
    public sealed class Fibonacci : BaseSequence<BigInteger>
    {
        //protect methods
        protected override IEnumerable<BigInteger> enumerate()
        {
            var value = BigInteger.Zero;
            var value2 = BigInteger.One;

            yield return value;
            yield return value2;
            while (true)
            {
                value += value2;
                yield return value;
                value2 += value;
                yield return value2;
            }
        }
    }
}
using System.Linq;

namespace Chaow.Numeric.Sequence
{
    public static class PrimeExt
    {
        public static long EulerPhi(this Prime prime, long num)
        {
            return prime.Factors(num).Distinct().Aggregate(num, (n, m) => (n / m) * (m - 1));
        }

        public static int MoebiusMu(this Prime prime, long num)
        {
            var lookup = prime.Factors(num).ToLookup(x => x);
            if (lookup.Any(x => x.Count() > 1))
                return 0;
            return ((lookup.Count & 1L) == 1L) ? -1 : 1;
        }

        ////number of solutions 1/x + 1/y = 1/n
        public static long A018892(this Prime prime, long num)
        {
            return (prime.Factors(num).ToLookup(x => x).Aggregate(1L, (n, m) => n * (2L * m.LongCount() + 1L)) + 1L) / 2L;
        }
    }
}
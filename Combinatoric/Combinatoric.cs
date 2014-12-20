using System;
using System.Collections.Generic;
using System.Linq;
using Chaow.Extensions;
using Chaow.Numeric;
using Chaow.Numeric.Sequence;

namespace Chaow.Combinatorics
{
    [Flags]
    public enum CombinatoricModel
    {
        Normal = 0,
        Repetition = 1,
        Distinct = 2,
        RepetitionDistinct = 3,
    }

    public static class Combinatoric
    {
        public static Permutation<T> Permute<T>(this IEnumerable<T> source)
        {
            return new Permutation<T>(source);
        }

        public static Permutation<T> Permute<T>(this IEnumerable<T> source, int length)
        {
            return new Permutation<T>(source, length);
        }

        public static Permutation<T> Permute<T>(this IEnumerable<T> source, int length, CombinatoricModel model)
        {
            return new Permutation<T>(source, length, model);
        }

        public static Combination<T> Combine<T>(this IEnumerable<T> source, int length)
        {
            return new Combination<T>(source, length);
        }

        public static Combination<T> Combine<T>(this IEnumerable<T> source, int length, CombinatoricModel model)
        {
            return new Combination<T>(source, length, model);
        }

        public static BinaryBracketing<T> BinaryBracketing<T>(this IEnumerable<T> source, Func<T, T, T> operation)
        {
            return new BinaryBracketing<T>(source, operation);
        }

        public static BinaryBracketing<T> BinaryBracketing<T>(this IEnumerable<T> source, IEnumerable<Func<T, T, T>> operations)
        {
            return new BinaryBracketing<T>(source, operations);
        }

        public static Partition Partition(this IEnumerable<int> source, int sum)
        {
            return new Partition(source, sum);
        }

        public static Divisor Divisors(this Prime prime, long num)
        {
            return new Divisor(num, prime);
        }

        public static long Sigma(this Divisor divisor, int k)
        {
            if (k == 0)
                return divisor.LongCount();
            if (k == 1)
                return divisor.Sum();
            return divisor.Sum(x => x.Power(k));
        }

        public static Backtracker<T, T[]> Backtrack<T>(this IEnumerable<T> source, int length)
        {
            return new Backtracker<T, T[]>(source, new T[0],
                (current, item, i) => current.Append(item),
                current => current.Length == length);
        }

        public static Backtracker<T1, T2> Backtrack<T1, T2>(this IEnumerable<T1> source, T2 seed, Func<T2, T1, int, T2> func, Func<T2, bool> yield)
        {
            return new Backtracker<T1, T2>(source, seed, func, yield);
        }

        public static T RandomSingle<T>(this BaseCombinatoric<T> source)
        {
            var count = source.LongCount;
            if (count == 0L)
                throw new InvalidOperationException("source contains no element");
            return source.ElementAt(MathExt.Random.NextLong(count));
        }

        public static T RandomSingleOrDefault<T>(this BaseCombinatoric<T> source)
        {
            var count = source.LongCount;
            if (count == 0L)
                return default(T);
            return source.ElementAt(MathExt.Random.NextLong(count));
        }

        public static T RandomSingle<T>(this IEnumerable<T> source)
        {
            var count = source.Count();
            if (count == 0)
                throw new InvalidOperationException("source contains no element");
            return source.ElementAt(MathExt.Random.Next(count));
        }

        public static T RandomSingleOrDefault<T>(this IEnumerable<T> source)
        {
            var count = source.Count();
            if (count == 0)
                return default(T);
            return source.ElementAt(MathExt.Random.Next(count));
        }

        public static IEnumerable<T> RandomSamples<T>(this IEnumerable<T> source, int length)
        {
            return randomSample(source, length);
        }

        public static IEnumerable<T> RandomSamples<T>(this IEnumerable<T> source, int length, bool repetition)
        {
            if (repetition)
                return randomSampleWithRepetition(source, length);
            return randomSample(source, length);
        }

        static IEnumerable<T> randomSample<T>(IEnumerable<T> source, int length)
        {
            var count = source.Count();
            if (count < length)
                throw new InvalidOperationException("source cannot be less than length");
            var set = new HashSet<int>();
            foreach (var i in new RandomSequence(count).Take(length))
            {
                var j = i;
                while (!set.Add(j))
                {
                    j++;
                    if (j == count)
                        j = 0;
                }
                yield return source.ElementAt(j);
            }
        }

        static IEnumerable<T> randomSampleWithRepetition<T>(IEnumerable<T> source, int length)
        {
            var count = source.Count();
            if (count == 0)
                throw new InvalidOperationException("source contains no element");
            foreach (var i in new RandomSequence(count).Take(length))
                yield return source.ElementAt(i);
        }

        public static int Factorial(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");

            var result = 1;
            for (var i = 2; i <= n; i++)
                result = checked(result * i);
            return result;
        }

        public static long Factorial(long n)
        {
            if (n < 0L)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");

            var result = 1L;
            for (var i = 2L; i <= n; i++)
                result = checked(result * i);
            return result;
        }

        public static int FallingFactorial(int n, int r)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");
            if (r < 0)
                throw new ArgumentOutOfRangeException("r", "r cannot be negative");
            if (n < r)
                return 0;
            if (r == 0)
                return 1;

            var result = 1;
            for (var i = n - r + 1; i <= n; i++)
                result = checked(result * i);
            return result;
        }

        public static long FallingFactorial(long n, long r)
        {
            if (n < 0L)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");
            if (r < 0L)
                throw new ArgumentOutOfRangeException("r", "r cannot be negative");
            if (n < r)
                return 0L;
            if (r == 0)
                return 1L;

            var result = 1L;
            for (var i = n - r + 1L; i <= n; i++)
                result = checked(result * i);
            return result;
        }

        public static int Choose(int n, int k)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");
            if (k < 0)
                throw new ArgumentOutOfRangeException("k", "k cannot be negative");
            if (n < k)
                return 0;

            var r = n - k;
            var result = 1;
            for (int i = n - Math.Min(k, r) + 1, j = 1; i <= n; i++, j++)
                result = checked(result * i) / j;
            return result;
        }

        public static long Choose(long n, long k)
        {
            if (n < 0L)
                throw new ArgumentOutOfRangeException("n", "n cannot be negative");
            if (k < 0L)
                throw new ArgumentOutOfRangeException("k", "k cannot be negative");
            if (n < k)
                return 0L;

            var r = n - k;
            var result = 1L;
            for (long i = n - Math.Min(k, r) + 1L, j = 1L; i <= n; i++, j++)
                result = checked(result * i) / j;
            return result;
        }

        public static int Multinomial(IEnumerable<int> source)
        {
            int result = 1, n = 0;

            foreach (var k in source)
            {
                n = checked(n + k);
                result = checked(result * Choose(n, k));
            }
            return result;
        }

        public static long Multinomial(IEnumerable<long> source)
        {
            long result = 1L, n = 0L;

            foreach (var k in source)
            {
                n = checked(n + k);
                result = checked(result * Choose(n, k));
            }
            return result;
        }
    }
}
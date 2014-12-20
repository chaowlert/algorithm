using System;
using System.Collections.Generic;

namespace Chaow.Extensions
{
    public static class IntExt
    {
        public static IEnumerable<int> To(this int from, int to)
        {
            for (; from < to; from++)
                yield return from;
            if (from == to)
                yield return from;
        }

        public static IEnumerable<long> To(this long from, long to)
        {
            for (; from < to; from++)
                yield return from;
            if (from == to)
                yield return from;
        }

        public static IEnumerable<int> DownTo(this int from, int to)
        {
            for (; from > to; from--)
                yield return from;
            if (from == to)
                yield return from;
        }

        public static IEnumerable<long> DownTo(this long from, long to)
        {
            for (; from > to; from--)
                yield return from;
            if (from == to)
                yield return from;
        }

        public static IEnumerable<int> StepTo(this int from, int to, int step)
        {
            if (step > 0)
            {
                var limit = Math.Min(to, int.MaxValue - step + 1);
                for (; from < limit; from += step)
                    yield return from;
                if (from <= to)
                    yield return from;
            }
            else if (step < 0)
            {
                var limit = Math.Max(to, int.MinValue - step - 1);
                for (; from > limit; from += step)
                    yield return from;
                if (from >= to)
                    yield return from;
            }
            else
                throw new ArgumentException("step cannot be zero", "step");
        }

        public static IEnumerable<long> StepTo(this long from, long to, long step)
        {
            if (step > 0L)
            {
                var limit = Math.Min(to, long.MaxValue - step + 1L);
                for (; from < limit; from += step)
                    yield return from;
                if (from <= to)
                    yield return from;
            }
            else if (step < 0)
            {
                var limit = Math.Max(to, long.MinValue - step - 1L);
                for (; from > limit; from += step)
                    yield return from;
                if (from >= to)
                    yield return from;
            }
            else
                throw new ArgumentException("step cannot be zero", "step");
        }

        public static void Times(this int num, Action<int> action)
        {
            for (var x = 0; x < num; x++)
                action(x);
        }

        public static int GetHighInt(this long i)
        {
            return (int)(i >> 32);
        }

        public static int GetLowInt(this long i)
        {
            return (int)i;
        }

        public static long CreateLong(int high, int low)
        {
            return ((long)high << 32) + low;
        }
    }
}
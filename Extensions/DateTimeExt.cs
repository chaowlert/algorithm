using System;
using System.Collections.Generic;

namespace Chaow.Extensions
{
    public struct MonthUnit : IComparable, IComparable<MonthUnit>, IEquatable<MonthUnit>
    {
        //fields
        readonly int _months;

        public MonthUnit(int months)
        {
            if (months < -120000 || months > 120000)
                throw new ArgumentOutOfRangeException("months", "month range cannot be greater than 12000");
            _months = months;
        }

        //properties
        public int Months
        {
            get { return _months; }
        }

        public int Years
        {
            get { return _months / 12; }
        }

        //constructors

        //public methods
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is MonthUnit))
                throw new ArgumentException("obj type must be MonthUnit", "obj");
            return CompareTo((MonthUnit)obj);
        }

        public int CompareTo(MonthUnit other)
        {
            if (_months > other._months)
                return 1;
            if (_months < other._months)
                return -1;
            return 0;
        }

        public bool Equals(MonthUnit other)
        {
            return _months == other._months;
        }

        public MonthUnit Duration()
        {
            return new MonthUnit(Math.Abs(_months));
        }

        public override bool Equals(object obj)
        {
            return (obj is MonthUnit) && (((MonthUnit)obj)._months == _months);
        }

        public override int GetHashCode()
        {
            return _months;
        }

        public override string ToString()
        {
            return string.Format("{0} months", _months);
        }

        //operators
        public static bool operator ==(MonthUnit m1, MonthUnit m2)
        {
            return m1._months == m2._months;
        }

        public static bool operator !=(MonthUnit m1, MonthUnit m2)
        {
            return m1._months != m2._months;
        }

        public static bool operator >(MonthUnit m1, MonthUnit m2)
        {
            return m1._months > m2._months;
        }

        public static bool operator >=(MonthUnit m1, MonthUnit m2)
        {
            return m1._months >= m2._months;
        }

        public static bool operator <(MonthUnit m1, MonthUnit m2)
        {
            return m1._months < m2._months;
        }

        public static bool operator <=(MonthUnit m1, MonthUnit m2)
        {
            return m1._months <= m2._months;
        }

        public static MonthUnit operator +(MonthUnit m1, MonthUnit m2)
        {
            return new MonthUnit(m1._months + m2._months);
        }

        public static DateTime operator +(DateTime dateTime, MonthUnit months)
        {
            return dateTime.AddMonths(months._months);
        }

        public static MonthUnit operator -(MonthUnit m1, MonthUnit m2)
        {
            return new MonthUnit(m1._months - m2._months);
        }

        public static DateTime operator -(DateTime dateTime, MonthUnit months)
        {
            return dateTime.AddMonths(-months._months);
        }

        public static MonthUnit operator -(MonthUnit months)
        {
            return new MonthUnit(-months._months);
        }
    }

    public struct DayUnit : IComparable, IComparable<DayUnit>, IEquatable<DayUnit>
    {
        //fields
        readonly int _days;

        public DayUnit(int days)
        {
            if (days < -3652060 || days >= 3652060)
                throw new ArgumentOutOfRangeException("days", "day range cannot be greater than 3652060");
            _days = days;
        }

        //properties
        public int Days
        {
            get { return _days; }
        }

        public int Weeks
        {
            get { return _days / 7; }
        }

        //constructors

        //public methods
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is DayUnit))
                throw new ArgumentException("obj type must be DayUnit", "obj");
            return CompareTo((DayUnit)obj);
        }

        public int CompareTo(DayUnit other)
        {
            if (_days > other._days)
                return 1;
            if (_days < other._days)
                return -1;
            return 0;
        }

        public bool Equals(DayUnit other)
        {
            return _days == other._days;
        }

        public DayUnit Duration()
        {
            return new DayUnit(Math.Abs(_days));
        }

        public override bool Equals(object obj)
        {
            return (obj is DayUnit) && (((DayUnit)obj)._days == _days);
        }

        public override int GetHashCode()
        {
            return _days;
        }

        public override string ToString()
        {
            return string.Format("{0} days", _days);
        }

        //operators
        public static bool operator ==(DayUnit d1, DayUnit d2)
        {
            return d1._days == d2._days;
        }

        public static bool operator !=(DayUnit d1, DayUnit d2)
        {
            return d1._days != d2._days;
        }

        public static bool operator >(DayUnit d1, DayUnit d2)
        {
            return d1._days > d2._days;
        }

        public static bool operator >=(DayUnit d1, DayUnit d2)
        {
            return d1._days >= d2._days;
        }

        public static bool operator <(DayUnit d1, DayUnit d2)
        {
            return d1._days < d2._days;
        }

        public static bool operator <=(DayUnit d1, DayUnit d2)
        {
            return d1._days <= d2._days;
        }

        public static DayUnit operator +(DayUnit d1, DayUnit d2)
        {
            return new DayUnit(d1._days + d2._days);
        }

        public static DateTime operator +(DateTime dateTime, DayUnit days)
        {
            return dateTime.AddDays(days._days);
        }

        public static DayUnit operator -(DayUnit d1, DayUnit d2)
        {
            return new DayUnit(d1._days - d2._days);
        }

        public static DateTime operator -(DateTime dateTime, DayUnit days)
        {
            return dateTime.AddDays(-days._days);
        }

        public static DayUnit operator -(DayUnit days)
        {
            return new DayUnit(-days._days);
        }
    }

    public static class DateTimeExt
    {
        public static MonthUnit Years(this int years)
        {
            if (years < -10000 || years > 10000)
                throw new ArgumentOutOfRangeException("years", "year range cannot be greater than 10000");
            return new MonthUnit(years * 12);
        }

        public static MonthUnit Months(this int months)
        {
            return new MonthUnit(months);
        }

        public static DayUnit Weeks(this int weeks)
        {
            if (weeks < -521722 || weeks > 521722)
                throw new ArgumentOutOfRangeException("weeks", "week range cannot be greater than 521722");
            return new DayUnit(weeks * 7);
        }

        public static DayUnit Days(this int days)
        {
            return new DayUnit(days);
        }

        public static TimeSpan Hours(this int hours)
        {
            return new TimeSpan(hours, 0, 0);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return new TimeSpan(0, minutes, 0);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return new TimeSpan(0, 0, seconds);
        }

        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return new TimeSpan(0, 0, 0, 0, milliseconds);
        }

        public static DateTime Ago(this MonthUnit months)
        {
            return DateTime.Today - months;
        }

        public static DateTime Ago(this DayUnit days)
        {
            return DateTime.Today - days;
        }

        public static DateTime Ago(this TimeSpan timeSpan)
        {
            return DateTime.Now - timeSpan;
        }

        public static DateTime Before(this MonthUnit months, DateTime dateTime)
        {
            return dateTime - months;
        }

        public static DateTime Before(this DayUnit days, DateTime dateTime)
        {
            return dateTime - days;
        }

        public static DateTime Before(this TimeSpan timeSpan, DateTime dateTime)
        {
            return dateTime - timeSpan;
        }

        public static DateTime FromNow(this MonthUnit months)
        {
            return DateTime.Today + months;
        }

        public static DateTime FromNow(this DayUnit days)
        {
            return DateTime.Today + days;
        }

        public static DateTime FromNow(this TimeSpan timeSpan)
        {
            return DateTime.Now + timeSpan;
        }

        public static DateTime From(this MonthUnit months, DateTime dateTime)
        {
            return dateTime + months;
        }

        public static DateTime From(this DayUnit days, DateTime dateTime)
        {
            return dateTime + days;
        }

        public static DateTime From(this TimeSpan timeSpan, DateTime dateTime)
        {
            return dateTime + timeSpan;
        }

        public static DateTime January(this int day, int year)
        {
            return new DateTime(year, 1, day);
        }

        public static DateTime February(this int day, int year)
        {
            return new DateTime(year, 2, day);
        }

        public static DateTime March(this int day, int year)
        {
            return new DateTime(year, 3, day);
        }

        public static DateTime April(this int day, int year)
        {
            return new DateTime(year, 4, day);
        }

        public static DateTime May(this int day, int year)
        {
            return new DateTime(year, 5, day);
        }

        public static DateTime June(this int day, int year)
        {
            return new DateTime(year, 6, day);
        }

        public static DateTime July(this int day, int year)
        {
            return new DateTime(year, 7, day);
        }

        public static DateTime August(this int day, int year)
        {
            return new DateTime(year, 8, day);
        }

        public static DateTime September(this int day, int year)
        {
            return new DateTime(year, 9, day);
        }

        public static DateTime October(this int day, int year)
        {
            return new DateTime(year, 10, day);
        }

        public static DateTime November(this int day, int year)
        {
            return new DateTime(year, 11, day);
        }

        public static DateTime December(this int day, int year)
        {
            return new DateTime(year, 12, day);
        }

        public static IEnumerable<DateTime> To(this DateTime from, DateTime to)
        {
            return from.StepTo(to, 1.Days());
        }

        public static IEnumerable<DateTime> DownTo(this DateTime from, DateTime to)
        {
            return from.StepTo(to, (-1).Days());
        }

        public static IEnumerable<DateTime> StepTo(this DateTime from, DateTime to, MonthUnit step)
        {
            if (step.Months > 0L)
            {
                var limit = (DateTime.MaxValue - step).AddTicks(1L);
                if (limit > to)
                    limit = to;
                for (; from < limit; from += step)
                    yield return from;
                if (from <= to)
                    yield return from;
            }
            else if (step.Months < 0L)
            {
                var limit = (DateTime.MinValue - step).AddTicks(-1L);
                if (limit < to)
                    limit = to;
                for (; from > limit; from += step)
                    yield return from;
                if (from >= to)
                    yield return from;
            }
            else
                throw new ArgumentException("step cannot be zero");
        }

        public static IEnumerable<DateTime> StepTo(this DateTime from, DateTime to, DayUnit step)
        {
            if (step.Days > 0L)
            {
                var limit = (DateTime.MaxValue - step).AddTicks(1L);
                if (limit > to)
                    limit = to;
                for (; from < limit; from += step)
                    yield return from;
                if (from <= to)
                    yield return from;
            }
            else if (step.Days < 0L)
            {
                var limit = (DateTime.MinValue - step).AddTicks(-1L);
                if (limit < to)
                    limit = to;
                for (; from > limit; from += step)
                    yield return from;
                if (from >= to)
                    yield return from;
            }
            else
                throw new ArgumentException("step cannot be zero");
        }

        public static IEnumerable<DateTime> StepTo(this DateTime from, DateTime to, TimeSpan step)
        {
            if (step.Ticks > 0L)
            {
                var limit = (DateTime.MaxValue - step).AddTicks(1L);
                if (limit > to)
                    limit = to;
                for (; from < limit; from += step)
                    yield return from;
                if (from <= to)
                    yield return from;
            }
            else if (step.Ticks < 0L)
            {
                var limit = (DateTime.MinValue - step).AddTicks(-1L);
                if (limit < to)
                    limit = to;
                for (; from > limit; from += step)
                    yield return from;
                if (from >= to)
                    yield return from;
            }
            else
                throw new ArgumentException("step cannot be zero");
        }
    }
}
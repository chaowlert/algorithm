using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaow.Extensions
{
    //DateTimeExt makes you use datetime easier
    public static class DateTimeExt_
    {
        public static void Declare_DateTime()
        {
            //this example shows how easy to declare a DateTime

            //you can easily declare DateTime by using method "day.monthName(year)"
            var d = 31.December(2008);

            //show result
            Console.WriteLine(d);
        }

        public static void Declare_TimeSpan()
        {
            //this example shows how easy to declare a TimeSpan

            //you can easily declare TimeSpan by using method "amount.timeUnit()"
            var ts1 = 12.Hours();
            var ts2 = 30.Minutes();
            var ts3 = 15.Seconds();
            var ts4 = 500.Milliseconds();

            //show results
            Console.WriteLine(ts1);
            Console.WriteLine(ts2);
            Console.WriteLine(ts3);
            Console.WriteLine(ts4);
        }

        public static void Declare_DayUnit()
        {
            //this example shows how to declare a DayUnit

            //you can declare DayUnit by using method "amount.dayUnit()"
            var d1 = 3.Days();
            var d2 = 2.Weeks();

            //show results
            Console.WriteLine(d1);
            Console.WriteLine(d2);
        }

        public static void Declare_MonthUnit()
        {
            //this example shows how to declare a MonthUnit

            //you can declare MonthUnit by using method "amount.monthUnit()"
            var m1 = 3.Months();
            var m2 = 1.Years();

            //show results
            Console.WriteLine(m1);
            Console.WriteLine(m2);
        }

        public static void DateTime_Reference()
        {
            //this example shows how to create DateTime by reference with another DateTime

            //you can create DateTime by reference with DateTime.Now or a specified DateTime
            var d1 = 3.Minutes().Ago();
            var d2 = 2.Days().Before(31.December(2008));
            var d3 = 3.Months().FromNow();
            var d4 = 2.Years().From(31.December(2008));

            //show results
            Console.WriteLine(d1);
            Console.WriteLine(d2);
            Console.WriteLine(d3);
            Console.WriteLine(d4);
        }

        public static void DateTime_Range()
        {
            //this example shows how to create DateTime range

            //you can create time range by DateTime.To(DateTime2)
            var dateRange = DateTime.Today.To(3.Days().FromNow());

            //show results
            dateRange.ForEach(d => Console.WriteLine(d));
        }

        public static void DateTime_Decreasing_Range()
        {
            //this example shows how to create decreasing DateTime range

            //you can create decreasing range by DateTime.DownTo(DateTime)
            var dateRange = DateTime.Today.DownTo(3.Days().Ago());

            //show results
            dateRange.ForEach(d => Console.WriteLine(d));
        }

        public static void DateTime_Stepping_Range()
        {
            //this example shows how to create stepping DateTime range

            //you can create stepping range by DateTime.StepTo(DateTime, TimeSpan)
            var dateRange = 1.January(2008).StepTo(31.January(2008), 1.Weeks());

            //show results
            dateRange.ForEach(d => Console.WriteLine(d));
        }

        public static void Sample_Find_Sunday()
        {
            //this example shows how to use datetime range to solve problem

            //How many Sundays fell on the first of the month during the twentieth century?
            //(Question from http://projecteuler.net)
            Console.WriteLine((from d in 1.January(1901).StepTo(31.December(2000), 1.Months())
                               where d.DayOfWeek == DayOfWeek.Sunday
                               select d).Count());
        }
    }
}
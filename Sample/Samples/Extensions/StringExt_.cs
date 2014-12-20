using System;
using System.Globalization;
using System.Linq;

namespace Chaow.Extensions
{
    //StringExt makes you create string in variety ways
    public static class StringExt_
    {
        public static void Join()
        {
            //this example shows how to join strings in string collection

            //create string collection
            var greeting = new[] {"Hello", "World"};

            //stringCollection.Join(separator) joins strings in collection together with a separator
            Console.WriteLine(greeting.Join(", "));
        }

        public static void Reverse()
        {
            //this example shows how to reverse a string

            //create string
            var greeting = "Hello, World";

            //string.Reverse reverses the string
            Console.WriteLine(greeting.Reverse());
        }

        public static void Repeat()
        {
            //this example shows how to repeat a string for specified times

            //create string
            var greeting = "Hello, ".Repeat(3);

            //string.Repeat(times) repeats the string for specified times
            Console.WriteLine(greeting);
        }

        public static void ToString_()
        {
            //this example shows how to create string from collection

            //create collection
            var nums = new[] {1, 2, 3};

            //collection.ToString(separator) is to convert collection into string with a separator
            Console.WriteLine(nums.ToString("."));
        }

        public static void ToString_With_Format()
        {
            //this example shows how to create formated string from collection

            //create collection
            var nums = new[] {1, 2, 3};

            //collection.ToString(separator,format) can format the result string
            Console.WriteLine(nums.ToString(", ", "C"));
        }

        public static void ToString_With_FormatProvider()
        {
            //this example shows how to create formated string by format provider from collection

            //create collection
            var nums = new[] {1, 2, 3};

            //collection.ToString(separator,format,provider) can format the result string with format provider
            foreach (var ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures).Take(10))
                Console.WriteLine("Format for {0}: {1}", ci.Name, nums.ToString(", ", "C", ci));
        }

        public static void Sample_Find_Largest_Palindrome()
        {
            //this example shows how to use string.Reverse to solve problem

            //Find the largest palindrome made from the product of two 3-digit numbers
            //(Question form http://projecteuler.net)
            //(Palindrome is number which can be read from either direction)
            Console.WriteLine((from x in 100.To(999)
                               from y in x.To(999)
                               select x * y
                               into z
                               where z.ToString() == z.ToString().Reverse()
                               select z).Max()
                );
        }
    }
}
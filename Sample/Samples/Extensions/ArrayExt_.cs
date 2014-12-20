using System;

//Chaow.Extensions extends .NET framework types

namespace Chaow.Extensions
{
    //ArrayExt makes you create Array easier
    public static class ArrayExt_
    {
        public static void Array_Create()
        {
            //this example shows how to create non-default value array

            //use ArrayExt.Create(length, defaultValue) to create non-default value array
            var bools = ArrayExt.Create(5, true);

            //show results
            Console.WriteLine(bools.ToString(", "));
        }

        public static void Array_Copy()
        {
            //this example shows how to copy an array

            //create array
            var primes = new[] {2, 3, 5, 7, 11, 13, 17, 19};

            //use array.Copy(index,length) to copy array
            //index is start index of original array, if you omit index, 0 will be assumed
            //length is length to copy, if you omit length, original array length will be assumed
            var primeCopy = primes.Copy();
            var primeLen5 = primeCopy.Copy(5);
            var primeSlice = primeLen5.Copy(1, 3);
            Console.WriteLine("primeCopy is {0}", primeCopy.ToString(", "));
            Console.WriteLine("primeLen5 is {0}", primeLen5.ToString(", "));
            Console.WriteLine("primeSlice is {0}", primeSlice.ToString(", "));
        }

        public static void Append_Array()
        {
            //this example shows how to copy and append an element into an array

            //create array
            var nums = new[] {1, 2, 3};

            //array.Append(item) to create a new copy and appends new member to index
            var result = nums.Append(4);

            //show result
            Console.WriteLine("Append 4 to {0} is", nums.ToString(", "));
            Console.WriteLine(result.ToString(", "));
        }

        public static void Enum()
        {
            //this example shows how to create array from enum type

            //ArrayExt.Enum<enumType> create array of the enum type
            var days = ArrayExt.Enum<DayOfWeek>();

            //show result
            Console.WriteLine(days.ToString(", "));
        }
    }
}
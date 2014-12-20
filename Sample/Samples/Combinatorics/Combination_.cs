using System;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    //Combination allows you to create unordered combinations from collection
    public static class Combination_
    {
        public static void Combination_Create()
        {
            //this example shows how to create combination

            //you can create combination by collection.Combine(length)
            var solutions = "abcd".Combine(2);

            //show results
            solutions.ForEach(s => Console.WriteLine(s.ToString("")));
        }

        public static void Combination_Create_2()
        {
            //this example shows how to various types of combination

            //shows results
            Console.WriteLine("Normal Combination");
            Console.WriteLine("aabc".Combine(2, CombinatoricModel.Normal).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Distinct Combination");
            Console.WriteLine("aabc".Combine(2, CombinatoricModel.Distinct).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Repetition Combination");
            Console.WriteLine("aabc".Combine(2, CombinatoricModel.Repetition).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Repetition-Distinct Combination");
            Console.WriteLine("aabc".Combine(2, CombinatoricModel.RepetitionDistinct).Select(s => s.ToString("")).ToString(", "));
        }

        public static void Combination_As_Collection()
        {
            //this example shows how to use Count/Contains

            //create combination
            var solutions = "abcd".Combine(2);

            //use combination.Count to do counting
            //this class has algorithm to do fast counting
            Console.WriteLine("Count of abcd combination is {0}", solutions.Count);

            //use combination.Contains(item) to get whethers item is in the collection
            //this class has algorithm to do fast getting contain
            Console.WriteLine("Does abcd combination contain 'bd'?");
            Console.WriteLine(solutions.Contains("bd"));
        }

        public static void Combination_As_List()
        {
            //this example shows how to use get_Item/IndexOf

            //create combination
            var solutions = "abcd".Combine(2);

            //use combination[index] to get item from index
            //this class has algorithm to do fast getting item from index
            Console.WriteLine("3rd of abcd combination is {0}", solutions[3].ToString(""));

            //use combination.IndexOf(item) to get index from the item
            //this class has algorithm to get fast index of
            Console.WriteLine("Index of 'bc' in abcd combination is {0}", solutions.IndexOf("bc"));
        }

        public static void Sample_Not_Bouncy()
        {
            //this example shows how to use combination to solve problem
            //How many numbers below a googol (10^100) are not bouncy?
            //Question from http://projecteuler.net

            //increasing number is number which each digit keep increasing or equal ie. 112234
            //decreasing number is number which each digit keep decreasing or equal ie. 997655
            //bouncy number is number which is not increasing and decreasing number
            //we can use Combination to find increasing and decreasing number

            var count = 0L;
            for (var n = 1; n <= 100; n++)
                count += 1.To(9).Combine(n, CombinatoricModel.Repetition).LongCount +
                         9.DownTo(0).Combine(n, CombinatoricModel.Repetition).LongCount - 10;
            Console.WriteLine(count);
        }
    }
}
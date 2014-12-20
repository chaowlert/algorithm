using System;
using System.Linq;
using Chaow.Extensions;
using Chaow.Numeric.Sequence;

namespace Chaow.Combinatorics
{
    //Permutation allows you to create ordered combinations from collection
    public static class Permutation_
    {
        public static void Permutation_Create()
        {
            //this example shows how to create permutation

            //you can create permutation by collection.Permute(length)
            //you can omit length, if length is omitted, full length will be assumed
            var solutions = "abc".Permute();

            //show results
            solutions.ForEach(s => Console.WriteLine(s.ToString("")));
        }

        public static void Permutation_Create_2()
        {
            //this example shows how to various types of permutation

            //shows results
            Console.WriteLine("Normal Permutation");
            Console.WriteLine("aabc".Permute(2, CombinatoricModel.Normal).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Distinct Permutation");
            Console.WriteLine("aabc".Permute(2, CombinatoricModel.Distinct).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Repetition Permutation");
            Console.WriteLine("aabc".Permute(2, CombinatoricModel.Repetition).Select(s => s.ToString("")).ToString(", "));
            Console.WriteLine();
            Console.WriteLine("Repetition-Distinct Permutation");
            Console.WriteLine("aabc".Permute(2, CombinatoricModel.RepetitionDistinct).Select(s => s.ToString("")).ToString(", "));
        }

        public static void Permutation_As_Collection()
        {
            //this example shows how to use Count/Contains

            //create combination
            var solutions = "abcd".Permute(2);

            //use permutation.Count to do counting
            //this class has algorithm to do fast counting
            Console.WriteLine("Count of abcd permutation is {0}", solutions.Count);

            //use permutation.Contains(item) to get whethers item is in the collection
            //this class has algorithm to do fast getting contain
            Console.WriteLine("Does abcd permutation contain 'db'?");
            Console.WriteLine(solutions.Contains("db"));
        }

        public static void Permutation_As_List()
        {
            //this example shows how to use get_Item/IndexOf

            //create permutation
            var solutions = "abcd".Permute(2);

            //use permutation[index] to get item from index
            //this class has algorithm to do fast getting item from index
            Console.WriteLine("3rd of abcd permutation is {0}", solutions[3].ToString(""));

            //use permutation.IndexOf(item) to get index from the item
            //this class has algorithm to get fast index of
            Console.WriteLine("Index of 'ba' in abcd permutation is {0}", solutions.IndexOf("ba"));
        }

        public static void Sample_Get_Index()
        {
            //this example shows how to use permutation to solve problem

            //What is the millionth lexicographic permutation of the digits 0, 1, 2, 3, 4, 5, 6, 7, 8 and 9?
            //Question from http://projecteuler.net
            Console.WriteLine(0.To(9).Permute()[999999].ToString(""));
        }

        public static void Sample_Find_Largest_Prime()
        {
            //this example shows how to use permutation to solve problem

            //What is the largest n-digit pandigital prime that exists?
            //Question from http://projecteuler.net

            //There is no prime for 1 to 9 since they all can be divided by 3
            //There is no prime for 1 to 8 since they all also can be divided by 3

            //create prime
            var prime = new Prime();
            Console.WriteLine((from sol in 7.DownTo(1).Permute()
                               select sol.Aggregate((a, b) => a * 10 + b)).First(p => prime.Contains(p)));
        }

        public static void Sample_5()
        {
            1.To(5).Permute().ForEach(s => Console.WriteLine(s.ToString(",")));
        }
    }
}
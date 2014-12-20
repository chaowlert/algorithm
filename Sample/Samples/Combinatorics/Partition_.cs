using System;
using Chaow.Extensions;

namespace Chaow.Combinatorics
{
    //Partition allows you to get combination of summing to a number
    public static class Partition_
    {
        public static void Partition_Create()
        {
            //this example shows how to create partition

            //use collection.Partition(sum) to create partition
            var solutions = 1.To(4).Partition(4);

            //show results
            solutions.ForEach(s => Console.WriteLine(s.ToString("+")));
        }

        public static void Partition_As_Collection()
        {
            //this example shows you can use Count/Contains on partition

            //create partition
            var solutions = 1.To(4).Partition(4);

            //use partition.Count to get count
            //this class has algorithm to do fast counting
            Console.WriteLine("Number of solutions of summing to 4 is {0}", solutions.Count);

            //use partition.Contains(collection) to get contains
            //this class has algorithm to do fast getting contain
            Console.WriteLine("Is summing to 4 contains 1+1+1+1?");
            Console.WriteLine(solutions.Contains(new[] {1, 1, 1, 1}));
        }

        public static void Partition_As_List()
        {
            //this example shows you can use get_Item/IndexOf on partition

            //create partition
            var solutions = 1.To(4).Partition(4);

            //use partition[index] to get item from index
            //this class has algorithm to do fast getting item from index
            Console.WriteLine("3th solution of summing to 4 is {0}", solutions[3].ToString("+"));

            //use partition.IndexOf(item) to get index from the item
            //this class has algorithm to get fast index of
            Console.WriteLine("Index of 2+2 in summing to 4 is {0}", solutions.IndexOf(new[] {2, 2}));
        }

        public static void Sample_Exchange()
        {
            //this example shows how to use partition to solve problem

            //How many different ways can £2 be made using any number of coins?
            //Question from http://projecteuler.net

            //Coins are 1p, 2p, 5p, 10p, 20p, 50p, £1 (100p) and £2 (200p)
            Console.WriteLine(new[] {1, 2, 5, 10, 20, 50, 100, 200}.Partition(200).Count);
        }

        public static void Sample_Sum_to_100()
        {
            //this example shows how to use partition to solve problem

            //How many different ways can one hundred be written as a sum of at least two positive integers?
            //Question from http://projecteuler.net
            Console.WriteLine(1.To(99).Partition(100).Count);
        }
    }
}

using System;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Algorithms
{
    //binary indexed tree maintains running sum over a set of numbers
    public static class BinaryIndexedTree_
    {
        public static void Sample()
        {
            //create tree
            var data = new[] {1, 0, 2, 1, 1, 3, 0, 4, 2, 5, 2, 2, 3, 1, 0};
            var tree = new BinaryIndexedTree(data);

            //get
            Console.WriteLine("Value                  : {0}", 0.To(14).Select(tree.Get).ToString(", ", "00"));
            
            //sum
            Console.WriteLine("Running sum            : {0}", 0.To(14).Select(i => tree.Sum(i)).ToString(", ", "00"));

            //add
            tree.Add(5, 1);
            Console.WriteLine("Value after add to id 5: {0}", 0.To(14).Select(tree.Get).ToString(", ", "00"));
            Console.WriteLine("Running sum after add  : {0}", 0.To(14).Select(i => tree.Sum(i)).ToString(", ", "00"));

            //sum range
            Console.WriteLine("Sum for position 2-5: {0}", tree.Sum(2, 5));

            //find sum
            for (var i = 0; i <= 30; i += 5)
                Console.WriteLine("Sum of {0} is at position: {1}", i, tree.Find(i));
        }
    }
}

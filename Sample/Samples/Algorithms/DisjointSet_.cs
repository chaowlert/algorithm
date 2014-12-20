using System;
using System.Linq;
using Chaow.Extensions;

namespace Chaow.Algorithms
{
    //DisjoinSet allows you to group members in collection
    public static class DisjointSet_
    {
        public static void DisjointSet_Simple()
        {
            //this example shows how to group collections

            //create disjoint set
            var disjoint = new DisjointSet<int>();
            disjoint.Union(2, 3);
            disjoint.Union(5, 7);
            disjoint.Union(11, 13);
            disjoint.Union(1, 2);
            disjoint.Union(3, 4);
            disjoint.Union(5, 6);

            //show results
            foreach (var grp in disjoint)
                Console.WriteLine(grp.ToString(", "));
        }

        public static void DisjointSet_GroupCount()
        {
            //this example shows how to get number of groups

            //create disjoint set
            var disjoint = new DisjointSet<int>();
            disjoint.Union(2, 3);
            disjoint.Union(5, 7);
            disjoint.Union(11, 13);
            disjoint.Union(1, 2);
            disjoint.Union(3, 4);
            disjoint.Union(5, 6);

            //show
            foreach (var grp in disjoint)
                Console.WriteLine(grp.ToString(", "));

            //use disjoint.GroupCount to get number of groups
            Console.WriteLine("Number of groups in this set is {0}", disjoint.GroupCount);
        }

        public static void DisjointSet_SizeOf()
        {
            //this example shows how to get size of the group from an item

            //create disjoint set
            var disjoint = new DisjointSet<int>();
            disjoint.Union(2, 3);
            disjoint.Union(5, 7);
            disjoint.Union(11, 13);
            disjoint.Union(1, 2);
            disjoint.Union(3, 4);
            disjoint.Union(5, 6);

            //show
            foreach (var grp in disjoint)
                Console.WriteLine(grp.ToString(", "));

            //use disjoint.SizeOf(item) to get size of the group from an item
            Console.WriteLine();
            Console.WriteLine("Size of group of {0} is {1}", 1, disjoint.SizeOf(1));
        }

        public static void DisjointSet_IsUnion()
        {
            //this example shows how to test whether 2 items are in the same group

            //create disjoint set
            var disjoint = new DisjointSet<int>();
            disjoint.Union(2, 3);
            disjoint.Union(5, 7);
            disjoint.Union(11, 13);
            disjoint.Union(1, 2);
            disjoint.Union(3, 4);
            disjoint.Union(5, 6);

            //show
            foreach (var grp in disjoint)
                Console.WriteLine(grp.ToString(", "));

            //use disjoint.IsUnion(item1,item2) to test whether 2 items are in the same group
            Console.WriteLine();
            Console.WriteLine("Are {0} and {1} in the same group?: {2}", 2, 3, disjoint.IsUnion(2, 3));
            Console.WriteLine("Are {0} and {1} in the same group?: {2}", 3, 5, disjoint.IsUnion(3, 5));
        }

        public static void DisjointSet_GroupOf()
        {
            //this example shows how to get members of a specified group

            //create disjoint set
            var disjoint = new DisjointSet<int>();
            disjoint.Union(2, 3);
            disjoint.Union(5, 7);
            disjoint.Union(11, 13);
            disjoint.Union(1, 2);
            disjoint.Union(3, 4);
            disjoint.Union(5, 6);

            //show
            foreach (var grp in disjoint)
                Console.WriteLine(grp.ToString(", "));

            //use disjoint.GroupOf(item) to get members of a specified group
            Console.WriteLine();
            Console.WriteLine("Group of {0} is {1}", 5, disjoint.GroupOf(5).ToString(", "));
        }

        public static void Sample_Minimum_Span_Tree()
        {
            //this example shows how to use disjoint set to solve problem

            //this is cost of network in seven nodes A to G
            //   A  B  C  D  E  F  G
            //A  - 16 12 21  -  -  -
            //B 16  -  - 17 20  -  -
            //C 12  -  - 28  - 31  -
            //D 21 17 28  - 18 19 23
            //E  - 20  - 18  -  - 11
            //F  -  - 31 19  -  - 27
            //G  -  -  - 23 11 27  -
            var costList = new[]
            {
                Tuple.Create('A', 'B', 16),
                Tuple.Create('A', 'C', 12),
                Tuple.Create('A', 'D', 21),
                Tuple.Create('B', 'D', 17),
                Tuple.Create('B', 'E', 20),
                Tuple.Create('C', 'D', 28),
                Tuple.Create('C', 'F', 31),
                Tuple.Create('D', 'E', 18),
                Tuple.Create('D', 'F', 19),
                Tuple.Create('D', 'G', 23),
                Tuple.Create('E', 'G', 11),
                Tuple.Create('F', 'G', 27)
            };

            //get current cost
            Console.WriteLine("Current cost of network is {0}", costList.Sum(c => c.Item3));

            //sort the cost and reconnect the network
            //do until you get only one group
            var disjoint = new DisjointSet<char>();
            var sum = 0;
            Console.WriteLine();
            foreach (var cost in costList.OrderBy(c => c.Item3))
            {
                if (!disjoint.IsUnion(cost.Item1, cost.Item2))
                {
                    disjoint.Union(cost.Item1, cost.Item2);
                    Console.WriteLine("Connect {0} and {1}", cost.Item1, cost.Item2);
                    sum += cost.Item3;
                    if (disjoint.SizeOf('A') == 7)
                        break;
                }
            }
            Console.WriteLine("Current cost of network is {0}", sum);
        }
    }
}
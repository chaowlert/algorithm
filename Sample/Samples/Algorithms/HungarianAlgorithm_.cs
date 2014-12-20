using System;

namespace Chaow.Algorithms
{
    //HungarianAlgorithm is for solving assignment problem
    public static class HungarianAlgorithm_
    {
        public static void FindAssignments()
        {
            //      Clean bathroom  Sweep floors    Wash windows
            //Jim   $3              $4              $5
            //Steve $4              $4              $4
            //Alan  $3              $5              $3

            //how to assign persons to tasks to minimize cost?

            var array = new[,]
            {
                {3, 4, 5},
                {4, 4, 4},
                {3, 5, 3},
            };
            var result = array.FindAssignments();

            var persons = new[] {"Jim", "Steve", "Alan"};
            var jobs = new[] {"Clean bathroom", "Sweep floors", "Wash windows"};
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine("Assign {0} to {1}", persons[i], jobs[result[i]]);
            }
        }
    }
}

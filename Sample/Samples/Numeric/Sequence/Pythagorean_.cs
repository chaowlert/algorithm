using System;
using Chaow.Extensions;

namespace Chaow.Numeric.Sequence
{
    //Pythagorean represents sequence of primitive Pythagorean triples
    public static class Pythagorean_
    {
        public static void Pythagorean()
        {
            //this example shows how to use Pythagorean sequence

            //Pythagorean triple is triple (a,b,c) which a^2 + b^2 = c^2
            //primitive Pythagorean triple is triple with (a,b,c) are all coprime with each other
            //here are first 25 primitive Pythagorean triple
            new Pythagorean().Take(25).ForEach(py => Console.WriteLine(py));
        }

        public static void PythagoreanTriple()
        {
            //this example shows how to use PythagoreanTriple

            //get a PythagoreanTriple
            var py = new Pythagorean().First();

            //here are members of PythagoreanTriple
            Console.WriteLine("This triple is {0}", py);
            Console.WriteLine("Seed1 is {0}", py.Seed1);
            Console.WriteLine("Seed2 is {0}", py.Seed2);
            Console.WriteLine("SideA is {0} <-- side a = seed1^2 - seed2^2", py.SideA);
            Console.WriteLine("SideB is {0} <-- side b = 2 * seed1 * seed2", py.SideB);
            Console.WriteLine("SideC is {0} <-- side c = seed1^2 + seed2^2", py.SideC);
            Console.WriteLine("Perimeter is {0}", py.Perimeter);
            Console.WriteLine("Size is {0}", py.Size);
            Console.WriteLine("Change to size 3 is {0}", py.ToSize(3));
        }

        public static void Sample_Pythagorean_1()
        {
            //this example shows how to use Pythagorean to solve problem

            //Find product abc of Pythagorean triplet, for which a + b + c = 1000
            //(Question from http://projecteuler.net)
            var py = new Pythagorean().First(p => 1000 % p.Perimeter == 0);
            py = py.ToSize(1000 / py.Perimeter);
            Console.WriteLine("Pythagorean triple which a + b + c = 1000 is {0}", py);
            Console.WriteLine("Product abc is {0}", py.SideA * py.SideB * py.SideC);
        }

        public static void Sample_Pythagorean_2()
        {
            //this example shows how to use Pythagorean to solve problem

            //If p is the perimeter of a right angle triangle, {a, b, c}, which value, for p ≤ 1000, has the most solutions?
            //(Question from http://projecteuler.net)
            var list = new int[1000];
            var maxSeed1 = 500.Sqrt(); //maxSeed1 = Sqrt(targetPerimeter/2)

            foreach (var py in new Pythagorean().TakeWhile(p => p.Seed1 <= maxSeed1))
            {
                for (var i = py.Perimeter; i < 1000; i += py.Perimeter)
                    list[i]++;
            }
            Console.WriteLine(0.To(999).MaxBy(x => list[x]));
        }
    }
}
using System;
using System.Collections.ObjectModel;
using Chaow.Extensions;

namespace Chaow.Numeric.Analysis
{
    //Diophantine represents integer solutions for aXX + bXY + cYY + dX + eY + f = 0
    //Code are ported from http://www.alpertron.com.ar/QUAD.HTM
    public static class Diophantine_
    {
        public static void Create()
        {
            //this example shows how to create Diophantine

            //you can create Diophantine by new Diophantine(a,b,c,d,e,f)
            //where aXX + bXY + cYY + dX + eY + f = 0

            //for example 5X + -25 = 0
            var solutions = new Diophantine(0, 0, 0, 5, 0, -25).Solutions;

            //show results
            Console.WriteLine(solutions[0].X0);
        }

        public static void Create2()
        {
            //this example shows how to create Diophantine

            //you can create Diophantine by Diophantine.Parse(formula)

            //for example 2X + 4X - 54 = 0
            var solutions = Diophantine.Parse((x, y) => 2 * x + 4 * x - 54).Solutions;

            //show results
            Console.WriteLine(solutions[0].X0);
        }

        public static void Linear()
        {
            //this example shows how to solve linear formula dX + eY + f = 0

            //10X + 84Y + 16 = 0
            var solutions = Diophantine.Parse((x, y) => 10 * x + 84 * y + 16).Solutions;

            //show formula
            Console.WriteLine("Solutions for X: {0}", solutions[0].FuncX1.Body);
            Console.WriteLine("Solutions for Y: {0}", solutions[0].FuncY1.Body);

            //show results
            Console.WriteLine();
            solutions[0].Take(10).ForEach(s =>
                Console.WriteLine("10*{0} + 84*{1} + 16 = {2}", s.X, s.Y, 10 * s.X + 84 * s.Y + 16));
        }

        public static void SimpleHyperbolic()
        {
            //this example shows how to solve simple hyperbolic formula bXY + dX + eY + f = 0

            //2XY + 5X + 56Y + 7 = 0
            var solutions = Diophantine.Parse((x, y) => 2 * x * y + 5 * x + 56 * y + 7).Solutions;

            //show results
            solutions.ForEach(s =>
                Console.WriteLine("2*{0}*{1} + 5*{0} + 56*{1} + 7 = {2}", s.X0, s.Y0, 2 * s.X0 * s.Y0 + 5 * s.X0 + 56 * s.Y0 + 7));
        }

        public static void Elliptical()
        {
            //this example shows how to solve elliptical formula where b^2 - 4ac < 0

            //42XX + 8XY + 15YY + 23X + 17Y - 4915 = 0
            var solutions = Diophantine.Parse((x, y) => 42 * x * x + 8 * x * y + 15 * y * y + 23 * x + 17 * y - 4915).Solutions;

            //show results
            solutions.ForEach(s =>
                Console.WriteLine("42*{0}*{0} + 8*{0}*{1} + 15*{1}*{1} + 23*{0} + 17*{1} - 4915 = {2}", s.X0, s.Y0, 42 * s.X0 * s.X0 + 8 * s.X0 * s.Y0 + 15 * s.Y0 * s.Y0 + 23 * s.X0 + 17 * s.Y0 - 4915));
        }

        public static void Parabolic()
        {
            //this example shows how to solve parabolic formula where b^2 - 4ac = 0

            //8XX - 24XY + 18YY + 5X + 7Y + 16 = 0
            var solutions = Diophantine.Parse((x, y) => 8 * x * x - 24 * x * y + 18 * y * y + 5 * x + 7 * y + 16).Solutions;

            //show formula
            for (var i = 0; i < solutions.Count; i++)
            {
                Console.WriteLine("Solution {0}", i + 1);
                Console.WriteLine("Solutions for X: {0}", solutions[i].FuncX1.Body);
                Console.WriteLine("Solutions for Y: {0}", solutions[i].FuncY1.Body);
                Console.WriteLine();
            }

            //show results
            foreach (var solution in solutions)
            {
                solution.Take(5).ForEach(s =>
                    Console.WriteLine("8*{0}*{0} - 24*{0}*{1} + 18*{1}*{1} + 5*{0} + 7*{1} + 16 = {2}", s.X, s.Y, 8 * s.X * s.X - 24 * s.X * s.Y + 18 * s.Y * s.Y + 5 * s.X + 7 * s.Y + 16));
                Console.WriteLine();
            }
        }

        public static void Hyperbolic()
        {
            //this example shows how to solve hyperbolic formula aXX + bXY + cYY + f = 0

            //18XX + 41XY + 19YY - 24 = 0
            var solutions = Diophantine.Parse((x, y) => 18 * x * x + 41 * x * y + 19 * y * y - 24).Solutions;

            //show formula
            Console.WriteLine("Solutions for X: {0}", solutions[0].FuncX1.Body);
            Console.WriteLine("Solutions for Y: {0}", solutions[0].FuncY1.Body);
            Console.WriteLine();

            //show X0 and Y0
            foreach (var solution in solutions)
            {
                Console.WriteLine("X: {0}", solution.X0);
                Console.WriteLine("Y: {0}", solution.Y0);
                Console.WriteLine();
            }

            //show results
            foreach (var solution in solutions)
            {
                solution.Take(3).ForEach(s =>
                    Console.WriteLine("18*{0}*{0} + 41*{0}*{1} + 19*{1}*{1} - 24 = {2}", s.X, s.Y, 18 * s.X * s.X + 41 * s.X * s.Y + 19 * s.Y * s.Y - 24));
                Console.WriteLine();
            }
        }

        public static void ComplexHyperbolic()
        {
            //this example shows how to solve hyperbolic formula aXX + bXY + cYY + dX + eY + f = 0

            //3XX + 13XY + 5YY - 11X - 7Y - 92 = 0
            var solutions = Diophantine.Parse((x, y) => 3 * x * x + 13 * x * y + 5 * y * y - 11 * x - 7 * y - 92).Solutions;

            //show formula
            Console.WriteLine("Solutions for X: {0}", solutions[0].FuncX1.Body);
            Console.WriteLine("Solutions for Y: {0}", solutions[0].FuncY1.Body);
            Console.WriteLine();

            //show X0 and Y0
            foreach (var solution in solutions)
            {
                Console.WriteLine("X: {0}", solution.X0);
                Console.WriteLine("Y: {0}", solution.Y0);
                Console.WriteLine();
            }

            //show results
            foreach (var solution in solutions)
            {
                solution.Take(3).ForEach(s =>
                    Console.WriteLine("3*{0}*{0} + 13*{0}*{1} + 5*{1}*{1} - 11*{0} - 7*{1} - 92 = {2}", s.X, s.Y, 3 * s.X * s.X + 13 * s.X * s.Y + 5 * s.Y * s.Y - 11 * s.X - 7 * s.Y - 92));
                Console.WriteLine();
            }
        }

        public static void ComplexHyperbolic2()
        {
            //this example shows how to solve hyperbolic formula aXX + bXY + cYY + dX + eY + f = 0

            //3XX + 14XY + 6YY - 17X - 23Y - 505 = 0
            var solutions = Diophantine.Parse((x, y) => 3 * x * x + 14 * x * y + 6 * y * y - 17 * x - 23 * y - 505).Solutions;

            //show formula
            Console.WriteLine("Solutions for X: {0}", solutions[0].FuncX1.Body);
            Console.WriteLine("Solutions for Y: {0}", solutions[0].FuncY1.Body);
            Console.WriteLine();

            //show X0 and Y0
            foreach (var solution in solutions)
            {
                Console.WriteLine("X: {0}", solution.X0);
                Console.WriteLine("Y: {0}", solution.Y0);
                Console.WriteLine();
            }

            //show results
            foreach (var solution in solutions)
            {
                solution.Take(3).ForEach(s =>
                    Console.WriteLine("3*{0}*{0} + 14*{0}*{1} + 6*{1}*{1} - 17*{0} - 23*{1} - 505 = {2}", s.X, s.Y, 3 * s.X * s.X + 14 * s.X * s.Y + 6 * s.Y * s.Y - 17 * s.X - 23 * s.Y - 505));
                Console.WriteLine();
            }
        }

        public static void Sample_Blue_Disc()
        {
            //this example shows how to use diophantine to solve problem

            //Consider 15 Blue Discs and 6 Red Discs
            //Chance to pick 2 Blue Discs at random is 1/2 (15/21 * 14/20)

            //Find the first arrangement
            //where chance to pick 2 Blue Discs at random is 1/2
            //but total number of Discs > 10^12
            //Question from http://projecteuler.net

            //formula X/(X + Y) * (X - 1)/(X + Y - 1) = 1/2
            //2 * X * (X - 1) = (X + Y) * (X + Y - 1)
            //2 * X * (X - 1) - (X + Y) * (X + Y - 1) = 0
            var solutions = Diophantine.Parse((x, y) => 2 * x * (x - 1) - (x + y) * (x + y - 1)).Solutions;

            //show formula
            Console.WriteLine("Solutions for X: {0}", solutions[0].FuncX1.Body);
            Console.WriteLine("Solutions for Y: {0}", solutions[0].FuncY1.Body);
            Console.WriteLine();

            //show X0, Y0
            Console.WriteLine("X: {0}", solutions[0].X0);
            Console.WriteLine("Y: {0}", solutions[0].Y0);
            Console.WriteLine();

            //show results
            Console.WriteLine(solutions[0].First(s => s.X + s.Y > 1000000000000));
        }
    }
}
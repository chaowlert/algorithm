using System;

//Chaow.Samples.Puzzles is collections of games and puzzles

namespace Chaow.Samples.Puzzles
{
    //Nonogram is paint by number game
    public static class Nonogram_
    {
        public static void Nonogram_Soccer()
        {
            //this puzzle is from http://en.wikipedia.org/wiki/Nonogram

            var rows = new[]
            {
                new[] {3},
                new[] {5},
                new[] {3, 1},
                new[] {2, 1},
                new[] {3, 3, 4},
                new[] {2, 2, 7},
                new[] {6, 1, 1},
                new[] {4, 2, 2},
                new[] {1, 1},
                new[] {3, 1},
                new[] {6},
                new[] {2, 7},
                new[] {6, 3, 1},
                new[] {1, 2, 2, 1, 1},
                new[] {4, 1, 1, 3},
                new[] {4, 2, 2},
                new[] {3, 3, 1},
                new[] {3, 3},
                new[] {3},
                new[] {2, 1}
            };
            var cols = new[]
            {
                new[] {2},
                new[] {1, 2},
                new[] {2, 3},
                new[] {2, 3},
                new[] {3, 1, 1},
                new[] {2, 1, 1},
                new[] {1, 1, 1, 2, 2},
                new[] {1, 1, 3, 1, 3},
                new[] {2, 6, 4},
                new[] {3, 3, 9, 1},
                new[] {5, 3, 2},
                new[] {3, 1, 2, 2},
                new[] {2, 1, 7},
                new[] {3, 3, 2},
                new[] {2, 4},
                new[] {2, 1, 2},
                new[] {2, 2, 1},
                new[] {2, 2},
                new[] {1},
                new[] {1}
            };
            var nonogram = new Nonogram(rows, cols);
            Console.WriteLine(nonogram);
        }

        public static void Nonogram_Flower()
        {
            //this puzzle is from http://www.comp.lancs.ac.uk/~ss/nonogram/

            var rows = new[]
            {
                new[] {2},
                new[] {1, 1},
                new[] {1, 1},
                new[] {2},
                new[] {2, 1},
                new[] {1, 2, 2},
                new[] {4, 1},
                new[] {3}
            };
            var cols = new[]
            {
                new[] {2},
                new[] {1, 1},
                new[] {2},
                new[] {2, 4},
                new[] {1, 1, 2},
                new[] {1, 1, 1, 1},
                new[] {2, 2},
                new[] {0}
            };
            var nonogram = new Nonogram(rows, cols);
            Console.WriteLine(nonogram);
        }

        public static void Nonogram_Dog()
        {
            //this puzzle is from http://www.comp.lancs.ac.uk/~ss/nonogram/

            var rows = new[]
            {
                new[] {3},
                new[] {2, 1},
                new[] {1, 1},
                new[] {1, 4},
                new[] {1, 1, 1, 1},
                new[] {2, 1, 1, 1},
                new[] {2, 1, 1},
                new[] {1, 2},
                new[] {2, 3},
                new[] {3}
            };
            var cols = new[]
            {
                new[] {3},
                new[] {2, 1},
                new[] {2, 2},
                new[] {2, 1},
                new[] {1, 2, 1},
                new[] {1, 1},
                new[] {1, 4, 1},
                new[] {1, 1, 2},
                new[] {3, 1},
                new[] {4}
            };
            var nonogram = new Nonogram(rows, cols);
            Console.WriteLine(nonogram);
        }

        public static void Nonogram_Rabbit()
        {
            //this puzzle is from http://www.comp.lancs.ac.uk/~ss/nonogram/

            var rows = new[]
            {
                new[] {2},
                new[] {4, 2},
                new[] {1, 1, 4},
                new[] {1, 1, 1, 1},
                new[] {1, 1, 1, 1},
                new[] {1, 1, 1, 1},
                new[] {1, 1, 1, 1},
                new[] {1, 1, 1, 1},
                new[] {1, 2, 2, 1},
                new[] {1, 3, 1},
                new[] {2, 1},
                new[] {1, 1, 1, 2},
                new[] {2, 1, 1, 1},
                new[] {1, 2},
                new[] {1, 2, 1}
            };
            var cols = new[]
            {
                new[] {3},
                new[] {3},
                new[] {10},
                new[] {2},
                new[] {2},
                new[] {8, 2},
                new[] {2},
                new[] {1, 2, 1},
                new[] {2, 1},
                new[] {7},
                new[] {2},
                new[] {2},
                new[] {10},
                new[] {3},
                new[] {2}
            };
            var nonogram = new Nonogram(rows, cols);
            Console.WriteLine(nonogram);
        }
    }
}
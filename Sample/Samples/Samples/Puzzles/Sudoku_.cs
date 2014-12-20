using System;

namespace Chaow.Samples.Puzzles
{
    //Sudoku is filling unique numbers game
    public static class Sudoku_
    {
        public static void Sudoku_Random()
        {
            //this example shows how to create new random game and solve sudoku by backtracking

            //use new Sudoku() to create new random game
            var sudoku = new Sudoku();
            Console.WriteLine(sudoku);

            //use sudoku.ToSolvedSudoku to solve the game
            sudoku = sudoku.ToSolvedSudoku();
            Console.WriteLine(sudoku);
        }

        public static void Sudoku_Backtrack()
        {
            //this example shows how to create static game
            //puzzle is from http://en.wikipedia.org/wiki/Sudoku

            //use new Sudoku(int[]) to create static game
            //for unknown number, use 0
            var sudoku = new Sudoku(
                new[]
                {
                    5, 3, 0, 0, 7, 0, 0, 0, 0,
                    6, 0, 0, 1, 9, 5, 0, 0, 0,
                    0, 9, 8, 0, 0, 0, 0, 6, 0,
                    8, 0, 0, 0, 6, 0, 0, 0, 3,
                    4, 0, 0, 8, 0, 3, 0, 0, 1,
                    7, 0, 0, 0, 2, 0, 0, 0, 6,
                    0, 6, 0, 0, 0, 0, 2, 8, 0,
                    0, 0, 0, 4, 1, 9, 0, 0, 5,
                    0, 0, 0, 0, 8, 0, 0, 7, 9
                });
            Console.WriteLine(sudoku);

            //solve the game
            sudoku = sudoku.ToSolvedSudoku();
            Console.WriteLine(sudoku);
        }

        public static void Sudoku_LookAhead()
        {
            //This example required LINCON module
            //Look ahead algorithm perform much faster than backtracking

            //create new game
            var sudoku = new Sudoku(
                new[]
                {
                    5, 3, 0, 0, 7, 0, 0, 0, 0,
                    6, 0, 0, 1, 9, 5, 0, 0, 0,
                    0, 9, 8, 0, 0, 0, 0, 6, 0,
                    8, 0, 0, 0, 6, 0, 0, 0, 3,
                    4, 0, 0, 8, 0, 3, 0, 0, 1,
                    7, 0, 0, 0, 2, 0, 0, 0, 6,
                    0, 6, 0, 0, 0, 0, 2, 8, 0,
                    0, 0, 0, 4, 1, 9, 0, 0, 5,
                    0, 0, 0, 0, 8, 0, 0, 7, 9
                });
            Console.WriteLine(sudoku);

            //change to lincon by setting solver to LinconSudokuSolver
            sudoku.Solver = LinconSudokuSolver.Default;

            //solve the game
            sudoku = sudoku.ToSolvedSudoku();
            Console.WriteLine(sudoku);
        }

        public static void WorstSudoku_LookAhead()
        {
            //This example shows solving near worst sudoku with Lincon
            //This example is from http://en.wikipedia.org/wiki/Algorithmics_of_sudoku

            //Backtracking may solve this problem in 20-30 mins
            //Concurrent backtracking may solve this problem in 10-20 mins
            //But Lincon can solve this in less than a sec

            //create new game
            var sudoku = new Sudoku(
                new[]
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 3, 0, 8, 5,
                    0, 0, 1, 0, 2, 0, 0, 0, 0,
                    0, 0, 0, 5, 0, 7, 0, 0, 0,
                    0, 0, 4, 0, 0, 0, 1, 0, 0,
                    0, 9, 0, 0, 0, 0, 0, 0, 0,
                    5, 0, 0, 0, 0, 0, 0, 7, 3,
                    0, 0, 2, 0, 1, 0, 0, 0, 0,
                    0, 0, 0, 0, 4, 0, 0, 0, 9
                });
            Console.WriteLine(sudoku);

            //change to lincon by setting solver to LinconSudokuSolver
            sudoku.Solver = LinconSudokuSolver.Default;

            //solve the game
            sudoku = sudoku.ToSolvedSudoku();
            Console.WriteLine(sudoku);
        }
    }
}
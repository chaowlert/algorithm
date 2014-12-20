using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Chaow.Combinatorics;
using Chaow.Extensions;
using Chaow.LINCON;
using Chaow.Numeric;
using Chaow.Threading;

namespace Chaow.Samples.Puzzles
{
    public interface ISudokuSolver
    {
        int[] ToSolvedSudoku(int[] table);
    }

    public class BacktrackSudokuSolver : ISudokuSolver
    {
        //static fields
        public static readonly BacktrackSudokuSolver Default = new BacktrackSudokuSolver();

        //public methods
        public int[] ToSolvedSudoku(int[] table)
        {
            var position = 0.To(80).OrderByDescending(x => table[x] > 0).ToArray();
            var valued = table.Where(x => x > 0).ToArray();

            var solutions = 1.To(9).Backtrack(
                valued,
                (set, cell, i) => set.Append(cell),
                set => set.Length == 81);

            solutions.AppendConstraint(
                set => cell => !set.Any((x, i) => x == cell && Sudoku.hasImpact(position[i], position[set.Length]))
                );

            var idx = 0;
            return solutions.SelectResults().First().OrderBy(x => position[idx++]).ToArray();
        }
    }

    public class LinconSudokuSolver : ISudokuSolver
    {
        //static SwappableCollection<int> source = new SwappableCollection<int>();
        static readonly int[][] source = new int[1][];
        static readonly ConstraintSolver<ReadOnlyCollection<int>> solver = from c in 1.To(9).ToConstraintList(81)
                                                                           from i in 0.To(8).ToConstraintIndex()
                                                                           from j in 0.To(80).ToConstraintIndex()
                                                                           where Constraint.AllDifferent(c[i * 9], c[i * 9 + 1], c[i * 9 + 2],
                                                                               c[i * 9 + 3], c[i * 9 + 4], c[i * 9 + 5],
                                                                               c[i * 9 + 6], c[i * 9 + 7], c[i * 9 + 8])
                                                                           where Constraint.AllDifferent(c[i], c[i + 9], c[i + 18],
                                                                               c[i + 27], c[i + 36], c[i + 45],
                                                                               c[i + 54], c[i + 63], c[i + 72])
                                                                           where Constraint.AllDifferent(c[((i % 3) * 3) + ((i / 3) * 27)],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 1],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 2],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 9],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 10],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 11],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 18],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 19],
                                                                               c[((i % 3) * 3) + ((i / 3) * 27) + 20])
                                                                           where (source.Protect()[0][j] > 0) ? c[j] == source.Protect()[0][j] : true
                                                                           //where source[0][j] > 0 ? c[j] == source[0][j] : true
                                                                           select c;

        public static readonly LinconSudokuSolver Default = new LinconSudokuSolver();

        public int[] ToSolvedSudoku(int[] table)
        {
            source[0] = table;
            return solver.First().ToArray();
        }
    }

    public class Sudoku
    {
        //fields
        readonly int[] _table;
        ISudokuSolver _solver;

        //properties

        //constructors
        public Sudoku() : this(BacktrackSudokuSolver.Default)
        {
        }

        public Sudoku(ISudokuSolver solver)
        {
            _table = generateNewSudoku();
            _solver = solver;
        }

        public Sudoku(int[] table) : this(BacktrackSudokuSolver.Default, table)
        {
        }

        public Sudoku(ISudokuSolver solver, int[] table)
        {
            if (table.Length != 81)
                throw new ArgumentException("table size must be 81", "table");
            if (table.Any(x => x < 0 || x > 9))
                throw new ArgumentException("Each value in table must be between 0 and 9");

            _table = table.Copy();
            _solver = solver;
        }

        public int this[int index]
        {
            get { return _table[index]; }
        }

        public int this[int row, int col]
        {
            get
            {
                if (row < 0 || row > 8 || col < 0 || col > 8)
                    throw new ArgumentOutOfRangeException("row", "row or column must be 0 to 8");
                return _table[row * 9 + col];
            }
        }

        public ISudokuSolver Solver
        {
            get { return _solver; }
            set { _solver = value; }
        }

        //public methods
        public Sudoku ToSolvedSudoku()
        {
            return new Sudoku(_solver, _solver.ToSolvedSudoku(_table));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    sb.Append(this[row, col] == 0 ? "." : this[row, col].ToString());
                    if (col == 2 || col == 5)
                        sb.Append(" | ");
                    else
                        sb.Append(" ");
                }
                sb.AppendLine();
                if (row == 2 || row == 5)
                    sb.AppendLine("---------------------");
            }
            return sb.ToString();
        }

        //static methods
        static int[] generateNewSudoku()
        {
            var table = new int[81];

            //Fill a random row
            var row = MathExt.Random.Next(9) * 9;
            1.To(9).RandomSamples(9).ForEach((x, i) => table[i + row] = x);
            table = BacktrackSudokuSolver.Default.ToSolvedSudoku(table);

            //Randomly remove fields & rotate number
            0.To(80).RandomSamples(49).ForEach(x => table[x] = 0);
            var rotate = 1.To(9).RandomSamples(9).ToArray();
            for (var i = 0; i < 81; i++)
            {
                if (table[i] > 0)
                    table[i] = rotate[table[i] - 1];
            }

            return table;
        }

        static int getRow(int x)
        {
            return x / 9;
        }

        static int getCol(int x)
        {
            return x % 9;
        }

        static int getZone(int x)
        {
            return (getRow(x) / 3) * 3 + (getCol(x) / 3);
        }

        internal static bool hasImpact(int x, int y)
        {
            return (getRow(x) == getRow(y)) ||
                   (getCol(x) == getCol(y)) ||
                   (getZone(x) == getZone(y));
        }
    }
}
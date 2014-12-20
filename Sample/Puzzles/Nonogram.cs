using System;
using System.Collections;
using System.Linq;
using System.Text;
using Chaow.Combinatorics;
using Chaow.Extensions;

namespace Chaow.Samples.Puzzles
{
    public interface INonogramSolver
    {
        BitArray SolveNonogram(int[][] rows, int[][] cols);
    }

    public class BacktackNonogramSolver : INonogramSolver
    {
        //static field
        public static readonly BacktackNonogramSolver Default = new BacktackNonogramSolver();

        //public methods
        public BitArray SolveNonogram(int[][] rows, int[][] cols)
        {
            var length = rows.Length * cols.Length;

            var solutions = CollectionExt.Booleans.Backtrack(
                new NonogramState(new int[cols.Length], ArrayExt.Create(cols.Length, -1), 0, -1, new BitArray(length), 0),
                (state, b, i) => state.AssignValue(rows, cols, b),
                state => state.Position == length);

            solutions.AppendConstraint(state => b => state.TestValue(rows, cols, b));

            return solutions.SelectResults(set => set.Table).First();
        }

        //child classes
        struct NonogramState
        {
            //fields
            readonly int[] _colIndexes;
            readonly int[] _colNum;
            readonly int _position;
            readonly int _rowIndex;
            readonly int _rowNum;
            readonly BitArray _table;

            public NonogramState(int[] colIndexes, int[] colNum, int rowIndex, int rowNum, BitArray table, int position)
            {
                _colIndexes = colIndexes;
                _colNum = colNum;
                _rowIndex = rowIndex;
                _rowNum = rowNum;
                _table = table;
                _position = position;
            }

            //properties
            public BitArray Table
            {
                get { return _table; }
            }

            public int Position
            {
                get { return _position; }
            }

            //constructors

            //static methods
            static bool checkValueForTrue(int num, int index, int[] nums)
            {
                if (num == 0)
                    return false;
                if (num < 0 && (index >= nums.Length || nums[index] == 0))
                    return false;
                return true;
            }

            static bool checkValueForFalse(int num, int index, int[] nums, int remaining)
            {
                if (num > 0)
                    return false;
                var sum = nums.Skip(index).Sum(i => i + 1) - 1;
                if (sum >= remaining)
                    return false;
                return true;
            }

            //public methods
            public bool TestValue(int[][] rows, int[][] cols, bool value)
            {
                int colPos;
                var rowPos = Math.DivRem(_position, cols.Length, out colPos);
                if (value)
                    return checkValueForTrue(_rowNum, _rowIndex, rows[rowPos])
                           && checkValueForTrue(_colNum[colPos], _colIndexes[colPos], cols[colPos]);
                return checkValueForFalse(_rowNum, _rowIndex, rows[rowPos], cols.Length - colPos)
                       && checkValueForFalse(_colNum[colPos], _colIndexes[colPos], cols[colPos], rows.Length - rowPos);
            }

            public NonogramState AssignValue(int[][] rows, int[][] cols, bool value)
            {
                var colIndexes = _colIndexes.Copy();
                var colNum = _colNum.Copy();
                var rowIndex = _rowIndex;
                var rowNum = _rowNum;
                var table = new BitArray(_table);
                var position = _position;
                int colPos;
                var rowPos = Math.DivRem(position, cols.Length, out colPos);

                if (value)
                {
                    if (rowNum < 0)
                    {
                        rowNum = rows[rowPos][rowIndex];
                        rowIndex++;
                    }
                    if (colNum[colPos] < 0)
                    {
                        colNum[colPos] = cols[colPos][colIndexes[colPos]];
                        colIndexes[colPos]++;
                    }
                    table[position] = true;
                }
                rowNum--;
                colNum[colPos]--;
                position++;
                if (colPos + 1 == cols.Length)
                {
                    rowIndex = 0;
                    rowNum = -1;
                }
                return new NonogramState(colIndexes, colNum, rowIndex, rowNum, table, position);
            }
        }
    }

    public class Nonogram
    {
        //fields
        readonly int _colLen;
        readonly int _rowLen;
        readonly BitArray _table;

        public Nonogram(int[][] rows, int[][] cols) : this(rows, cols, BacktackNonogramSolver.Default)
        {
        }

        public Nonogram(int[][] rows, int[][] cols, INonogramSolver solver)
        {
            _rowLen = rows.Length;
            _colLen = cols.Length;
            _table = solver.SolveNonogram(rows, cols);
        }

        //properties
        public bool this[int index]
        {
            get { return _table[index]; }
        }

        public bool this[int row, int col]
        {
            get { return _table[_colLen * row + col]; }
        }

        public int RowLength
        {
            get { return _rowLen; }
        }

        public int ColumnLength
        {
            get { return _colLen; }
        }

        //constructors

        //public methods
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var row = 0; row < _rowLen; row++)
            {
                for (var col = 0; col < _colLen; col++)
                {
                    if (this[row, col])
                        sb.Append("#");
                    else
                        sb.Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicDotsSolver.Solving
{
    [Flags]
    public enum PuzzleCellState
    {
        Empty = 1,
        Blocked = 2,
        Dot = 4,
        Up = 12,
        Down = 20,
        Left = 36,
        Right = 68,
        Vertical = 28,
        Horizontal = 100,
        Hint = 128
    }

    public enum LineType
    {
        Row,
        Column
    }

    public class PuzzleCellPointer
    {
        private readonly Puzzle _puzzle;
        private readonly int _x;
        private readonly int _y;



        public PuzzleCellPointer(Puzzle puzzle, int x, int y)
        {
            _puzzle = puzzle;
            _x = x;
            _y = y;
        }



        public PuzzleCellState Value
        {
            get { return _puzzle[_x, _y]; }
            set { _puzzle[_x, _y] = value; }
        }



        public IEnumerable<PuzzleCellPointer> Around
        {
            get
            {

                if (_y > 0)
                {
                    if (_x > 0)
                        yield return new PuzzleCellPointer(_puzzle, _x - 1, _y - 1);

                    yield return new PuzzleCellPointer(_puzzle, _x, _y - 1);

                    if (_x < _puzzle.BoardSize - 1)
                        yield return new PuzzleCellPointer(_puzzle, _x + 1, _y - 1);
                }

                if (_x < _puzzle.BoardSize - 1)
                    yield return new PuzzleCellPointer(_puzzle, _x + 1, _y);


                if (_x > 0)
                    yield return new PuzzleCellPointer(_puzzle, _x - 1, _y);


                if (_y < _puzzle.BoardSize - 1)
                {
                    if (_x > 0)
                        yield return new PuzzleCellPointer(_puzzle, _x - 1, _y + 1);

                    yield return new PuzzleCellPointer(_puzzle, _x, _y + 1);

                    if (_x < _puzzle.BoardSize - 1)
                        yield return new PuzzleCellPointer(_puzzle, _x + 1, _y + 1);
                }
            }
        }
    }


    public class Puzzle
    {
        private readonly string _path;
        private readonly int _boardSize;
        private readonly PuzzleCellState[,] _cellStates;

        private Queue<int> _pieces;

        private int[] _columnLimits;
        private int[] _rowLimits;



        public Puzzle(int boardSize)
        {
            _boardSize = boardSize;
            _cellStates = new PuzzleCellState[boardSize, boardSize];
            _pieces = new Queue<int>();

            //initialize with zeros
            _columnLimits = new int[boardSize];
            _rowLimits = new int[boardSize];
        }



        private Puzzle(Puzzle puzzle)
        {
            _path = puzzle.Path;
            _boardSize = puzzle._boardSize;

            _cellStates = new PuzzleCellState[_boardSize, _boardSize];
            for (int i = 0; i < _boardSize; i++)
                for (int j = 0; j < _boardSize; j++)
                    _cellStates[i, j] = puzzle._cellStates[i, j];

            _pieces = new Queue<int>(puzzle._pieces);
            _columnLimits = puzzle._columnLimits.ToArray();
            _rowLimits = puzzle._rowLimits.ToArray();
        }



        public Puzzle(String path)
        {
            _path = path;

            string[] lines = File.ReadAllLines(path);

            //first line, the size
            _boardSize = Int32.Parse(lines[0].Split(';')[0].Trim());

            //second line, the column limits
            _columnLimits = lines[1].Split(';')[0].Trim().Split(',').Select(x => Int32.Parse(x.Trim())).ToArray();

            //third line, the column limits
            _rowLimits = lines[2].Split(';')[0].Trim().Split(',').Select(x => Int32.Parse(x.Trim())).ToArray();

            //fourth line, the pieces
            _pieces = new Queue<int>(lines[3].Split(';')[0].Trim().Split(',').Select(x => Int32.Parse(x.Trim())).OrderByDescending(x => x));

            //the rest of the lines, the data
            _cellStates = new PuzzleCellState[_boardSize, _boardSize];
            for (int j = 0; j < _boardSize; j++)
            {
                var data = lines[4 + j].Split(';')[0].Trim().Split(',').Select(x => (PuzzleCellState) Enum.Parse(typeof(PuzzleCellState), x.Trim())).ToArray();
                for (int i = 0; i < _boardSize; i++)
                {
                    _cellStates[i, j] = data[i] | PuzzleCellState.Hint;
                }
            }
        }



        public int BoardSize
        {
            get { return _boardSize; }
        }



        public PuzzleCellState[,] CellStates
        {
            get { return _cellStates; }
        }



        public Queue<int> Pieces
        {
            get { return _pieces; }
            set { _pieces = value; }
        }



        public string Path
        {
            get { return _path; }
        }



        public int[] ColumnLimits
        {
            get { return _columnLimits; }
            set
            {
                if (value.Length != BoardSize)
                    throw new Exception("Number of values must be equal to the size of the board.");

                _columnLimits = value;
            }
        }



        public int[] RowLimits
        {
            get { return _rowLimits; }
            set
            {
                if (value.Length != BoardSize)
                    throw new Exception("Number of values must be equal to the size of the board.");

                _rowLimits = value;
            }
        }



        public PuzzleCellState this[int x, int y]
        {
            get { return _cellStates[x, y]; }
            set { _cellStates[x, y] = value; }
        }



        public int GetLineLimits(int index, LineType lineType)
        {
            if (lineType == LineType.Row)
                return RowLimits[index];

            return ColumnLimits[index];
        }



        public IEnumerable<PuzzleCellPointer> GetLine(int index, LineType lineType)
        {
            if (lineType == LineType.Row)
                return GetRow(index);

            return GetColumn(index);
        }



        public IEnumerable<PuzzleCellPointer> GetRow(int index)
        {
            for (int x = 0; x < _boardSize; x++)
                yield return new PuzzleCellPointer(this, x, index);
        }



        public IEnumerable<PuzzleCellPointer> GetColumn(int index)
        {
            for (int y = 0; y < _boardSize; y++)
                yield return new PuzzleCellPointer(this, index, y);
        }



        public IEnumerable<PuzzleCellPointer> GetDiagonals(int x, int y)
        {
            if (x >= 1 && y >= 1)
                yield return new PuzzleCellPointer(this, x - 1, y - 1);

            if (x >= 1 && y < _boardSize - 1)
                yield return new PuzzleCellPointer(this, x - 1, y + 1);

            if (x < _boardSize - 1 && y < _boardSize - 1)
                yield return new PuzzleCellPointer(this, x + 1, y + 1);

            if (x < _boardSize - 1 && y >= 1)
                yield return new PuzzleCellPointer(this, x + 1, y - 1);
        }



        public PuzzleCellPointer GetPointerOrDefault(int x, int y)
        {
            if (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
                return new PuzzleCellPointer(this, x, y);

            return null;
        }



        public Puzzle Clone()
        {
            return new Puzzle(this);
        }
    }
}
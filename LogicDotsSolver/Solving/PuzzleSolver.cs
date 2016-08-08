using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicDotsSolver.Solving
{
    public class PuzzleSolver
    {

        public static Puzzle Solve(Puzzle puzzle, int roundsLimit)
        {
            puzzles.Clear();

            RoundsLimit = puzzle.Pieces.Count - roundsLimit;

            //first, we can work with the hints that the puzzle already provides
            //this means connecting dots that imply that others are connected
            FillHints(puzzle);

            BlockHintedAreas(puzzle);

            BlockDiagonals(puzzle);

            //check below where the puzzle is clone!!!
            puzzle = DoRecursive(puzzle.Clone());

            //now, in the end, make all pieces look connected according to their proximity
            if(puzzle != null)
                FixDotConnections(puzzle);

            //Console.WriteLine("Total: " + count);

            return puzzle;
        }

        private static int RoundsLimit = 0;
        private static int count = 0;

        private static Puzzle DoRecursive(Puzzle puzzle)
        {
            FillReachedLimits(puzzle);

            //return puzzle;

            //if we have no more pieces to place, end this
            if (puzzle.Pieces.Count == 0)
                return puzzle;

            //if (rounds == 0)
            //    return puzzle;
                //Console.WriteLine();
            

            //select and piece from the puzzle
            int piece = puzzle.Pieces.Dequeue();

            count++;

            for (int index = 0; index < puzzle.BoardSize; index++)
            {
                var solvedPuzzle = DoStuff(puzzle, LineType.Column, index, piece);
                if (solvedPuzzle != null)
                    return solvedPuzzle;

                solvedPuzzle = DoStuff(puzzle, LineType.Row, index, piece);
                if (solvedPuzzle != null)
                    return solvedPuzzle;


                /*if ()
                {
                    var solvedPuzzle = DoRecursive(newPuzzle, rounds - 1);
                    if (solvedPuzzle != null)
                        return solvedPuzzle;
                }

                if (TryPlace(newPuzzle, newPuzzle.GetColumn(index).ToList(), newPuzzle.ColumnLimits[index], piece, offset))
                {
                    var solvedPuzzle = DoRecursive(newPuzzle, rounds - 1);
                    if (solvedPuzzle != null)
                        return solvedPuzzle;
                }


                //create a clone of the puzzle and attempt to place the piece horizontally
                newPuzzle = puzzle.Clone();

                if (TryPlace(newPuzzle, newPuzzle.GetRow(index).ToList(), newPuzzle.RowLimits[index], piece, offset))
                {
                    var solvedPuzzle = DoRecursive(newPuzzle, rounds - 1);
                    if (solvedPuzzle != null)
                        return solvedPuzzle;
                }*/
            }

            return null;
        }



        private static Puzzle DoStuff(Puzzle puzzle, LineType lineType, int index, int piece)
        {
            var limits = puzzle.GetLineLimits(index, lineType);

            //if the piece alone is bigger than the limits, stop right here
            if (piece > limits)
                return null;

            var pointers = puzzle.GetLine(index,lineType).ToList();
            
            //OR the number or already placed dots (hints are not counted)
            int placedDots = pointers.Count(x => x.Value == PuzzleCellState.Dot);
            if(placedDots + piece > limits)
                return null;

            return TryPlace(puzzle, lineType, index, piece, limits - placedDots);
        }

        public static List<Puzzle> puzzles = new List<Puzzle>();

        private static Puzzle TryPlace(Puzzle puzzle, LineType lineType, int index, int piece, int limits)
        {
            var pointers = puzzle.GetLine(index, lineType).ToList();

            //var coiso = GetOffsetIndices(puzzle, pointers, piece);

            foreach (var offset in GetOffsetIndices(puzzle, pointers, piece, limits))
            {
                //create a clone of the puzzle
                var newPuzzle = puzzle.Clone();

                pointers = newPuzzle.GetLine(index, lineType).ToList();

                //fill the parts of the piece, since the beginning
                for (int j = 0; j < piece; j++)
                    pointers[offset + j].Value = PuzzleCellState.Dot;

                for (int j = 0; j < piece; j++)
                {
                    foreach (PuzzleCellPointer puzzleCellPointer in pointers[offset + j].Around)
                    {
                        if (puzzleCellPointer.Value != PuzzleCellState.Dot)
                            puzzleCellPointer.Value = PuzzleCellState.Blocked;
                    }
                }

                var solution = DoRecursive(newPuzzle);
                if (solution != null)
                    return solution;
            }


            /*for (int offset = 0; offset < puzzle.BoardSize - piece; offset++)
            {

                //and attempt to place the piece
                int sequentialCount = 0;

                //create a clone of the puzzle
                var newPuzzle = puzzle.Clone();
                
                if (piece == 5)
                    puzzles.Add(newPuzzle);

                

                for (int i = offset; i < pointers.Count; i++)
                {
                    if (pointers[i].Value.HasFlag(PuzzleCellState.Empty)
                        || (pointers[i].Value.HasFlag(PuzzleCellState.Dot) && pointers[i].Value.HasFlag(PuzzleCellState.Hint)))
                    {
                        sequentialCount++;
                        if (sequentialCount == piece)
                        {
                            var baseOffset = i - sequentialCount + 1;

                            //fill the parts of the piece, since the beginning
                            for (int j = 0; j < piece; j++)
                                pointers[baseOffset + j].Value = PuzzleCellState.Dot;

                            for (int j = 0; j < piece; j++)
                            {
                                foreach (PuzzleCellPointer puzzleCellPointer in pointers[baseOffset + j].Around)
                                {
                                    if (puzzleCellPointer.Value != PuzzleCellState.Dot)
                                        puzzleCellPointer.Value = PuzzleCellState.Blocked;
                                }
                            }

                            var solution = DoRecursive(newPuzzle);
                            if (solution != null)
                                return solution;
                        }
                    }
                    else
                    {
                        sequentialCount = 0;
                    }
                }
            }*/


            return null;
        }


        private static IEnumerable<int> GetOffsetIndices(Puzzle puzzle, List<PuzzleCellPointer> pointers, int piece, int limit)
        {
            int totalHints = 0;

            int[] support = new int[puzzle.BoardSize];
            int[] hintCount = new int[puzzle.BoardSize];

            //this does not SOLVE IT!! We need to consider the hints
            int lastValid = -1;
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                if (pointers[i].Value.HasFlag(PuzzleCellState.Empty))
                {
                    for (int j = lastValid + 1; j <= i; j++)
                    {
                        support[j]++;
                    }
                }
                else if (pointers[i].Value.HasFlag(PuzzleCellState.Dot) && pointers[i].Value.HasFlag(PuzzleCellState.Hint))
                {
                    totalHints++;

                    for (int j = lastValid + 1; j <= i; j++)
                    {
                        support[j]++;
                        hintCount[j]++;
                    }

                    //if this is a hint, a piece cannot be placed in a location that ends before a new hint appears
                    int positionBeforeHint = i - piece;
                    if (positionBeforeHint >= 0)
                        support[positionBeforeHint] = -puzzle.BoardSize;
                }
                else
                {
                    lastValid = i;
                }
            }

            bool previousIsDotHint = false;
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                if (support[i] >= piece
                    && !previousIsDotHint
                    && (piece + totalHints - hintCount[i]) <= limit)
                    yield return i;

                //a piece cannot be placed in a location that starts after a new hint appears
                previousIsDotHint = (pointers[i].Value.HasFlag(PuzzleCellState.Dot) &&
                                  pointers[i].Value.HasFlag(PuzzleCellState.Hint));
            }
        }

        

        private static void BlockHintedAreas(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                for (int j = 0; j < puzzle.BoardSize; j++)
                {
                    //first, block cells hinted by the puzzle "directional" dots
                    if (puzzle[i, j].HasFlag(PuzzleCellState.Left))
                    {
                        var pointer = puzzle.GetPointerOrDefault(i + 1, j);
                        if (pointer != null)
                            pointer.Value = PuzzleCellState.Blocked;
                    }

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Right))
                    {
                        var pointer = puzzle.GetPointerOrDefault(i - 1, j);
                        if (pointer != null)
                            pointer.Value = PuzzleCellState.Blocked;
                    }

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Up))
                    {
                        var pointer = puzzle.GetPointerOrDefault(i, j + 1);
                        if (pointer != null)
                            pointer.Value = PuzzleCellState.Blocked;
                    }

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Down))
                    {
                        var pointer = puzzle.GetPointerOrDefault(i, j - 1);
                        if (pointer != null)
                            pointer.Value = PuzzleCellState.Blocked;
                    }
                }
            }
        }

        private static void BlockDiagonals(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                for (int j = 0; j < puzzle.BoardSize; j++)
                {
                    if (puzzle[i, j].HasFlag(PuzzleCellState.Dot))
                        foreach (PuzzleCellPointer puzzleCellPointer in puzzle.GetDiagonals(i, j))
                            puzzleCellPointer.Value = PuzzleCellState.Blocked;
                }
            }
        }


        private static void FillHints(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                for (int j = 0; j < puzzle.BoardSize; j++)
                {
                    if (puzzle[i, j].HasFlag(PuzzleCellState.Left))
                        puzzle[i - 1, j] = PuzzleCellState.Dot | PuzzleCellState.Hint;

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Right))
                        puzzle[i + 1, j] = PuzzleCellState.Dot | PuzzleCellState.Hint;

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Up))
                        puzzle[i, j - 1] = PuzzleCellState.Dot | PuzzleCellState.Hint;

                    if (puzzle[i, j].HasFlag(PuzzleCellState.Down))
                        puzzle[i, j + 1] = PuzzleCellState.Dot | PuzzleCellState.Hint;
                }
            }
        }

        private static void FillReachedLimits(Puzzle puzzle)
        {
            //iterate over all the columns and 
            for (int index = 0; index < puzzle.BoardSize; index++)
            {
                FillReachedLimitsOfLine(puzzle.GetRow(index).ToList(), puzzle.RowLimits[index]);
                FillReachedLimitsOfLine(puzzle.GetColumn(index).ToList(), puzzle.ColumnLimits[index]);
            }
        }

        private static void FillReachedLimitsOfLine(List<PuzzleCellPointer> valueList, int limit)
        {
            //count the current number of dots in that line or row
            var countDots = valueList.Select(x => x.Value).Count(x => x.HasFlag(PuzzleCellState.Dot));

            //if it equals the limit, block the empty spaces
            if (countDots == limit)
            {
                foreach (PuzzleCellPointer puzzleCellPointer in valueList.Where(x => x.Value.HasFlag(PuzzleCellState.Empty)))
                    puzzleCellPointer.Value = PuzzleCellState.Blocked;
            }
        }

        private static void FixDotConnections(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.BoardSize; i++)
            {
                FixLineDotConnections(puzzle.GetRow(i), PuzzleCellState.Right, PuzzleCellState.Left);
                FixLineDotConnections(puzzle.GetColumn(i).ToList(), PuzzleCellState.Down, PuzzleCellState.Up);
            }
        }

        private static void FixLineDotConnections(IEnumerable<PuzzleCellPointer> enumerable, PuzzleCellState previous, PuzzleCellState next)
        {
            PuzzleCellPointer lastPointer = null;

            foreach (PuzzleCellPointer currentPointer in enumerable)
            {
                if (lastPointer != null)
                {
                    if (lastPointer.Value.HasFlag(PuzzleCellState.Dot) && currentPointer.Value.HasFlag(PuzzleCellState.Dot))
                    {
                        lastPointer.Value = lastPointer.Value | previous;
                        currentPointer.Value = currentPointer.Value | next;
                    }
                }

                lastPointer = currentPointer;
            }
        }
    }
}
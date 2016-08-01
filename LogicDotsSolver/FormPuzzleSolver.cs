using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogicDotsSolver.Solving;

namespace LogicDotsSolver
{
    public partial class FormPuzzleSolver : Form
    {
        private Puzzle _initialPuzzle;
        private int _currentRoundLimit = 1;

        public FormPuzzleSolver()
        {
            InitializeComponent();

            _initialPuzzle = new Puzzle("Puzzles/Puzzle11 - 2.txt");

            gridCanvas.Puzzle = _initialPuzzle;

            RequestSolve();

            KeyDown += GridCanvasOnKeyDown;
        }

        private void RequestSolve(int limit = 1)
        {
            var resultPuzzle = PuzzleSolver.Solve(_initialPuzzle, limit);
            if (resultPuzzle != null)
                gridCanvas.Puzzle = resultPuzzle;
            else
                MessageBox.Show("Could not be solved");
        }

        private int location = 0;

        private void GridCanvasOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Right)
            {
                LoadHistoryPuzzle(++location);
                //RequestSolve(++_currentRoundLimit);
                //gridCanvas.Puzzle = PuzzleSolver.Solve(_initialPuzzle, ++_currentRoundLimit);
            }
            else if (keyEventArgs.KeyCode == Keys.Left)
            {
                LoadHistoryPuzzle(--location);
                //RequestSolve(--_currentRoundLimit);
                //gridCanvas.Puzzle = PuzzleSolver.Solve(_initialPuzzle, --_currentRoundLimit);
            }
        }

        private void LoadHistoryPuzzle(int index)
        {
            if (PuzzleSolver.puzzles.Count > 0)
            {
                var nindex = GetNormalizedIndex(index);
                label1.Text = nindex.ToString() + " / " + PuzzleSolver.puzzles.Count;
                gridCanvas.Puzzle = PuzzleSolver.puzzles[nindex];

            }
        }

        public int GetNormalizedIndex(int index)
        {
            if (index < 0)
                return PuzzleSolver.puzzles.Count + index % PuzzleSolver.puzzles.Count;
            if (index < PuzzleSolver.puzzles.Count)              //most cases will probaly fit this case anyway, requiring only 2 simple conditions to reach it
                return index;

            return index % PuzzleSolver.puzzles.Count;
            //this expression would be simpler, but requires the expensive modulo operation every time
            //return index < 0 ? Count + index % Count : index % Count;
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string puzzleSizeInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Puzzle Size (e.g. 10):", "Create New Puzzle", "10");

            if (!String.IsNullOrWhiteSpace(puzzleSizeInput))
            {
                int value = 0;
                if (!Int32.TryParse(puzzleSizeInput, out value))
                    throw new ArgumentException("Input must be a value.");

                if (value < 1)
                    throw new ArgumentException("Input must be a value greater or equal than 1.");

                //create the puzzle and set it to the grid
                gridCanvas.Puzzle = new Puzzle(value);

                //try to request the limits of columns
                if (RequestColumnLimits())
                {
                    //if no error occurred, proceed to ask for rows
                    if (RequestRowLimits())
                    {
                        RequestPieces();
                    }
                }
            }
        }



        private void setColumnValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestColumnLimits();
        }



        private bool RequestColumnLimits()
        {
            string puzzleLimitsInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Column Limits (e.g. 2,3,5):", "Set Column Limits", "");
            
            try
            {
                int[] limitValues = puzzleLimitsInput.Split(',').Select(x => Int32.Parse(x.Trim())).ToArray();
                gridCanvas.Puzzle.ColumnLimits = limitValues;
                gridCanvas.Refresh();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }



        private void setRowValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestRowLimits();
        }



        private bool RequestRowLimits()
        {
            string puzzleLimitsInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Row Limits (e.g. 2,3,5):", "Set Row Limits", "");
            
            try
            {
                int[] limitValues = puzzleLimitsInput.Split(',').Select(x => Int32.Parse(x.Trim())).ToArray();
                gridCanvas.Puzzle.RowLimits = limitValues;
                gridCanvas.Refresh();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
        


        private void RequestPieces()
        {
            string puzzlePiecesInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Piece Limits (e.g. 2,3,5):", "Set Row Limits", "");

            try
            {
                var pieces = new Queue<int>(puzzlePiecesInput.Split(',').Select(x => Int32.Parse(x.Trim())));
                gridCanvas.Puzzle.Pieces = pieces;
                gridCanvas.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

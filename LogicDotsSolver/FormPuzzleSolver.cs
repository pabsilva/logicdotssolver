using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogicDotsSolver.Solving;

namespace LogicDotsSolver
{
    public partial class FormPuzzleSolver : Form
    {
        private readonly List<Puzzle> _puzzles = new List<Puzzle>();
        private int _chosenPuzzleIndex;



        public FormPuzzleSolver()
        {
            InitializeComponent();

            foreach (string puzzleFile in Directory.GetFiles("Puzzles"))
                _puzzles.Add(new Puzzle(puzzleFile));
            
            if (_puzzles.Count > 0)
                RequestSolve(_puzzles[_chosenPuzzleIndex]);

            KeyDown += GridCanvasOnKeyDown;
        }



        private void RequestSolve(Puzzle puzzle)
        {
            labelPuzzleName.Text = puzzle.Path;

            var resultPuzzle = PuzzleSolver.Solve(puzzle);
            if (resultPuzzle != null)
                gridCanvas.Puzzle = resultPuzzle;
            else
            {
                MessageBox.Show("Could not be solved");
                gridCanvas.Puzzle = puzzle;
            }
        }



        private void GridCanvasOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (_puzzles.Count == 0)
                return;

            if (keyEventArgs.KeyCode == Keys.Right)
            {
                RequestSolve(_puzzles[GetNormalizedIndex(++_chosenPuzzleIndex)]);
            }
            else if (keyEventArgs.KeyCode == Keys.Left)
            {
                RequestSolve(_puzzles[GetNormalizedIndex(--_chosenPuzzleIndex)]);
            }
        }



        public int GetNormalizedIndex(int index)
        {
            if (index < 0)
                return _puzzles.Count + index%_puzzles.Count;

            //most cases will probaly fit this case anyway, requiring only 2 simple conditions to reach it
            if (index < _puzzles.Count)
                return index;

            //this expression would be simpler, but requires the expensive modulo operation every time
            return index%_puzzles.Count;
        }
    }
}
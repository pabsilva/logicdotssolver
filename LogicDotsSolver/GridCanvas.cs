using System.Drawing;
using System.Windows.Forms;
using LogicDotsSolver.Solving;

namespace LogicDotsSolver
{
    public class GridCanvas : PictureBox
    {
        private const int CellSize = 30;
        private const int DotSize = 15;
        private const int PenSize = 4;

        private Puzzle _puzzle = new Puzzle(1);



        public GridCanvas()
        {
            ResizeRedraw = true;

            Paint += OnPaint;
        }



        public Puzzle Puzzle
        {
            get { return _puzzle; }
            set
            {
                _puzzle = value;
                Refresh();
            }
        }



        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
        }



        private void DrawGrid(Graphics graphics)
        {
            graphics.Clear(Color.White);

            //how much space we're taking with the cells
            var totalCellSize = _puzzle.BoardSize*CellSize;

            //how much space we're taking with the borders
            var gridSize = totalCellSize + (_puzzle.BoardSize + 1)*PenSize;

            //determine where we are starting our grid
            var startingX = Size.Width/2 - gridSize/2;
            var startingY = Size.Height/2 - gridSize/2;

            DrawGrid(graphics, startingX, startingY, gridSize);

            DrawTextLimits(graphics, startingX, startingY);

            DrawPuzzleContent(graphics, startingX, startingY);
        }



        private void DrawGrid(Graphics graphics, int startingX, int startingY, int gridSize)
        {
            Pen pen = new Pen(Color.Black, PenSize);

            for (int index = 0; index < _puzzle.BoardSize + 1; index++)
            {
                //the pen starts drawing from the center
                int offset = index*(CellSize + PenSize) + PenSize/2;

                //draw the horizontal and vertical lines
                graphics.DrawLine(pen, startingX + offset, startingY, startingX + offset, startingY + gridSize);
                graphics.DrawLine(pen, startingX, startingY + offset, startingX + gridSize, startingY + offset);
            }
        }



        private void DrawPuzzleContent(Graphics graphics, int startingX, int startingY)
        {
            Brush brushCircle = new SolidBrush(Color.Red);
            Brush brushCirclePale = new SolidBrush(Color.PaleVioletRed);
            for (int x = 0; x < _puzzle.BoardSize; x++)
            {
                int xOffset = PenSize + x*(CellSize + PenSize);
                int xOffsetDotCenter = xOffset + CellSize/2 - DotSize/2;

                for (int y = 0; y < _puzzle.BoardSize; y++)
                {
                    int yOffset = PenSize + y*(CellSize + PenSize);
                    int yOffsetDotCenter = yOffset + CellSize/2 - DotSize/2;

                    //get the content of the cell
                    //depending on the content, draw different shapes or using different shades
                    var cellState = _puzzle.CellStates[x, y];

                    if (cellState.HasFlag(PuzzleCellState.Right))
                        graphics.FillRectangle(brushCirclePale, new Rectangle(startingX + xOffset + CellSize/2, startingY + yOffsetDotCenter, CellSize/2, DotSize));

                    if (cellState.HasFlag(PuzzleCellState.Left))
                        graphics.FillRectangle(brushCirclePale, new Rectangle(startingX + xOffset, startingY + yOffsetDotCenter, CellSize/2, DotSize));

                    if (cellState.HasFlag(PuzzleCellState.Up))
                        graphics.FillRectangle(brushCirclePale, new Rectangle(startingX + xOffsetDotCenter, startingY + yOffset, DotSize, CellSize/2));

                    if (cellState.HasFlag(PuzzleCellState.Down))
                        graphics.FillRectangle(brushCirclePale, new Rectangle(startingX + xOffsetDotCenter, startingY + yOffset + CellSize/2, DotSize, CellSize/2));

                    if (cellState.HasFlag(PuzzleCellState.Dot))
                        graphics.FillEllipse(brushCircle, new Rectangle(startingX + xOffsetDotCenter, startingY + yOffsetDotCenter, DotSize, DotSize));

                    if (cellState.HasFlag(PuzzleCellState.Blocked))
                        graphics.FillRectangle(brushCircle, new Rectangle(startingX + xOffset, startingY + yOffset, CellSize, CellSize));
                }
            }
        }



        private void DrawTextLimits(Graphics graphics, int startingX, int startingY)
        {
            Font myFont = new Font("Arial Black", 15);
            Brush brushText = new SolidBrush(Color.Black);

            for (int i = 0; i < _puzzle.BoardSize; i++)
            {
                int offset = PenSize + i*(CellSize + PenSize);

                graphics.DrawString(_puzzle.ColumnLimits[i].ToString(), myFont, brushText, startingX + offset + CellSize/4, startingY - CellSize);
                graphics.DrawString(_puzzle.RowLimits[i].ToString(), myFont, brushText, startingX - CellSize, startingY + offset);
            }
        }
    }
}
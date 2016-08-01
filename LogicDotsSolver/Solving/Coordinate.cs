namespace LogicDotsSolver.Solving
{
    public struct Coordinate
    {
        private readonly int _x;
        private readonly int _y;

        public Coordinate(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static Coordinate operator +(Coordinate left, Coordinate right)
        {
            return new Coordinate(left.X + right.X, left.Y + right.Y);
        }

        public static Coordinate operator -(Coordinate left, Coordinate right)
        {
            return new Coordinate(left.X - right.X, left.Y - right.Y);
        }

        public static Coordinate operator *(Coordinate left, Coordinate right)
        {
            return new Coordinate(left.X * right.X, left.Y * right.Y);
        }

        public static Coordinate operator /(Coordinate left, Coordinate right)
        {
            return new Coordinate(left.X / right.X, left.Y / right.Y);
        }


        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }
    }
}

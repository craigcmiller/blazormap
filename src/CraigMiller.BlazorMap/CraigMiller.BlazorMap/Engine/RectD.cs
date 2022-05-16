namespace CraigMiller.BlazorMap.Engine
{
    public readonly struct RectD
    {
        public readonly double X, Y, Width, Height;

        /// <summary>
        /// Creates a rect that contains all points in <paramref name="points"/>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static RectD BoundingPoints(PointD[] points)
        {
            double top = points[0].Y, bottom = top, left = points[0].X, right = left;
            for (int i = 1; i < points.Length; i++)
            {
                PointD pt = points[i];
                if (pt.X < left)
                {
                    left = pt.X;
                }
                if (pt.X > right)
                {
                    right = pt.X;
                }
                if (pt.Y > top)
                {
                    top = pt.Y;
                }
                if (pt.Y < bottom)
                {
                    bottom = pt.Y;
                }
            }

            return new RectD(top, left, right - left, bottom - top);
        }

        public RectD(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public double Right => X + Width;

        public double Bottom => Y + Height;

        /// <summary>
        /// Gets if this rect intersects with <paramref name="rect"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool IntersectsWith(RectD rect) => rect.X < Right && X < rect.Right && rect.Y < Bottom && Y < rect.Bottom;
    }
}

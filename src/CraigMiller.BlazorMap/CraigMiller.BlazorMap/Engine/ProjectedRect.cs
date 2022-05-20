namespace CraigMiller.BlazorMap.Engine
{
    /// <summary>
    /// A rectangle in projected coordinate system
    /// </summary>
    public readonly struct ProjectedRect
    {
        public readonly double Left, Bottom, Width, Height;

        /// <summary>
        /// Creates a rect that encompasses all points in <paramref name="points"/>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static ProjectedRect BoundingPoints(PointD[] points)
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

            return new ProjectedRect(left, bottom, right - left, top - bottom);
        }

        public ProjectedRect(double left, double bottom, double width, double height)
        {
            Left = left;
            Bottom = bottom;
            Width = width;
            Height = height;
        }

        public double Right => Left + Width;

        public double Top => Bottom + Height;

        /// <summary>
        /// Gets if this rect intersects with <paramref name="rect"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool IntersectsWith(ProjectedRect rect) => rect.Left < Right && rect.Right > Left && rect.Bottom < Top && rect.Top > Bottom;

        public override string ToString() => $"[{Left}, {Bottom}, {Width}, {Height}]";
    }
}

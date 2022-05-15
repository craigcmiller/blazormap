namespace CraigMiller.BlazorMap.Engine
{
    public readonly struct RectD
    {
        public readonly double X, Y, Width, Height;

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

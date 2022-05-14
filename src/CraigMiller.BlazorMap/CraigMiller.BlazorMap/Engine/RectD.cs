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
    }
}

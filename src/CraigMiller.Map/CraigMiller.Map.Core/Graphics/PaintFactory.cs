using SkiaSharp;

namespace CraigMiller.Map.Core.Graphics
{
    public static class PaintFactory
    {
        public static SKPaint CreateStrokePaint(SKColor color, float strokeWidth)
        {
            return new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                IsAntialias = true
            };
        }

        public static SKPaint CreateFillPaint(SKColor color)
        {
            return new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
        }
    }
}

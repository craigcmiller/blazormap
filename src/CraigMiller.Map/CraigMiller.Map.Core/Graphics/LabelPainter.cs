using SkiaSharp;

namespace CraigMiller.Map.Core.Graphics
{
    public sealed class LabelPainter : IDisposable
    {
        readonly SKPaint _foreground, _background;

        public static LabelPainter OutlineLabelPainter(SKColor foregroundColor, SKColor backgroundColor, float textSize, bool isBold = false)
        {
            SKPaint foreground = PaintFactory.CreateStrokePaint(foregroundColor, 1f);
            SKPaint background = PaintFactory.CreateFillPaint(backgroundColor);

            foreground.TextSize = background.TextSize = textSize;
            foreground.FakeBoldText = background.FakeBoldText = isBold;

            return new LabelPainter(foreground, background);
        }

        public LabelPainter(SKPaint foreground, SKPaint background)
        {
            _foreground = foreground;
            _background = background;
        }

        public void Dispose()
        {
            _foreground.Dispose();
            _background.Dispose();
        }

        public void PaintLabel(SKCanvas canvas, float x, float y, string label)
        {
            canvas.DrawText(label, x, y, _background);
            canvas.DrawText(label, x, y, _foreground);
        }
    }
}

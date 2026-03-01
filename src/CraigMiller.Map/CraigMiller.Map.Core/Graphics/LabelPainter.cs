using SkiaSharp;

namespace CraigMiller.Map.Core.Graphics
{
    public sealed class LabelPainter : IDisposable
    {
        readonly SKPaint _foreground, _background;

        readonly SKFont _font = new SKFont(SKTypeface.Default, 12f)
        {
            Embolden = true,
        };

        public static LabelPainter OutlineLabelPainter(SKColor foregroundColor, SKColor backgroundColor, float textSize, bool isBold = false)
        {
            SKPaint foreground = PaintFactory.CreateStrokePaint(foregroundColor, 1f);
            SKPaint background = PaintFactory.CreateFillPaint(backgroundColor);

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
            _font.Dispose();
        }

        public void PaintLabel(SKCanvas canvas, float x, float y, string label)
        {
            canvas.DrawText(label, x, y, _font, _background);
            canvas.DrawText(label, x, y, _font, _foreground);
        }
    }
}

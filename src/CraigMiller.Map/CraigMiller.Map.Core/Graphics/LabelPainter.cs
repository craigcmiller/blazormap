using SkiaSharp;

namespace CraigMiller.Map.Core.Graphics
{
    public sealed class LabelPainter : IDisposable
    {
        readonly SKPaint _foreground, _background;
        readonly SKFont _font;

        public static LabelPainter OutlineLabelPainter(SKColor foregroundColor, SKColor backgroundColor, float textSize, bool isBold = false)
        {
            SKPaint foreground = PaintFactory.CreateFillPaint(foregroundColor);
            SKPaint background = PaintFactory.CreateFillPaint(backgroundColor);

            return new LabelPainter(foreground, background, textSize, isBold);
        }

        public LabelPainter(SKPaint foreground, SKPaint background, float textSize, bool isBold)
        {
            _foreground = foreground;
            _background = background;

            _font = new SKFont(SKTypeface.Default, 12f)
            {
                Embolden = true,
                Edging = SKFontEdging.Antialias,
            };
        }

        public void Dispose()
        {
            _foreground.Dispose();
            _background.Dispose();
            _font.Dispose();
        }

        public void PaintLabel(SKCanvas canvas, float x, float y, string label)
        {
            canvas.DrawText(label, x - 1f, y - 1f, _font, _background);
            canvas.DrawText(label, x, y, _font, _foreground);
        }
    }
}

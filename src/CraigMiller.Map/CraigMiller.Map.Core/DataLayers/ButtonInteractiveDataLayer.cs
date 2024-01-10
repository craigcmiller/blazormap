using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class ButtonInteractiveDataLayer : IDataLayer, IInteractiveLayer
{
    readonly SKPaint _textPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.StrokeAndFill,
        Color = new SKColor(20, 20, 20),
        TextSize = 14f,
        FakeBoldText = true
    };
    SKPaint? _backgroundPaint;
    SKRect? _rect;

    public event EventHandler? Clicked;

    public ButtonInteractiveDataLayer()
    {
    }

    public float X { get; set; }

    public float Y { get; set; }

    public string? Text { get; set; }

    public float MinWidth { get; set; }

    public float HorizontalPadding { get; set; } = 8f;

    public byte Alpha { get; set; } = 255;

    public float CornerRadius = 3f;

    public void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter)
    {
        if (Text is not null)
        {
            _backgroundPaint ??= new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.WhiteSmoke.WithAlpha(Alpha)
            };

            float textWidth = _textPaint.MeasureText(Text);

            float buttonWidth = textWidth + HorizontalPadding + HorizontalPadding;
            if (buttonWidth < MinWidth)
            {
                buttonWidth = MinWidth;
            }

            _rect = SKRect.Create(X, Y, buttonWidth, _textPaint.TextSize + 14f);

            canvas.DrawRoundRect(_rect.Value, CornerRadius, CornerRadius, _backgroundPaint);
            canvas.DrawText(Text, X + HorizontalPadding, Y + _textPaint.TextSize + 6f, _textPaint);
        }
    }

    bool PositionInside(double x, double y) => _rect.HasValue && _rect.Value.Contains((float)x, (float)y);

    public bool MouseClicked(CanvasRenderer renderer, double canvasX, double canvasY)
    {
        if (PositionInside(canvasX, canvasY))
        {
            Clicked?.Invoke(this, EventArgs.Empty);

            return true;
        }

        return false;
    }

    public bool PrimaryMouseDown(CanvasRenderer renderer, double canvasX, double canvasY) => PositionInside(canvasX, canvasY);

    public bool PrimaryMouseUp(CanvasRenderer renderer, double canvasX, double canvasY) => PositionInside(canvasX, canvasY);

    public bool MouseMoved(CanvasRenderer renderer, double canvasX, double canvasY) => PositionInside(canvasX, canvasY);
}

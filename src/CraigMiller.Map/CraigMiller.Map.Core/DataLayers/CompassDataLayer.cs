using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class CompassDataLayer : IDataLayer, IDisposable
{
    readonly SKPathEffect _arrowPathEffect = SKPathEffect.CreateDash([2f, 2f], 0f);

    readonly SKPaint _arrowPaint;

    readonly SKPaint _textPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.StrokeAndFill,
        Color = SKColors.LightGray,
    };

    readonly SKFont _font = new SKFont(SKTypeface.Default, 13f)
    {
        Embolden = true,
    };

    readonly SKPaint _circleFillPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.StrokeAndFill,
        Color = new SKColor(50, 50, 50, 220),
    };

    readonly SKPaint _circleOutlinePaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = new SKColor(55, 55, 75, 220),
        StrokeWidth = 1f,
    };

    public CompassDataLayer()
    {
        _arrowPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            StrokeCap = SKStrokeCap.Round,
            Color = SKColors.LightGray,
            PathEffect = _arrowPathEffect,
        };
    }

    public void Dispose()
    {
        _arrowPaint.Dispose();
        _textPaint.Dispose();
        _font.Dispose();
        _circleFillPaint.Dispose();
        _circleOutlinePaint.Dispose();
        _arrowPathEffect.Dispose();
    }

    public void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter)
    {
        canvas.Save();

        const float arrowLength = 36f;
        const float arrowHeadLength = 3.5f;
        const float halfArrowLength = arrowLength / 2f; ;

        canvas.Translate((float)canvasWidth - arrowLength - 12f, arrowLength + 12f);
        canvas.RotateRadians(-converter.RotationRadians);

        canvas.DrawCircle(0f, 0f, halfArrowLength + 15f, _circleFillPaint);
        canvas.DrawCircle(0f, 0f, halfArrowLength + 15f, _circleOutlinePaint);

        canvas.DrawText("N", 0f, -halfArrowLength - 3f, SKTextAlign.Center, _font, _textPaint);
        canvas.DrawText("S", 0f, halfArrowLength + 12f, SKTextAlign.Center, _font, _textPaint);
        canvas.DrawText("E", halfArrowLength + 6f, 4f, SKTextAlign.Center, _font, _textPaint);
        canvas.DrawText("W", -halfArrowLength - 6f, 4f, SKTextAlign.Center, _font, _textPaint);

        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(0f, halfArrowLength), _arrowPaint);
        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(-arrowHeadLength, -halfArrowLength + arrowHeadLength), _arrowPaint);
        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(arrowHeadLength, -halfArrowLength + arrowHeadLength), _arrowPaint);

        canvas.DrawLine(new SKPoint(-halfArrowLength, 0f), new SKPoint(halfArrowLength, 0f), _arrowPaint);

        canvas.Restore();
    }
}

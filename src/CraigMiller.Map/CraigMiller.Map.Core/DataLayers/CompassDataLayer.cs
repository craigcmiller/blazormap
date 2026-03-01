using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class CompassDataLayer : IDataLayer
{
    readonly SKPaint _outlinePaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        StrokeCap = SKStrokeCap.Round,
        Color = SKColors.DarkGray,
    };

    readonly SKPaint _textPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.StrokeAndFill,
        TextSize = 12f,
        Color = SKColors.DarkBlue,
        FakeBoldText = true,
        TextAlign = SKTextAlign.Center,
    };

    readonly SKPaint _circleFillPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.StrokeAndFill,
        Color = SKColors.LightGray.WithAlpha(128),
    };

    readonly SKPaint _circleOutlinePaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = SKColors.DarkGray.WithAlpha(230),
        StrokeWidth = 1f,
    };

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

        canvas.DrawText("N", 0f, -halfArrowLength - 3f, _textPaint);
        canvas.DrawText("S", 0f, halfArrowLength + 12f, _textPaint);
        canvas.DrawText("E", halfArrowLength + 6f, 4f, _textPaint);
        canvas.DrawText("W", -halfArrowLength - 6f, 4f, _textPaint);

        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(0f, halfArrowLength), _outlinePaint);
        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(-arrowHeadLength, -halfArrowLength + arrowHeadLength), _outlinePaint);
        canvas.DrawLine(new SKPoint(0f, -halfArrowLength), new SKPoint(arrowHeadLength, -halfArrowLength + arrowHeadLength), _outlinePaint);

        canvas.DrawLine(new SKPoint(-halfArrowLength, 0f), new SKPoint(halfArrowLength, 0f), _outlinePaint);

        canvas.Restore();
    }
}

using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Units;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class ScaleDataLayer : IDataLayer, IDisposable
{
    readonly SKPaint _backgroundPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = SKColors.White,
        StrokeWidth = 5f,
        StrokeCap = SKStrokeCap.Round,
    };

    readonly SKPaint _foregroundPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = SKColors.DarkGray,
        StrokeWidth = 3f,
        StrokeCap = SKStrokeCap.Round,
    };

    readonly SKPaint _textPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        Color = SKColors.Black,
    };

    readonly SKFont _font = new SKFont(SKTypeface.Default, 14f)
    {
        Embolden = true,
    };

    public void Dispose()
    {
        _backgroundPaint.Dispose();
        _foregroundPaint.Dispose();
        _textPaint.Dispose();
        _font.Dispose();
    }

    public void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter)
    {
        float scaleY = (float)canvasHeight - 20f;
        float scaleXEnd = (float)canvasWidth - 20f;
        float scaleXStart = scaleXEnd - ScaleWidth;

        converter.CanvasToLatLon(scaleXStart, scaleY, out double startLat, out double startLon);
        converter.CanvasToLatLon(scaleXEnd, scaleY, out double endLat, out double endLon);

        Distance scaleDist = Distance.Between(startLat, startLon, endLat, endLon);

        double distVal = scaleDist.GetValue(Units);
        int distInt = (int)distVal;

        float renderLen = ScaleWidth;
        if (distInt > 0)
        {
            renderLen = ScaleWidth * (distInt / (float)distVal);
        }

        scaleXStart = scaleXEnd - renderLen;

        var points = new[]
        {
            new SKPoint(scaleXStart, scaleY - 5),
            new SKPoint(scaleXStart, scaleY),
            new SKPoint(scaleXStart, scaleY),
            new SKPoint(scaleXEnd, scaleY),
            new SKPoint(scaleXEnd, scaleY),
            new SKPoint(scaleXEnd, scaleY - 10)
        };

        canvas.DrawPoints(SKPointMode.Lines, points, _backgroundPaint);
        canvas.DrawPoints(SKPointMode.Lines, points, _foregroundPaint);

        string valText = distInt == 0 ? distVal.ToString("0.00") : distInt.ToString();

        string text = $"{valText} {Units.ShortSuffix()}";

        _textPaint.Color = SKColors.White;
        canvas.DrawText(text, (scaleXEnd - scaleXStart) / 2f + scaleXStart, scaleY - 6f, SKTextAlign.Center, _font, _textPaint);
        _textPaint.Color = SKColors.Black;
        canvas.DrawText(text, (scaleXEnd - scaleXStart) / 2f + scaleXStart + 1, scaleY - 5f, SKTextAlign.Center, _font, _textPaint);
    }

    public float ScaleWidth { get; set; } = 150f;

    public DistanceUnits Units { get; set; } = DistanceUnits.NauticalMiles;
}

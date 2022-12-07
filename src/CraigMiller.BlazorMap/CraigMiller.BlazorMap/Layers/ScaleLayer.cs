using CraigMiller.BlazorMap.Engine;
using CraigMiller.BlazorMap.Units;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers;

public class ScaleLayer : ILayer
{
    readonly SKPaint _backgroundPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = SKColors.White,
        StrokeWidth = 5f
    };

    readonly SKPaint _foregroundPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        Color = SKColors.DarkGray,
        StrokeWidth = 3f,
        TextAlign = SKTextAlign.Center,
        TextSize = 12f
    };

    readonly SKPaint _textPaint = new SKPaint
    {
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        Color = SKColors.Black,
        TextAlign = SKTextAlign.Center,
        TextSize = 14f,
        FakeBoldText = true
    };

    public void DrawLayer(SKCanvas canvas, GeoConverter converter)
    {
        float scaleY = (float)converter.CanvasHeight - 20f;
        float scaleXEnd = (float)converter.CanvasWidth - 20f;
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

        canvas.DrawText($"{valText} {Units.ShortSuffix()}", (scaleXEnd - scaleXStart) / 2f + scaleXStart, scaleY - 6f, _textPaint);
    }

    public float ScaleWidth { get; set; } = 150f;

    public DistanceUnits Units { get; set; } = DistanceUnits.NauticalMiles;
}

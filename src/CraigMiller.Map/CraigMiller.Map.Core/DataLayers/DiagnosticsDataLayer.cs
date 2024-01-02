using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;
using SkiaSharp;

namespace CraigMiller.Map.Core.DataLayers;

public class DiagnosticsDataLayer : IDataLayer
{
    readonly SKPaint _textPaint = new SKPaint
    {
        Style = SKPaintStyle.Fill,
        IsAntialias = true,
        Color = SKColors.Black
    };

    public void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter)
    {
        converter.CanvasToLatLon(converter.CanvasWidth / 2.0, converter.CanvasHeight / 2.0, out double centerLat, out double centerLon);
        canvas.DrawText(new Location(centerLat, centerLon).ToDegreesMinutesSecondsString(), new SKPoint(20, 20), _textPaint);
    }
}

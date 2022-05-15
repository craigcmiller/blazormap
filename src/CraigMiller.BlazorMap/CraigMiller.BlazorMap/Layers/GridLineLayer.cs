using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class GridLineLayer : ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            using SKPathEffect pathEffect = SKPathEffect.CreateDash(new[] { 4f, 2f }, 0);
            using var gridPaint = new SKPaint
            {
                Color = SKColors.DarkGray,
                StrokeWidth = 2f,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = pathEffect
            };

            // Draw latitude grid lines
            for (double lat = -80; lat <= 80; lat += 10)
            {
                converter.LatLonToCanvas(lat, 0, out _, out float cnvY);

                canvas.DrawLine(0, cnvY, (float)converter.CanvasWidth, cnvY, gridPaint);
            }

            // Draw longitude grid lines
            for (double lon = -180; lon <= 180; lon += 10)
            {
                converter.LatLonToCanvas(0, lon, out float cnvX, out _);

                canvas.DrawLine(cnvX, 0, cnvX, (float)converter.CanvasHeight, gridPaint);
            }
        }
    }
}

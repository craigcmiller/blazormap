using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class DiagnosticsLayer : ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            converter.CanvasToProjected(0, 0, out double leftPrj, out double topPrj);
            converter.Projection.ToLatLon(leftPrj, topPrj, out double topLat, out double leftLon);
            canvas.DrawText($"{topLat:0.00} {leftLon:0.00}", new SKPoint(20, 20), new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = SKColors.Black
            });
        }
    }
}

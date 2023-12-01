using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Routes;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class RouteLayer : ILayer
    {
        readonly SKPaint _routeLineBackgroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White.WithAlpha(120),
            StrokeWidth = 9f
        };
        readonly SKPaint _routeLineForegroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Magenta.WithAlpha(160),
            StrokeWidth = 5f
        };

        readonly SKPaint _waypointBackgroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.White.WithAlpha(120)
        };
        readonly SKPaint _waypointForegroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.Blue.WithAlpha(170)
        };

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            if (Route.WaypointCount > 0)
            {
                SKPoint[] canvasPoints = Route.Select(wp => converter.LatLonToCanvas(wp.Latitude, wp.Longitude)).ToArray();

                var lines = new SKPoint[canvasPoints.Length * 2 - 2];
                lines[0] = canvasPoints[0];
                for (int i = 1, lineIndex = 1; i < canvasPoints.Length - 1; i++)
                {
                    lines[lineIndex++] = canvasPoints[i];
                    lines[lineIndex++] = canvasPoints[i];
                }
                lines[lines.Length - 1] = canvasPoints[canvasPoints.Length - 1];

                canvas.DrawPoints(SKPointMode.Lines, lines, _routeLineBackgroundPaint);
                canvas.DrawPoints(SKPointMode.Lines, lines, _routeLineForegroundPaint);

                foreach (SKPoint canvasPoint in canvasPoints)
                {
                    canvas.DrawCircle(canvasPoint, 8f, _waypointBackgroundPaint);
                    canvas.DrawCircle(canvasPoint, 6f, _waypointForegroundPaint);
                }
            }
        }

        public Route Route { get; set; } = new Route();
    }
}

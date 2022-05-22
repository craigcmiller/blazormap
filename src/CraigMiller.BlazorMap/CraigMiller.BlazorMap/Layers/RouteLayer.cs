using CraigMiller.BlazorMap.Engine;
using CraigMiller.BlazorMap.Routes;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class RouteLayer : ILayer
    {
        readonly SKPaint _routeLineForegroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Magenta.WithAlpha(180)
        };

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            if (Route.WaypointCount > 0)
            {
                SKPoint[] canvasPoints = Route.Select(wp => converter.LatLonToCanvas(wp.Latitude, wp.Longitude)).ToArray();

                canvas.DrawPoints(SKPointMode.Lines, canvasPoints, _routeLineForegroundPaint);
            }
        }

        public Route Route { get; set; } = new Route();
    }
}

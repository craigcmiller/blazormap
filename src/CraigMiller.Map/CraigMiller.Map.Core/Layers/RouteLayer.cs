using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Routes;
using CraigMiller.Map.Core.Units;
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
            StrokeWidth = 7f
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
            Color = SKColors.DarkGray.WithAlpha(180)
        };
        readonly SKPaint _waypointForegroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.WhiteSmoke.WithAlpha(190)
        };

        readonly SKPaint _textPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.DarkRed,
            TextSize = 12f,
            FakeBoldText = true
        };
        readonly SKPaint _textBackgroundPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.White.WithAlpha(150)
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

                SKPoint prevCanvas = canvasPoints[0];
                Waypoint prevWaypoint = Route[0];
                for (int i = 1; i < canvasPoints.Length; i++)
                {
                    SKPoint curCanvas = canvasPoints[i];
                    Waypoint curWaypoint = Route[i];

                    Distance dist = Distance.Between(prevWaypoint.Location, curWaypoint.Location);

                    double xDiff = curCanvas.X - prevCanvas.X, yDiff = curCanvas.Y - prevCanvas.Y;
                    double pixDist = Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

                    double rads = Math.Atan2(yDiff, xDiff);
                    double degs = rads * 180.0 / Math.PI;
                    //degs = 180.0 - degs;
                    if (degs < 0.0)
                    {
                        //degs += 90.0;
                    }
                    else
                    {
                        
                    }

                    string infoText = $"{dist.NatuticalMiles:0.0}nm {degs:000}° {180.0 - degs:000}° {degs - 180.0:000} {180.0 - degs + 270:000}°";

                    float textWidth = _textPaint.MeasureText(infoText);
                    if (textWidth + 20f < pixDist)
                    {
                        canvas.Save();

                        canvas.Translate(prevCanvas.X, prevCanvas.Y);
                        canvas.RotateRadians((float)rads);

                        canvas.DrawRoundRect(6f, 5f, textWidth + 6f, 14f, 4f, 4f, _textBackgroundPaint);
                        canvas.DrawText(infoText, 10f, 16f, _textPaint);

                        canvas.Restore();
                    }
                    //Console.WriteLine($"{i}. {prevCanvas} to {curCanvas} : {degs:000} {pixDist:0} ({dist.NatuticalMiles:0.0}nm)");

                    prevCanvas = curCanvas;
                    prevWaypoint = curWaypoint;
                }

                foreach (SKPoint canvasPoint in canvasPoints)
                {
                    canvas.DrawCircle(canvasPoint, 7f, _waypointBackgroundPaint);
                    canvas.DrawCircle(canvasPoint, 5.5f, _waypointForegroundPaint);
                }
            }
        }

        public Route Route { get; set; } = new Route();
    }
}

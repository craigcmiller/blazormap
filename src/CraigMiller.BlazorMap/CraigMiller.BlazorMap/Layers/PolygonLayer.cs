using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class PolygonLayer : ILayer
    {
        readonly IList<Polygon> _polygons;
        readonly SKPaint _fillPaint, _linePaint;

        public PolygonLayer()
        {
            _polygons = new List<Polygon>();

            _fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(220, 180, 180, 100)
            };
            _linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(20, 20, 20, 230),
                StrokeWidth = 2f
            };
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            using var path = new SKPath();

            foreach (Polygon polygon in _polygons)
            {
                SKPoint[] canvasPoints = converter.ProjectedToCanvas(polygon.ProjectedPoints);

                path.AddPoly(canvasPoints);

                canvas.DrawPath(path, _fillPaint);
                canvas.DrawPath(path, _linePaint);

                path.Reset();
            }
        }

        public void AddPolygon(IEnumerable<Location> points)
        {
            _polygons.Add(Polygon.FromLocations(points));
        }

        public void AddPolygon(PointD[] projectedPoints)
        {
            _polygons.Add(new Polygon(projectedPoints));
        }
    }

    public readonly struct Polygon
    {
        public readonly RectD Bounds;
        public readonly PointD[] ProjectedPoints;

        public static Polygon FromLocations(IEnumerable<Location> locations)
        {
            PointD[] projectedPoints = locations.Select(loc =>
            {
                SmcProjection.LatLonToProjected(loc.Latitude, loc.Longitude, out double prjX, out double prjY);

                return new PointD(prjX, prjY);
            }).ToArray();

            return new Polygon(projectedPoints);
        }

        public Polygon(PointD[] projectedPoints)
        {
            ProjectedPoints = projectedPoints;
            Bounds = RectD.BoundingPoints(projectedPoints);
        }
    }
}

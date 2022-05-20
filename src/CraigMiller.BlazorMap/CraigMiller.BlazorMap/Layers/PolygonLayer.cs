using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class PolygonLayer<TTag> : ILayer
    {
        readonly IList<Polygon<TTag>> _polygons;
        readonly SKPaint _fillPaint, _linePaint, _boundsPaint;

        public event Func<Polygon<TTag>, bool>? ShouldRender;

        public PolygonLayer()
        {
            _polygons = new List<Polygon<TTag>>();

            _fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(220, 180, 180, 100)
            };
            _linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(20, 20, 20, 230),
                StrokeWidth = 1f
            };
            _boundsPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 1f
            };
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            ProjectedRect projectedRect = converter.ProjectedRect;

            SKPoint a = converter.ProjectedToCanvas(projectedRect.Left, projectedRect.Bottom);
            SKPoint b = converter.ProjectedToCanvas(projectedRect.Left, projectedRect.Bottom + projectedRect.Height);
            canvas.DrawCircle(a.X + 10f, a.Y, 6f, _boundsPaint);
            canvas.DrawCircle(b.X + 20f, b.Y, 6f, _boundsPaint);

            using var path = new SKPath();

            foreach (Polygon<TTag> polygon in _polygons)
            {
                if (projectedRect.IntersectsWith(polygon.Bounds) && (ShouldRender is null || ShouldRender(polygon)))
                {
                    //RenderBounds(polygon.Bounds, canvas, converter);

                    SKPoint[] canvasPoints = converter.ProjectedToCanvas(polygon.ProjectedPoints);

                    path.AddPoly(canvasPoints);

                    canvas.DrawPath(path, _fillPaint);
                    canvas.DrawPath(path, _linePaint);

                    path.Reset();
                }
            }
        }

        void RenderBounds(ProjectedRect bounds, SKCanvas canvas, GeoConverter converter)
        {
            SKPoint a = converter.ProjectedToCanvas(bounds.Left, bounds.Bottom);
            SKPoint b = converter.ProjectedToCanvas(bounds.Right, bounds.Top);

            canvas.DrawCircle(a.X, a.Y, 4f, _boundsPaint);

            canvas.DrawRect(a.X, a.Y, b.X - a.X, b.Y - a.Y, _boundsPaint);
        }

        public void AddPolygon(IEnumerable<Location> points, TTag? tag = default)
        {
            _polygons.Add(Polygon<TTag>.FromLocations(points, tag));
        }

        public void AddPolygon(PointD[] projectedPoints, TTag? tag = default)
        {
            _polygons.Add(new Polygon<TTag>(projectedPoints, tag));
        }
    }

    public readonly struct Polygon<T>
    {
        public readonly ProjectedRect Bounds;
        public readonly PointD[] ProjectedPoints;
        public readonly T? Tag;

        public static Polygon<T> FromLocations(IEnumerable<Location> locations, T? tag = default)
        {
            PointD[] projectedPoints = locations.Select(loc =>
            {
                SmcProjection.LatLonToProjected(loc.Latitude, loc.Longitude, out double prjX, out double prjY);

                return new PointD(prjX, prjY);
            }).ToArray();

            return new Polygon<T>(projectedPoints, tag);
        }

        public Polygon(PointD[] projectedPoints, T? tag = default)
        {
            ProjectedPoints = projectedPoints;
            Bounds = ProjectedRect.BoundingPoints(projectedPoints);
            Tag = tag;
        }
    }
}

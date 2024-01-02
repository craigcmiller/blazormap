using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class CircleMarkerLayer : ILayer
    {
        readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f,
            IsAntialias = true,
        };

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            foreach (Location location in Locations)
            {
                DrawMarker(canvas, converter, location.Latitude, location.Longitude);
            }
        }

        private void DrawMarker(SKCanvas canvas, GeoConverter converter, double lat, double lon)
        {
            converter.LatLonToCanvas(lat, lon, out float x, out float y);
            canvas.DrawCircle(x, y, Radius, _paint);
        }

        public IList<Location> Locations { get; set; } = new List<Location>();

        public float Radius { get; set; } = 10f;

        public static DurationAnimatedLayer<CircleMarkerLayer> CreateAnimated(CircleMarkerLayer circleMarkerLayer, TimeSpan duration, float minRadius, float maxRadius)
        {
            float radiusDelta = maxRadius - minRadius;

            DurationAnimatedLayer<CircleMarkerLayer> animatedLayer = new(circleMarkerLayer, duration, (CircleMarkerLayer layer, GeoConverter converter, double secondsSinceStart, double ratioOfDuration) =>
            {
                float ratioOfRadius = (float)(ratioOfDuration * radiusDelta) + minRadius;

                circleMarkerLayer.Radius = ratioOfRadius;
            });

            return animatedLayer;
        }
    }
}

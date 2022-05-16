using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
{
    public class CircleMarkerLayer : ILayer
    {
        readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f
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
            canvas.DrawCircle(x, y, 10f, _paint);
        }

        public IList<Location> Locations { get; set; } = new List<Location>();
    }
}

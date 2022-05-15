using SkiaSharp;

namespace CraigMiller.BlazorMap.Engine
{
    public static class GeoConverterSkiaExtensions
    {
        public static SKPoint LatLonToCanvas(this GeoConverter converter, double latitude, double longitude)
        {
            converter.LatLonToCanvas(latitude, longitude, out float x, out float y);

            return new SKPoint(x, y);
        }
    }
}

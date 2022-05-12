using SkiaSharp;

namespace CraigMiller.BlazorMap.Engine
{
    public interface ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter);
    }
}

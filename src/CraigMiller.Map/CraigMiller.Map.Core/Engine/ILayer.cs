using SkiaSharp;

namespace CraigMiller.Map.Core.Engine
{
    public interface ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter);
    }
}

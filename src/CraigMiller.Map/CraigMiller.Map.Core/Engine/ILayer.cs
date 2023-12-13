using SkiaSharp;

namespace CraigMiller.Map.Core.Engine
{
    public interface ILayer
    {
        void DrawLayer(SKCanvas canvas, GeoConverter converter);
    }
}

using SkiaSharp;

namespace CraigMiller.Map.Core.Engine
{
    public interface IDataLayer
    {
        void DrawLayer(SKCanvas canvas, double canvasWidth, double canvasHeight, SKMatrix rotationMatrix, GeoConverter converter);
    }
}

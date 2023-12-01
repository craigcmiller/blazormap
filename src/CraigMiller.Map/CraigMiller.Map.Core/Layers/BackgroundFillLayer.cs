using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class BackgroundFillLayer : ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            canvas.Clear(Color);
        }

        public SKColor Color { get; set; } = SKColors.LightBlue;
    }
}

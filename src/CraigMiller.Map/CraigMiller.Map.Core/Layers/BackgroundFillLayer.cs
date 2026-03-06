using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Graphics;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class BackgroundFillLayer : ILayer
    {
        public void DrawLayer(SKCanvas canvas, GeoConverter converter, GraphicsObjects graphicsObjects)
        {
            canvas.Clear(Color);
        }

        public SKColor Color { get; set; } = SKColors.LightBlue;
    }
}

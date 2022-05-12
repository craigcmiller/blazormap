using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers
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

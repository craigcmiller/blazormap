using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.Graphics
{
    /// <summary>
    /// Renderer for drawing a map on a static bitmap
    /// </summary>
    public class MapImageRenderer
    {
        public MapImageRenderer()
        {
            Engine = new MapEngine();
        }

        public MapEngine Engine { get; }

        public SKBitmap RenderOnImage()
        {
            var bitmap = new SKBitmap((int)Engine.AreaView.CanvasWidth, (int)Engine.AreaView.CanvasHeight);

            using var canvas = new SKCanvas(bitmap);

            Engine.DrawMapLayers(canvas);

            return bitmap;
        }
    }
}

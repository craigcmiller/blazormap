using CraigMiller.Map.Core.Engine;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigMiller.Map.Core.Graphics
{
    /// <summary>
    /// Renderer for drawing a map on a static bitmap
    /// </summary>
    public class ImageMapRenderer
    {
        public ImageMapRenderer()
        {
            Engine = new MapEngine();
        }

        public MapEngine Engine { get; }

        public SKBitmap RenderOnImage()
        {
            var bitmap = new SKBitmap((int)Engine.AreaView.CanvasWidth, (int)Engine.AreaView.CanvasHeight);

            using var canvas = new SKCanvas(bitmap);

            Engine.Draw(canvas);

            return bitmap;
        }
    }
}

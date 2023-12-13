using CraigMiller.Map.Core.Engine;
using SkiaSharp;
using System.Text.Json;

namespace CraigMiller.Map.Core.Layers
{
    public class AsynchronousLayer : ILayer
    {
        Task? _runTask;
        readonly CanvasRenderer _renderer;
        BitmapConverter? _lastRender;

        public AsynchronousLayer(params ILayer[] layers)
        {
            _renderer = new();

            foreach(ILayer layer in layers)
            {
                _renderer.AddLayer(layer);
            }
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            GeoConverter clonedConverter = converter.Clone();

            try
            {
                if (_runTask == null)
                {
                    _runTask = Task.Run(() => Render(clonedConverter)).ContinueWith(t =>
                    {
                        _runTask = null;
                    });
                }

                if (_lastRender.HasValue)
                {
                    _lastRender.Value.Draw(canvas, converter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        void Render(GeoConverter converter)
        {
            SKBitmap bitmap = new((int)converter.CanvasWidth, (int)converter.CanvasHeight);
            using SKCanvas canvas = new(bitmap);

            _renderer.AreaView = converter;
            for (int i = 0; i < 1; i++)
            {
                _renderer.DrawMapLayers(canvas);
            }

            BitmapConverter? prevLastRender = _lastRender;

            _lastRender = new BitmapConverter(bitmap, converter.ProjectedRect);

            if (prevLastRender.HasValue)
            {
                prevLastRender.Value.Dispose();
            }
        }

        readonly struct BitmapConverter : IDisposable
        {
            readonly SKBitmap Bitmap;
            readonly ProjectedRect Bounds;

            public BitmapConverter(SKBitmap bitmap, ProjectedRect bounds)
            {
                Bitmap = bitmap;
                Bounds = bounds;
            }

            public void Dispose()
            {
                Bitmap.Dispose();
            }

            public void Draw(SKCanvas canvas, GeoConverter converter)
            {
                converter.ProjectedToCanvas(Bounds.Left, Bounds.Top, out float left, out float top);
                converter.ProjectedToCanvas(Bounds.Right, Bounds.Bottom, out float right, out float bottom);

                canvas.DrawBitmap(Bitmap, new SKRect(left, top, right, bottom));
            }
        }
    }
}

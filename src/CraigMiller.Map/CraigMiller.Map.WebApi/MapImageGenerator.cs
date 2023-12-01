using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Graphics;
using CraigMiller.Map.Core.Layers;
using CraigMiller.Map.Core.Layers.Tiling;
using SkiaSharp;

namespace CraigMiller.Map.WebApi
{
    public class MapImageGenerator
    {
        public static SKBitmap Generate(int width, int height, Location center, int tileZoomLevel)
        {
            ImageMapRenderer renderer = new();
            renderer.Engine.AreaView.CanvasWidth = width;
            renderer.Engine.AreaView.CanvasHeight = height;

            renderer.Engine.Center = center;
            renderer.Engine.Zoom = Tile.GetZoomScale(tileZoomLevel);

            AddDebugLayers(renderer.Engine);

            return renderer.RenderOnImage();
        }

        public static void AddDebugLayers(MapEngine engine)
        {
            engine.AddLayer(new BackgroundFillLayer());
            engine.AddLayer(new TileLayer(new HttpTileLoader(new HttpClient()))
            {
                SynchronousLoading = true
            });
            engine.AddLayer(new GridLineLayer());
            engine.AddLayer(new ScaleLayer());

            engine.AddLayer(new CircleMarkerLayer
            {
                Locations = new List<Location> {
                new Location(51,0),
                new Location(80,-170),
                new Location(80,170),
                new Location(-80, -170),
                new Location(-80, 170)
            }
            });

            engine.AddLayer(new DiagnosticsLayer());
        }
    }
}

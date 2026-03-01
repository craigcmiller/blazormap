using CraigMiller.Map.Core.DataLayers;
using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;
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
            MapImageRenderer renderer = new();
            renderer.Engine.AreaView.CanvasWidth = width;
            renderer.Engine.AreaView.CanvasHeight = height;

            renderer.Engine.Zoom = Tile.GetZoomScale(tileZoomLevel);
            renderer.Engine.Center = center;

            AddDebugLayers(renderer.Engine);

            return renderer.RenderOnImage();
        }

        public static void AddDebugLayers(MapEngine engine)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

            engine.AddLayer(new BackgroundFillLayer());
            engine.AddLayer(new TileLayer(new HttpTileLoader(httpClient))
            {
                SynchronousLoading = true
            });
            engine.AddLayer(new GridLineLayer());
            engine.AddDataLayer(new ScaleDataLayer());

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

            engine.AddDataLayer(new DiagnosticsDataLayer());
        }
    }
}

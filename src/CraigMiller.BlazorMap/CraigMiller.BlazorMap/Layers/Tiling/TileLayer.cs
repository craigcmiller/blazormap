using CraigMiller.BlazorMap.Engine;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers.Tiling
{
    public class TileLayer : ILayer
    {
        readonly HttpClient _httpClient;
        readonly ISet<Tile> _loadingTiles;
        readonly AccessOrderedCache<Tile, SKBitmap> _cache;

        public TileLayer(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _loadingTiles = new HashSet<Tile>();
            _cache = new AccessOrderedCache<Tile, SKBitmap>((_, bitmap) => bitmap.Dispose());
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            RectD projectedRect = converter.ProjectedRect;

            int zoomLevel = Tile.GetZoomLevel(converter.Zoom, TileSize);

            foreach (Tile tile in Tile.GetTilesInProjectedRect(projectedRect, zoomLevel, TileSize))
            {
                if (!projectedRect.IntersectsWith(tile.GetProjectedBounds(TileSize)))
                {
                    continue;
                }

                if (_cache.TryGetValue(tile, out SKBitmap? bitmap))
                {
                    RectD projected = tile.GetProjectedBounds(TileSize);

                    converter.ProjectedToCanvas(projected.X, projected.Y, out double x1, out double y1);
                    converter.ProjectedToCanvas(projected.X + projected.Width, projected.Y + projected.Height, out double x2, out double y2);

                    canvas.DrawBitmap(bitmap, new SKRect((float)x1, (float)y2, (float)x2, (float)y1));
                }
                else if (!_loadingTiles.Contains(tile))
                {
                    _httpClient.GetAsync($"https://tile.openstreetmap.org/{tile.Z}/{tile.X}/{tile.Y}.png").ContinueWith(t =>
                    {
                        HttpResponseMessage resp = t.Result;

                        Task.Run(() =>
                        {
                            SKBitmap bitmap;
                            using (Stream stream = resp.Content.ReadAsStream())
                            {
                                bitmap = SKBitmap.Decode(stream);
                            }
                            resp.Dispose();

                            _cache.Add(tile, bitmap);

                            lock (_loadingTiles)
                            {
                                _loadingTiles.Remove(tile);
                            }
                        });
                    });
                }
            }
        }

        public int TileSize { get; set; } = 256;
    }
}

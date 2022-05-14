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
        readonly IDictionary<Tile, SKBitmap> _cache;
        //readonly IMemoryCache _cache;

        public TileLayer(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _loadingTiles = new HashSet<Tile>();
            _cache = new Dictionary<Tile, SKBitmap>();
            /*_cache = new MemoryCache(Options.Create(new MemoryCacheOptions
            {
                SizeLimit = 256
            }));*/
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            RectD projectedRect = converter.ProjectedRect;

            int zoomLevel = Tile.GetZoomLevel(converter.Zoom, TileSize);

            foreach (Tile tile in Tile.GetTilesInProjectedRect(projectedRect, zoomLevel, TileSize))
            {
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

                        SKBitmap bitmap = SKBitmap.Decode(resp.Content.ReadAsStream());

                        //ICacheEntry cacheEntry = _cache.CreateEntry(tile);
                        //cacheEntry.SetValue(bitmap);

                        if (_cache.Count > 256)
                        {
                            foreach(SKBitmap toDispose in _cache.Values)
                            {
                                toDispose.Dispose();
                            }

                            _cache.Clear();
                        }

                        _cache.Add(tile, bitmap);

                        _loadingTiles.Remove(tile);
                    });
                }
            }
        }

        public int TileSize { get; set; } = 256;
    }
}

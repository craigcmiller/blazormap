using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers.Tiling
{
    public class TileLayer : ILayer
    {
        readonly ITileLoader _tileLoader;
        readonly ISet<Tile> _loadingTiles;
        readonly AccessOrderedCache<Tile, SKBitmap> _cache;
        readonly SKPaint _paint;

        public TileLayer(ITileLoader tileLoader)
        {
            _tileLoader = tileLoader;
            _loadingTiles = new HashSet<Tile>();
            _cache = new AccessOrderedCache<Tile, SKBitmap>((tile, bitmap) =>
            {
                bitmap.Dispose();
                _loadingTiles.Remove(tile);
            });
            _paint = new SKPaint
            {
                //IsAntialias = true,
                //FilterQuality = SKFilterQuality.High,
            };
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            ProjectedRect projectedRect = converter.ProjectedRect;

            int zoomLevel = Tile.GetZoomLevel(converter.Zoom, TileSize);

            foreach (Tile tile in Tile.GetTilesInProjectedRect(projectedRect, zoomLevel, TileSize))
            {
                if (!projectedRect.IntersectsWith(tile.GetProjectedBounds(TileSize)))
                {
                    continue;
                }

                SKBitmap? bitmap;
                if (SynchronousLoading)
                {
                    bitmap = _tileLoader.LoadTile(tile, default).Result;
                }
                else
                {
                    if (!_cache.TryGetValue(tile, out bitmap))
                    {
                        lock (_loadingTiles)
                        {
                            if (_loadingTiles.Contains(tile))
                            {
                                continue;
                            }

                            _loadingTiles.Add(tile);
                        }

                        _tileLoader.LoadTile(tile, default).ContinueWith(t =>
                        {
                            _cache.Add(tile, t.Result);

                            lock (_loadingTiles)
                            {
                                _loadingTiles.Remove(tile);
                            }
                        });

                        continue;
                    }
                }

                ProjectedRect projected = tile.GetProjectedBounds(TileSize);

                converter.ProjectedToCanvas(projected.Left, projected.Bottom, out double x1, out double y1);
                converter.ProjectedToCanvas(projected.Left + projected.Width, projected.Bottom + projected.Height, out double x2, out double y2);

                canvas.DrawBitmap(bitmap, new SKRect((float)x1, (float)y2, (float)x2, (float)y1), _paint);
            }
        }

        public int TileSize { get; set; } = 256;

        public bool SynchronousLoading { get; set; }
    }
}

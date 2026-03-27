using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Graphics;
using SkiaSharp;
using System.Numerics;

namespace CraigMiller.Map.Core.Layers.Tiling
{
    /// <summary>
    /// WMTS style tile layer. Loads and draws tiles from an ITileLoader. Caches loaded tiles in memory and disposes of them when they are evicted from the cache.
    /// </summary>
    public class TileLayer : ILayer, IDisposable
    {
        readonly ITileLoader _tileLoader;
        readonly ISet<Tile> _loadingTiles;
        readonly AccessOrderedCache<Tile, SKImage> _cache;
        readonly SKPaint _paint;
        readonly SKSamplingOptions _samplingOptions;

        public TileLayer(ITileLoader tileLoader)
        {
            _tileLoader = tileLoader;
            _loadingTiles = new HashSet<Tile>();
            _cache = new AccessOrderedCache<Tile, SKImage>((tile, bitmap) =>
            {
                bitmap.Dispose();
                _loadingTiles.Remove(tile);
            }, 1024);

            _paint = new SKPaint
            {
                IsAntialias = true,
            };
            _samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Nearest);
        }

        public void Dispose()
        {
            _paint.Dispose();

            _cache.Dispose();
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter, GraphicsObjects graphicsObjects)
        {
            // Adjust cache capacity based on the current zoom level and canvas size to try to keep enough tiles in memory
            // for smooth panning and zooming without using too much memory. The multiplier is somewhat arbitrary.
            _cache.Capacity = ((int)converter.CanvasWidth / TileSize) * ((int)converter.CanvasHeight / TileSize) * 8;

            ProjectedRect projectedRect = converter.ProjectedRect;

            int zoomLevel = Tile.GetZoomLevel(converter.Zoom, TileSize);

            foreach (Tile tile in Tile.GetTilesInProjectedRect(projectedRect, zoomLevel, TileSize))
            {
                if (!projectedRect.IntersectsWith(tile.GetProjectedBounds(TileSize)) || tile.X < 0 || tile.Y < 0)
                {
                    continue;
                }

                SKImage? image;
                if (SynchronousLoading)
                {
                    image = _tileLoader.LoadTile(tile, default).Result;
                }
                else
                {
                    if (!_cache.TryGetValue(tile, out image))
                    {
                        bool isAlreadyLoading = false;

                        lock (_loadingTiles)
                        {
                            if (_loadingTiles.Contains(tile))
                            {
                                isAlreadyLoading = true;
                            }
                            else
                            {
                                _loadingTiles.Add(tile);
                            }
                        }

                        if (!isAlreadyLoading)
                        {
                            _tileLoader.LoadTile(tile, default).ContinueWith(t =>
                            {
                                if (!t.IsCompletedSuccessfully)
                                {
                                    return;
                                }

                                _cache.Add(tile, t.Result);

                                lock (_loadingTiles)
                                {
                                    _loadingTiles.Remove(tile);
                                }
                            });
                        }

                        DrawZoomedOutTile(canvas, converter, graphicsObjects, tile);

                        continue;
                    }
                }

                ProjectedRect projected = tile.GetProjectedBounds(TileSize);

                converter.ProjectedToCanvas(projected.Left, projected.Bottom, out double x1, out double y1);
                converter.ProjectedToCanvas(projected.Right, projected.Top, out double x2, out double y2);

                canvas.DrawImage(image, new SKRect((float)Math.Round(x1), (int)Math.Round(y2), (float)Math.Round(x2), (float)Math.Round(y1)), _samplingOptions, _paint);
            }
        }

        void DrawZoomedOutTile(SKCanvas canvas, GeoConverter converter, GraphicsObjects graphicsObjects, Tile tile)
        {
            (Tile zoomedOutTile, int xQuadrant, int yQuadrant) = tile.GetZoomedOutEquivalentTile();

            if (zoomedOutTile.Equals(tile) || !_cache.TryGetValue(zoomedOutTile, out SKImage? zoomedOutImage))
            {
                return;
            }

            ProjectedRect projected = tile.GetProjectedBounds(TileSize);

            converter.ProjectedToCanvas(projected.Left, projected.Bottom, out double left, out double bottom);
            converter.ProjectedToCanvas(projected.Right, projected.Top, out double right, out double top);

            float canvasWidth = (float)Math.Round(right - left);
            float canvasHeight = (float)Math.Round(bottom - top);

            int halfTileSize = TileSize / 2;

            canvas.DrawImage(
                zoomedOutImage,
                new SKRect(halfTileSize * xQuadrant, halfTileSize * yQuadrant, halfTileSize * (xQuadrant + 1), halfTileSize * (yQuadrant + 1)),
                new SKRect((float)Math.Round(left), (int)Math.Round(top), (float)Math.Round(right), (float)Math.Round(bottom)),
                _samplingOptions,
                _paint);
        }

        public int TileSize { get; set; } = 256;

        public bool SynchronousLoading { get; set; }
    }
}

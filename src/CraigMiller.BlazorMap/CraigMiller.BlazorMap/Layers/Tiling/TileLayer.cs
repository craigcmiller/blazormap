﻿using CraigMiller.BlazorMap.Engine;
using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers.Tiling
{
    public class TileLayer : ILayer
    {
        readonly ITileLoader _tileLoader;
        readonly ISet<Tile> _loadingTiles;
        readonly AccessOrderedCache<Tile, SKBitmap> _cache;

        public TileLayer(ITileLoader tileLoader)
        {
            _tileLoader = tileLoader;
            _loadingTiles = new HashSet<Tile>();
            _cache = new AccessOrderedCache<Tile, SKBitmap>((tile, bitmap) =>
            {
                bitmap.Dispose();
                _loadingTiles.Remove(tile);
            });
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

                if (_cache.TryGetValue(tile, out SKBitmap? bitmap))
                {
                    ProjectedRect projected = tile.GetProjectedBounds(TileSize);

                    converter.ProjectedToCanvas(projected.Left, projected.Bottom, out double x1, out double y1);
                    converter.ProjectedToCanvas(projected.Left + projected.Width, projected.Bottom + projected.Height, out double x2, out double y2);

                    canvas.DrawBitmap(bitmap, new SKRect((float)x1, (float)y2, (float)x2, (float)y1));
                }
                else
                {
                    lock (_loadingTiles)
                    {
                        if (_loadingTiles.Contains(tile))
                        {
                            continue;
                        }
                        else
                        {
                            _loadingTiles.Add(tile);
                        }
                    }

                    _tileLoader.LoadTile(tile, default).ContinueWith(t =>
                    {
                        _cache.Add(tile, t.Result);

                        lock (_loadingTiles)
                        {
                            _loadingTiles.Remove(tile);
                        }
                    });
                }
            }
        }

        public int TileSize { get; set; } = 256;
    }
}

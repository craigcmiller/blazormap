using SkiaSharp;

namespace CraigMiller.Map.Core.Layers.Tiling
{
    public interface ITileLoader
    {
        Task<SKBitmap> LoadTile(Tile tile, CancellationToken cancellation);
    }
}

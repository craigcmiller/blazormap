using SkiaSharp;

namespace CraigMiller.Map.Core.Layers.Tiling
{
    public interface ITileLoader
    {
        Task<SKImage> LoadTile(Tile tile, CancellationToken cancellation);
    }
}

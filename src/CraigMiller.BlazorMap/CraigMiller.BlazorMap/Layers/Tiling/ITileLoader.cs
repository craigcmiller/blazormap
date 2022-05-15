using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers.Tiling
{
    public interface ITileLoader
    {
        Task<SKBitmap> LoadTile(Tile tile, CancellationToken cancellation);
    }
}

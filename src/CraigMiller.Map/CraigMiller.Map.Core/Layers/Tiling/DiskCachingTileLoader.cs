using SkiaSharp;

namespace CraigMiller.Map.Core.Layers.Tiling;

public class DiskCachingTileLoader : ITileLoader
{
    readonly ITileLoader _innerLoader;
    readonly string _cacheDirectory;

    public DiskCachingTileLoader(ITileLoader innerLoader, string cacheDirectory)
    {
        _innerLoader = innerLoader;
        _cacheDirectory = cacheDirectory;

        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }
    }

    public async Task<SKImage> LoadTile(Tile tile, CancellationToken cancellation)
    {
        string cachePath = GetCachePath(tile);

        if (File.Exists(cachePath))
        {
            return SKImage.FromEncodedData(cachePath);
        }

        SKImage image = await _innerLoader.LoadTile(tile, cancellation);
        using FileStream fs = new(cachePath, FileMode.Create, FileAccess.Write);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);

        return image;
    }

    string GetCachePath(Tile tile) => Path.Combine(_cacheDirectory, $"{tile.Z}_{tile.Y}_{tile.X}.png");
}

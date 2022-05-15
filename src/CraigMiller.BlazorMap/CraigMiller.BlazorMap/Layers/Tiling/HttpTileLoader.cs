using SkiaSharp;

namespace CraigMiller.BlazorMap.Layers.Tiling
{
    public class HttpTileLoader : ITileLoader
    {
        readonly HttpClient _httpClient;

        public HttpTileLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SKBitmap> LoadTile(Tile tile, CancellationToken cancellation)
        {
            using HttpResponseMessage resp = await _httpClient.GetAsync($"https://tile.openstreetmap.org/{tile.Z}/{tile.X}/{tile.Y}.png", cancellation);

            // Read as a byte array to minimise time spent in SKBitmap.Decode(). Until we have threading in Blazor this leads to a laggy UI
            byte[] bytes = await resp.Content.ReadAsByteArrayAsync(cancellation);

            return SKBitmap.Decode(bytes);
        }
    }
}

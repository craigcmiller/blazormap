using SkiaSharp;

namespace CraigMiller.Map.Core.Layers.Tiling
{
    public class HttpTileLoader : ITileLoader
    {
        readonly HttpClient _httpClient;
        readonly Func<Tile, string> _createUrlFunction;

        public static string CreateOsmUrlFunction(Tile tile) => $"https://tile.openstreetmap.org/{tile.Z}/{tile.X}/{tile.Y}.png";

        public HttpTileLoader(HttpClient httpClient, Func<Tile, string>? createUrlFunction = null)
        {
            _httpClient = httpClient;
            _createUrlFunction = createUrlFunction ?? CreateOsmUrlFunction;
        }

        public async Task<SKBitmap> LoadTile(Tile tile, CancellationToken cancellation)
        {
            string url = _createUrlFunction(tile);

            using HttpResponseMessage resp = await _httpClient.GetAsync(url, cancellation);

            if (!resp.IsSuccessStatusCode)
            {
                string failedResponseMessage = await resp.Content.ReadAsStringAsync(cancellation);
                throw new InvalidDataException($"Download of tile from {url} failed with \"{failedResponseMessage}\"");
            }

            // Read as a byte array to minimise time spent in SKBitmap.Decode(). Until we have threading in Blazor this leads to a laggy UI
            byte[] bytes = await resp.Content.ReadAsByteArrayAsync(cancellation);

            return SKBitmap.Decode(bytes);
        }
    }
}

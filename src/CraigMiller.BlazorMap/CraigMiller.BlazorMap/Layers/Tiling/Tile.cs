using CraigMiller.BlazorMap.Engine;
using System.Diagnostics.CodeAnalysis;

namespace CraigMiller.BlazorMap.Layers.Tiling
{
	/// <summary>
	/// Specherical Mercator Web Map Tile
	/// </summary>
    public readonly struct Tile
	{
		public readonly int X, Y, Z;

		public Tile(int x, int y, int z, int tileSize = 256)
		{
			int tilesPerLine = GetTilesPerLine(z, tileSize);

			X = x % tilesPerLine;
			if (X < 0)
			{
				X += tilesPerLine;
			}
			Y = y % tilesPerLine;
			Z = z;
		}

		public override string ToString() => $"Tile X: {X}, Y: {Y}, Z: {Z}";

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
			if (obj is Tile tile)
            {
				return tile.X == X && tile.Y == Y && tile.Z == Z;
            }

			return false;
        }

		public override int GetHashCode() => X ^ Y ^ Z;

        /// <summary>
        /// Gets the tile zoom level for a zoom scale
        /// </summary>
        /// <param name="zoomScale"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static int GetZoomLevel(double zoomScale, int tileSize = 256)
			=> (int)Math.Round(Math.Log(zoomScale * SmcProjection.WorldWidthHeight / 2.0 / tileSize) / Math.Log(2.0)) + 1;

		/// <summary>
		/// Gets the zoom scale for a tile zoom level
		/// </summary>
		/// <param name="tileZoom"></param>
		/// <param name="tileSize"></param>
		/// <returns></returns>
		public static double GetZoomScale(int tileZoom, int tileSize = 256)
			=> Math.Pow(2.0, tileZoom) * tileSize / SmcProjection.WorldWidthHeight;

		/// <summary>
		/// Gets the tile Spherical Mercator bounds for the given pixel (X and Y) tileSizes
		/// </summary>
		/// <param name="tileSize">Tile width and height in pixels</param>
		/// <returns></returns>
		public RectD GetProjectedBounds(int tileSize = 256)
		{
			double smcPerTile = GetProjectedTileSize(Z, tileSize);

			return new RectD(SmcProjection.WorldMin + smcPerTile * X, SmcProjection.WorldMax - smcPerTile * (Y + 1), smcPerTile, smcPerTile);
		}

		/// <summary>
		/// Gets the tile Spherical Mercator bounds for the given pixel (X and Y) tileSizes
		/// </summary>
		/// <param name="width">Tile width tileSize in pixels</param>
		/// <param name="height">Tile height tileSize in pixels</param>
		/// <returns></returns>
		public RectD GetProjectedBounds(int width, int height)
		{
			double smcWidthPerTile = GetProjectedTileSize(Z, width);
			double smcHeightPerTile = GetProjectedTileSize(Z, height);

			return new RectD(SmcProjection.WorldMin + smcWidthPerTile * X, SmcProjection.WorldMax - smcHeightPerTile * (Y + 1), smcWidthPerTile, smcHeightPerTile);
		}

		/// <summary>
		/// Gets the projected width and height of a single tile for zoom level <paramref name="zoomLevel"/>
		/// </summary>
		/// <param name="zoomLevel"></param>
		/// <param name="pixelTileSize"></param>
		/// <returns></returns>
		public static double GetProjectedTileSize(int zoomLevel, int pixelTileSize = 256) => SmcProjection.WorldWidthHeight / GetTilesPerLine(zoomLevel, pixelTileSize);

		public static int GetTilesPerLine(int zoom, int tileSize = 256) => (int)Math.Round(Math.Pow(2.0, zoom) / (tileSize / 256.0));

		/// <summary>
		/// Enumerates all tiles at a zoom level
		/// </summary>
		/// <param name="zoomLevel"></param>
		/// <param name="tileSize"></param>
		/// <returns></returns>
		public static IEnumerable<Tile> TilesForZoomLevel(int zoomLevel, int tileSize = 256)
		{
			int tilesPerLine = GetTilesPerLine(zoomLevel, tileSize);

			for (int yIndex = 0; yIndex < tilesPerLine; yIndex++)
			{
				for (int xIndex = 0; xIndex < tilesPerLine; xIndex++)
				{
					yield return new Tile(xIndex, yIndex, zoomLevel);
				}
			}
		}

		/// <summary>
		/// Gets all the tiles in <paramref name="projectedRect"/> at zoom level <paramref name="zoomLevel"/>
		/// </summary>
		/// <param name="projectedRect"></param>
		/// <param name="zoomLevel"></param>
		/// <param name="tileSize"></param>
		/// <returns></returns>
		public static IEnumerable<Tile> GetTilesInProjectedRect(RectD projectedRect, int zoomLevel, int tileSize = 256)
		{
			double tileProjectedtileSize = GetProjectedTileSize(zoomLevel, tileSize);

			int tilesPerLine = GetTilesPerLine(zoomLevel, tileSize);

			const double halfWorldSmcWidth = SmcProjection.WorldWidthHeight / 2.0;

			int leftTile = (int)((halfWorldSmcWidth + projectedRect.X) / tileProjectedtileSize);
			int topTile = tilesPerLine - (int)Math.Ceiling((halfWorldSmcWidth + projectedRect.Y) / tileProjectedtileSize);
			int rightTile = (int)((halfWorldSmcWidth + projectedRect.Right) / tileProjectedtileSize);
			int bottomTile = tilesPerLine - (int)Math.Ceiling((halfWorldSmcWidth + projectedRect.Bottom) / tileProjectedtileSize);

			for (int y = bottomTile; y <= topTile; y++)
			{
				for (int x = leftTile; x <= rightTile; x++)
				{
					yield return new Tile(x, y, zoomLevel);
				}
			}
		}
	}
}

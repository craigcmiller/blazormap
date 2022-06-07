namespace CraigMiller.BlazorMap.Engine
{
    public readonly struct GeoRect
    {
        public readonly double NorthLatitude, SouthLatitude, EastLongitude, WestLongitude;

        public GeoRect(double northLatitude, double westLongitude, double southLatitude, double eastLongitude)
        {
            NorthLatitude = northLatitude;
            SouthLatitude = southLatitude;
            WestLongitude = westLongitude;
            EastLongitude = eastLongitude;
        }

        public bool Contains(double latitude, double longitude)
            => latitude >= SouthLatitude && latitude <= NorthLatitude && longitude >= WestLongitude && longitude <= EastLongitude;

        /// <summary>
        /// Expand the rect by ratio <paramref name="multiplier"/> of the lat and lon of the rect
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public GeoRect ExpandRect(double multiplier)
        {
            double latTolerance = (NorthLatitude - SouthLatitude) * multiplier;
            double lonTolerance = (EastLongitude - WestLongitude) * multiplier;

            double northLat = NorthLatitude + latTolerance;
            double southLat = SouthLatitude - latTolerance;
            double eastLon = EastLongitude + lonTolerance;
            double westLon = WestLongitude - lonTolerance;

            return new GeoRect(northLat, westLon, southLat, eastLon);
        }
    }
}

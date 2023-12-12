namespace CraigMiller.Map.Core.Engine
{
    /// <summary>
    /// Geographic rectangle bounded by north, west, south and east latitudes and longitudes
    /// </summary>
    public readonly struct GeoRect
    {
        public readonly double NorthLatitude, SouthLatitude, EastLongitude, WestLongitude;

        public static GeoRect EncompassingLocations(IEnumerable<Location> locations)
        {
            if (locations.Count() == 0)
            {
                throw new ArgumentException("Locations is empty", nameof(locations));
            }

            Location first = locations.First();
            double northLatitude = first.Latitude, southLatitude = first.Latitude, eastLongitude = first.Longitude, westLongitude = first.Longitude;
            foreach (Location location in locations.Skip(1))
            {
                if (location.Latitude > northLatitude)
                {
                    northLatitude = location.Latitude;
                }
                if (location.Latitude < southLatitude)
                {
                    southLatitude = location.Latitude;
                }
                if (location.Longitude > eastLongitude)
                {
                    eastLongitude = location.Longitude;
                }
                if (location.Longitude < westLongitude)
                {
                    westLongitude = location.Longitude;
                }
            }

            return new GeoRect(northLatitude, westLongitude, southLatitude, eastLongitude);
        }

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

        public Location Center => new Location(SouthLatitude + (NorthLatitude - SouthLatitude) / 2.0, WestLongitude + (EastLongitude - WestLongitude) / 2.0);

        public override string ToString() =>
            $"{NorthLatitude} {WestLongitude}, {SouthLatitude} {EastLongitude}";
    }
}

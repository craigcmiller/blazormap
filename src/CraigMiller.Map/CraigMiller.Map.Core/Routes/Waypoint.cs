using CraigMiller.Map.Core.Geo;

namespace CraigMiller.Map.Core.Routes
{
    public class Waypoint
    {
        public Waypoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Waypoint(Location location) : this(location.Latitude, location.Longitude) { }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Location Location => new Location(Latitude, Longitude);
    }
}

using CraigMiller.BlazorMap.Engine;

namespace CraigMiller.BlazorMap.Routes
{
    public class Waypoint
    {
        public Waypoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Location Location => new Location(Latitude, Longitude);
    }
}

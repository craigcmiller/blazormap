namespace CraigMiller.BlazorMap.Engine
{
    public readonly struct Location
    {
        public readonly double Latitude, Longitude;

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}

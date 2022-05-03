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
    }
}

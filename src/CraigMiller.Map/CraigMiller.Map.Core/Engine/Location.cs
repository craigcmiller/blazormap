namespace CraigMiller.Map.Core.Engine
{
    public readonly struct Location
    {
        public readonly double Latitude, Longitude;

        public static double InitialBearingDegrees(Location pointA, Location pointB)
        {
            double lat1 = MathHelper.DegsToRads(pointA.Latitude);
            double lon1 = MathHelper.DegsToRads(pointA.Longitude);
            double lat2 = MathHelper.DegsToRads(pointB.Latitude);
            double lon2 = MathHelper.DegsToRads(pointB.Longitude);

            double deltaLon = lon2 - lon1;

            double x = Math.Sin(deltaLon) * Math.Cos(lat2);
            double y = Math.Cos(lat1) * Math.Sin(lat2) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));

            double initialBearing = Math.Atan2(x, y);

            // Convert radians to degrees
            initialBearing = MathHelper.RadsToDegs(initialBearing);

            // Convert bearing to compass direction
            return (initialBearing + 360) % 360;
        }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => $"{Latitude} {Longitude}";
    }
}

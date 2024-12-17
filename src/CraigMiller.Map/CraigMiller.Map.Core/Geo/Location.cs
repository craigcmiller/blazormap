using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Geo;

/// <summary>
/// Latitude longitude pair
/// </summary>
public readonly struct Location
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    const string DecimalDegreesRegexPattern = @"(?<lat>\-?\d+\.\d+)\s*,?\s*(?<lon>\-?\d+\.\d+)";

    public readonly double Latitude, Longitude;

    /// <summary>
    /// Gets a location at 0 latitude and 0 longitude
    /// </summary>
    public static Location NullIsland => new Location(0, 0);

    public static double InitialBearingDegrees(Location pointA, Location pointB)
    {
        double lat1 = MathHelper.DegsToRads(pointA.Latitude);
        double lon1 = MathHelper.DegsToRads(pointA.Longitude);
        double lat2 = MathHelper.DegsToRads(pointB.Latitude);
        double lon2 = MathHelper.DegsToRads(pointB.Longitude);

        double deltaLon = lon2 - lon1;

        double x = Math.Sin(deltaLon) * Math.Cos(lat2);
        double y = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);

        double initialBearing = Math.Atan2(x, y);

        // Convert radians to degrees
        initialBearing = MathHelper.RadsToDegs(initialBearing);

        // Convert bearing to compass direction
        return (initialBearing + 360) % 360;
    }

    public static bool TryParse(string str, out Location location)
    {
        Match decimalDegreesMatch= Regex.Match(str, DecimalDegreesRegexPattern);
        if (decimalDegreesMatch.Success)
        {
            location = new Location(double.Parse(decimalDegreesMatch.Groups["lat"].Value), double.Parse(decimalDegreesMatch.Groups["lon"].Value));
            return true;
        }

        location = new Location(double.NaN, double.NaN);
        return false;
    }

    public static bool TryParsePart(string str, out double value)
    {
        if (double.TryParse(str, out value))
        {
            return true;
        }

        return false;
    }

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Gets if the location is within valid latitude and longitude values
    /// </summary>
    public bool IsValid => Latitude >= -90.0 && Latitude <= 90.0 && Longitude >= -180.0 && Longitude <= 180.0;

    public override string ToString() => $"{Latitude} {Longitude}";

    public PartLocation PartLatitude => new PartLocation(LocationPartType.Latitude, Latitude);

    public PartLocation PartLongitude => new PartLocation(LocationPartType.Longitude, Longitude);

    public string ToDegreesMinutesSecondsString(bool useSymbols = true, int secondsDecimalPlaces = 0, string separator = " ")
        => $"{PartLatitude.ToDegreesMinutesSecondsString(useSymbols, secondsDecimalPlaces)}{separator}{PartLongitude.ToDegreesMinutesSecondsString(useSymbols, secondsDecimalPlaces)}";
}

public enum CardinalDirection { North, South, East, West }

public enum LocationPartType { Latitude, Longitude }

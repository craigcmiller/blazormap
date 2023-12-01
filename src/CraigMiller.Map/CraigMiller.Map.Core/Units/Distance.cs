using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Units;

public readonly struct Distance
{
    const double MetreInNauticalMiles = 0.000539957;
    const double MetreInStatuteMiles = 0.000621371;
    const double MetreInKilometres = 0.0001;
    const double MetreInFeet = 3.28084;
    public readonly double Metres;

    public static Distance Zero => new Distance(0);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="otherLatitude"></param>
    /// <param name="otherLongitude"></param>
    /// <returns></returns>
    /// <remarks>From https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates</remarks>
    static double DistanceBetweenPointsMetres(double latitude, double longitude, double otherLatitude, double otherLongitude)
    {
        var d1 = latitude * (Math.PI / 180.0);
        var num1 = longitude * (Math.PI / 180.0);
        var d2 = otherLatitude * (Math.PI / 180.0);
        var num2 = otherLongitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }

    public static Distance Between(Location a, Location b)
        => Between(a.Latitude, a.Longitude, b.Latitude, b.Longitude);

    public static Distance Between(double latA, double lonA, double latB, double lonB)
        => new Distance(DistanceBetweenPointsMetres(latA, lonA, latB, lonB));

    public static Distance FromNauticalMiles(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnits.NauticalMiles);

    public static Distance FromStatuteMiles(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnits.StatuteMiles);

    public static Distance FromKilometres(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnits.Kilometres);

    public static Distance FromFeet(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnits.Feet);

    public static Distance FromMetres(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnits.Metres);

    public Distance(double value, DistanceUnits units) : this(units switch
    {
        DistanceUnits.Metres => value,
        DistanceUnits.Feet => value / MetreInFeet,
        DistanceUnits.Kilometres => value / MetreInKilometres,
        DistanceUnits.NauticalMiles => value / MetreInNauticalMiles,
        DistanceUnits.StatuteMiles => value / MetreInStatuteMiles,
        _ => throw new NotImplementedException()
    })
    { }

    public Distance(double metres)
    {
        Metres = metres;
    }

    public double Feet => MetreInFeet * Metres;

    public double NatuticalMiles => MetreInNauticalMiles * Metres;

    public double Kilometres => Metres * MetreInKilometres;

    public double StatuteMiles => Metres * MetreInStatuteMiles;

    public double GetValue(DistanceUnits units)
    {
        return units switch
        {
            DistanceUnits.Metres => Metres,
            DistanceUnits.Feet => Feet,
            DistanceUnits.Kilometres => Kilometres,
            DistanceUnits.NauticalMiles => NatuticalMiles,
            DistanceUnits.StatuteMiles => StatuteMiles,
            _ => throw new NotImplementedException()
        };
    }
}

public enum DistanceUnits
{
    Metres,
    Feet,
    Kilometres,
    NauticalMiles,
    StatuteMiles
}

public static class DistanceUnitsExtensions
{
    public static string ShortSuffix(this DistanceUnits units) => units switch
    {
        DistanceUnits.Metres => "m",
        DistanceUnits.Feet => "ft",
        DistanceUnits.Kilometres => "km",
        DistanceUnits.NauticalMiles => "nm",
        DistanceUnits.StatuteMiles => "sm",
        _ => throw new NotImplementedException()
    };
}

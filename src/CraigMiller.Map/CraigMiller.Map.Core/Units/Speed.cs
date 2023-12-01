namespace CraigMiller.Map.Core.Units;

public readonly struct Speed
{
    const double KnotInMetresPerSecond = 0.514444;
    const double KphInMetresPerSecond = 0.277778;
    const double MphInMetresPerSecond = 0.44704;

    public readonly double MetresPerSecond;

    public static Speed FromMetresPerSecond(double metresPerSecond) => new Speed(metresPerSecond, SpeedUnits.MetresPerSecond);

    public static Speed FromKnots(double knots) => new Speed(knots, SpeedUnits.Knots);

    public static Speed FromMph(double mpg) => new Speed(mpg, SpeedUnits.Mph);

    public static Speed FromKph(double kph) => new Speed(kph, SpeedUnits.Kph);

    public Speed(double value, SpeedUnits units)
    {
        MetresPerSecond = units switch
        {
            SpeedUnits.MetresPerSecond => value,
            SpeedUnits.Knots => value * KnotInMetresPerSecond,
            SpeedUnits.Kph => value * KphInMetresPerSecond,
            SpeedUnits.Mph => value * MphInMetresPerSecond,
            _ => throw new NotImplementedException(),
        };
    }

    public double Knots => MetresPerSecond / KnotInMetresPerSecond;

    public double Kph => MetresPerSecond / KphInMetresPerSecond;

    public double Mph => MetresPerSecond / MphInMetresPerSecond;

    public double GetValue(SpeedUnits units)
    {
        return units switch
        {
            SpeedUnits.MetresPerSecond => MetresPerSecond,
            SpeedUnits.Knots => Knots,
            SpeedUnits.Kph => Kph,
            SpeedUnits.Mph => Mph,
            _ => throw new NotImplementedException(),
        };
    }
}

public enum SpeedUnits
{
    MetresPerSecond,
    Knots,
    Kph,
    Mph
}

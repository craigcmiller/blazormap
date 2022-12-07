namespace CraigMiller.BlazorMap.Units;

public readonly struct Pressure
{
    const double HectopascalInPascals = 100;
    const double HectopascalInInchesOfMercury = 0.02953;
    const double HectopascalInBars = 0.001;
    const double HectopascalInPoundsPerSquareInch = 0.0145038;

    public readonly double Hectopascals;

    public static Pressure FromHectopascals(double hectopascals) => new Pressure(hectopascals, PressureUnits.Hectopascals);

    public static Pressure FromPascals(double pascals) => new Pressure(pascals, PressureUnits.Pascals);

    public static Pressure FromInchesOfMercury(double inchesOfMercury) => new Pressure(inchesOfMercury, PressureUnits.InchesOfMercury);

    public static Pressure FromBars(double bars) => new Pressure(bars, PressureUnits.Bars);

    public static Pressure FromPoundsPerSquareInch(double psi) => new Pressure(psi, PressureUnits.PoundsPerSquareInch);

    public Pressure(double value, PressureUnits units)
    {
        Hectopascals = units switch
        {
            PressureUnits.Pascals => value / HectopascalInPascals,
            PressureUnits.Hectopascals => value,
            PressureUnits.InchesOfMercury => value / HectopascalInInchesOfMercury,
            PressureUnits.Bars => value / HectopascalInBars,
            PressureUnits.PoundsPerSquareInch => value / HectopascalInPoundsPerSquareInch,
            _ => throw new NotImplementedException()
        };
    }

    public double GetValue(PressureUnits units)
    {
        return units switch
        {
            PressureUnits.Pascals => Pascals,
            PressureUnits.Hectopascals => Hectopascals,
            PressureUnits.InchesOfMercury => InchesOfMercury,
            PressureUnits.Bars => Bars,
            PressureUnits.PoundsPerSquareInch => PoundsPerSquareInch,
            _ => throw new NotImplementedException()
        };
    }

    public double Pascals => Hectopascals * HectopascalInPascals;

    public double InchesOfMercury => Hectopascals * HectopascalInInchesOfMercury;

    public double Bars => Hectopascals * HectopascalInBars;

    public double PoundsPerSquareInch => Hectopascals * HectopascalInPoundsPerSquareInch;
}

public enum PressureUnits
{
    Pascals,
    Hectopascals,
    InchesOfMercury,
    Bars,
    PoundsPerSquareInch
}

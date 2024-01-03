namespace CraigMiller.Map.Core.Geo;

/// <summary>
/// Location part either representing a latitude or a longitude
/// </summary>
public readonly struct PartLocation
{
    public PartLocation(LocationPartType partType, double value)
    {
        PartType = partType;
        DecimalValue = value;
    }

    public LocationPartType PartType { get; }

    public double DecimalValue { get; }

    public int Degrees => (int)Math.Abs(DecimalValue);

    public double MinutesDecimalSeconds => (Math.Abs(DecimalValue) - Degrees) * 60.0;

    public int Minutes => (int)MinutesDecimalSeconds;

    public double Seconds => (MinutesDecimalSeconds - Minutes) * 60.0;

    public CardinalDirection CardinalDirection => PartType switch
    {
        LocationPartType.Latitude => DecimalValue < 0.0 ? CardinalDirection.South : CardinalDirection.North,
        LocationPartType.Longitude => DecimalValue < 0.0 ? CardinalDirection.West : CardinalDirection.East,
        _ => throw new InvalidDataException($"Invalid {nameof(PartType)}: {PartType}")
    };

    public string DegreesFormat => PartType switch
    {
        LocationPartType.Latitude => "00",
        LocationPartType.Longitude => "000",
        _ => throw new InvalidDataException($"Invalid {nameof(PartType)}: {PartType}")
    };

    char CardinalDirectionToChar(CardinalDirection cardinalDirection) => cardinalDirection switch
    {
        CardinalDirection.North => 'N',
        CardinalDirection.South => 'S',
        CardinalDirection.East => 'E',
        CardinalDirection.West => 'W',
        _ => throw new InvalidDataException($"Invalid {nameof(CardinalDirection)}: {cardinalDirection}")
    };

    public string ToDegreesMinutesSecondsString(bool useSymbols, int secondsDecimalPlaces)
    {
        string degreesStr = Degrees.ToString(DegreesFormat);
        string minutesStr = Minutes.ToString("00");
        string secondsStr = Seconds.ToString("00" + (secondsDecimalPlaces > 0 ? ".".PadRight(secondsDecimalPlaces + 1, '0') : string.Empty));
        char cardinalChar = CardinalDirectionToChar(CardinalDirection);

        if (useSymbols)
        {
            return $"{degreesStr}° {minutesStr}' {secondsStr}\" {cardinalChar}";
        }

        return $"{degreesStr} {minutesStr} {secondsStr} {cardinalChar}";
    }
}

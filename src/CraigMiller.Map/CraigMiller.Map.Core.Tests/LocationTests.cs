using CraigMiller.Map.Core.Geo;

namespace CraigMiller.Map.Core.Tests;

public class LocationTests
{
    [Theory]
    [InlineData("25.25 -1.5", 25.25, -1.5)]
    [InlineData("-25.25, 1.5", -25.25, 1.5)]
    public void TryParse(string str, double expectedLatitude, double expectedLongitude)
    {
        bool success = Location.TryParse(str, out Location parsedLocation);

        Assert.True(success);
        Assert.Equal(expectedLatitude, parsedLocation.Latitude);
        Assert.Equal(expectedLongitude, parsedLocation.Longitude);
    }

    [Theory]
    [InlineData(true, 0, 0)]
    [InlineData(true, -80, -170)]
    [InlineData(true, 80, 170)]
    [InlineData(false, -170, 0)]
    [InlineData(false, 170, 0)]
    [InlineData(false, 0, -190)]
    [InlineData(false, 0, 190)]
    public void IsValid(bool valid, double lat, double lon)
    {
        Assert.Equal(valid, new Location(lat, lon).IsValid);
    }

    [Theory]
    [InlineData("50 30 00 N 001 15 00 W", 50.5, -1.25, false, 0)]
    [InlineData("50 30 00.0 N 001 15 00.0 E", 50.5, 1.25, false, 1)]
    [InlineData("50° 30' 00.0\" S 001° 15' 00.0\" W", -50.5, -1.25, true, 1)]
    public void ToDegreesMinutesSecondsString(string expected, double lat, double lon, bool useSymbols, int secondsDecimalPlaces)
    {
        Location loc = new(lat, lon);

        Assert.Equal(expected, loc.ToDegreesMinutesSecondsString(useSymbols, secondsDecimalPlaces));
    }
}

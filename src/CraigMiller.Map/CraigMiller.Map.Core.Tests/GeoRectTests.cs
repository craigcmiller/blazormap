using CraigMiller.Map.Core.Geo;

namespace CraigMiller.Map.Core.Tests;

public class GeoRectTests
{
    [Fact]
    public void EncompassingTwoPoints()
    {
        double south = -1, west = -2, north = 1, east = 2;

        GeoRect rect = GeoRect.EncompassingLocations([new Location(north, east), new Location(south, west)]);

        Assert.Equal(north, rect.NorthLatitude);
        Assert.Equal(east, rect.EastLongitude);
        Assert.Equal(south, rect.SouthLatitude);
        Assert.Equal(west, rect.WestLongitude);
    }
}

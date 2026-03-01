namespace CraigMiller.Map.Core.Geo.WellKnownText;

/// <summary>
/// Well Known Text (WKT) shape
/// </summary>
public class WktShape
{
    public WktShape(WktShapeType shapeType)
        : this(shapeType, new List<IList<Location>> { new List<Location>() }) { }

    public WktShape(WktShapeType shapeType, IList<IList<Location>> multiCoordinates)
    {
        ShapeType = shapeType;
        MultiCoordinates = multiCoordinates;
    }

    public WktShapeType ShapeType { get; }

    public IList<Location> Locations => MultiCoordinates[0];

    public IList<IList<Location>> MultiCoordinates { get; }
}

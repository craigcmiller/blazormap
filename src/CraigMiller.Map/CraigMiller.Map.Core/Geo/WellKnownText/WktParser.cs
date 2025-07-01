using System.Text;
using System.Text.RegularExpressions;

namespace CraigMiller.Map.Core.Geo.WellKnownText;

/// <summary>
/// Simple Well Known Text (WKT) parsing. Only POINT, POLYGON and MULTIPOLYGON are supported
/// </summary>
/// <remarks>
/// See https://en.wikipedia.org/wiki/Well-known_text
/// </remarks>
public class WKtParser
{
    public static WktShape? TryParse(string text)
    {
        Match match = Regex.Match(text, @"(?<type>\w+)\s*\(\(?(?<points>(\-|\.|\d|,|\s|e)+)\)?\)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            string geometryType = match.Groups["type"].Value;
            string pointsStr = match.Groups["points"].Value;

            string[] pointStrings = pointsStr.Split(',');

            WktShapeType shapeType;
            switch (geometryType)
            {
                case "POINT":
                    shapeType = WktShapeType.Point;
                    break;
                case "LINESTRING":
                    shapeType = WktShapeType.LineString;
                    break;
                case "POLYGON":
                    shapeType = WktShapeType.Polygon;
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unsupported shape type: {0}", geometryType));
            }

            var shape = new WktShape(shapeType);

            for (int i = 0; i < pointStrings.Length; i++)
            {
                string[] coordinateParts = Regex.Split(pointStrings[i].Trim(), "\\s+");

                shape.Locations.Add(new Location(double.Parse(coordinateParts[1]), double.Parse(coordinateParts[0])));
            }

            return shape;
        }

        Match multiPolyMatch = Regex.Match(text, @"(?<type>\w+)\s*\(.*?(\(\((?<points>((\-?\d+(\.\d+)?\s+\-?\d+(\.\d+)?),?)+?)\)\),?)+\)", RegexOptions.IgnoreCase);
        if (multiPolyMatch.Success)
        {
            string geometryType = multiPolyMatch.Groups["type"].Value;

            if (geometryType == "MULTIPOLYGON")
            {
                Group pointsGroup = multiPolyMatch.Groups["points"];

                var multiCoordinates = new List<IList<Location>>(pointsGroup.Captures.Count);

                foreach (Capture capture in pointsGroup.Captures)
                {
                    string[] pointStrings = capture.Value.Split(',');

                    var coords = new List<Location>(pointStrings.Length * 2);
                    multiCoordinates.Add(coords);

                    for (int i = 0; i < pointStrings.Length; i++)
                    {
                        string[] coordinateParts = Regex.Split(pointStrings[i].Trim(), "\\s+");

                        coords.Add(new Location(double.Parse(coordinateParts[1]), double.Parse(coordinateParts[0])));
                    }
                }

                return new WktShape(WktShapeType.MultiPolygon, multiCoordinates);
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsupported shape type: {0}", geometryType));
            }
        }

        return null;
    }

    public static string ToPointText(Location pointCoordinate)
    {
        var point = new WktShape(WktShapeType.Point);
        point.Locations.Add(pointCoordinate);

        return ToText(point);
    }

    public static string ToLineStringText(IEnumerable<Location> lineStringCoordinates)
    {
        var shape = new WktShape(WktShapeType.LineString);
        foreach (Location coord in lineStringCoordinates)
        {
            shape.Locations.Add(coord);
        }

        return ToText(shape);
    }

    public static string ToPolygonText(IEnumerable<Location> polyCoordinates)
    {
        var shape = new WktShape(WktShapeType.Polygon);
        foreach (Location polyCoordinate in polyCoordinates)
        {
            shape.Locations.Add(polyCoordinate);
        }

        return ToText(shape);
    }

    public static string ToText(WktShape shape)
    {
        switch (shape.ShapeType)
        {
            case WktShapeType.Point:
                return string.Format("POINT({0} {1})", shape.Locations[0].Longitude, shape.Locations[0].Latitude);
            case WktShapeType.LineString:
                {
                    var wkt = new StringBuilder("LINESTRING(");

                    AppendWktPoints(shape, wkt);

                    wkt.Append(")");

                    return wkt.ToString();
                }
            case WktShapeType.Polygon:
                {
                    var wkt = new StringBuilder("POLYGON((");

                    AppendWktPoints(shape, wkt);

                    wkt.Append("))");

                    return wkt.ToString();
                }
            case WktShapeType.MultiPolygon:
                {
                    var wkt = new StringBuilder("MULTIPOLYGON(");

                    for (int i = 0; i < shape.MultiCoordinates.Count; i++)
                    {
                        if (i > 0)
                            wkt.Append(',');

                        wkt.Append("((");

                        AppendWktPoints(shape.MultiCoordinates[i], wkt);

                        wkt.Append("))");
                    }

                    wkt.Append(')');

                    return wkt.ToString();
                }
            default:
                throw new NotSupportedException();
        }
    }

    private static void AppendWktPoints(WktShape shape, StringBuilder wkt)
    {
        AppendWktPoints(shape.Locations, wkt);
    }

    private static void AppendWktPoints(IList<Location> coords, StringBuilder wkt)
    {
        for (int i = 0; i < coords.Count; i++)
        {
            if (i > 0) wkt.Append(',');

            Location coordinate = coords[i];

            wkt.Append($"{coordinate.Longitude} {coordinate.Latitude}");
        }
    }

    public static IList<Location> ParseCoordinatePart(string wkt)
    {
        var coords = new List<Location>();

        for (int i = 0; i < wkt.Length; i++)
        {
            char c = wkt[i];

            switch (c)
            {
                case '(':
                    break;
                case ')':
                    break;
            }
        }

        return coords;
    }
}

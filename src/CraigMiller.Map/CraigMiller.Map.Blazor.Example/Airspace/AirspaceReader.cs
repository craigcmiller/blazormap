using System.Reflection;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NodeDeserializers;

namespace CraigMiller.Map.Blazor.Example.Airspace;

public class AirspaceReader
{
    public static async Task<string> ReadAirspaceFromEmbeddedResource()
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CraigMiller.Map.Blazor.Example.Airspace.airspace.yaml");
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);

        return await reader.ReadToEndAsync();
    }

    public static async Task<AirspaceItem[]> AirspaceFromEmbeddedResource()
    {
        string yaml = await ReadAirspaceFromEmbeddedResource();

        var deserializer = new DeserializerBuilder()
            .WithNodeDeserializer(inner => new GeometryNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var airspace = deserializer.Deserialize<AirspaceRoot>(yaml);

        return airspace.Airspace;
    }
}

public record AirspaceRoot(AirspaceItem[] Airspace)
{
    public AirspaceRoot() : this([])
    {
    }
}

public record AirspaceItem()
{
    public required string Name { get; init; }

    public required string Id { get; init; }
    
    public string? Type { get; set; }

    public string? Class { get; set; }

    public required AirspaceGeomerty[] Geomerty { get; init; }
}

public record AirspaceGeomerty
{
    public required string Seq { get; init; }
    
    public required string? Upper { get; set; }

    public required string? Lower { get; set; }

    public required AirspaceGeometryBase[] Geometry { get; init; }
}

public abstract class AirspaceGeometryBase
{

}

public class GeometryNodeDeserializer : INodeDeserializer
{
    public GeometryNodeDeserializer(INodeDeserializer f)
    {
        F = f;
    }

    public INodeDeserializer F { get; }

    public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value, ObjectDeserializer rootDeserializer)
    {
        throw new NotImplementedException();
    }
}

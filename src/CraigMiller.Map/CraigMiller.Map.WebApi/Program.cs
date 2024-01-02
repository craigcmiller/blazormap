using CraigMiller.Map.WebApi;
using SkiaSharp;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.WebHost.UseKestrelHttpsConfiguration();

var app = builder.Build();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

RouteGroupBuilder mapGroupBuilder = app.MapGroup("/map");
// https://localhost:5207/map/1000,800/0,51/10
mapGroupBuilder.MapGet(
    "/{width},{height}/{lonCenter},{latCenter}/{zoom}", 
    async (HttpResponse response, int width, int height, double lonCenter, double latCenter, int zoom) =>
{
    using SKBitmap bitmap = MapImageGenerator.Generate(width, height, new CraigMiller.Map.Core.Geo.Location(latCenter, lonCenter), zoom);

    using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
    using Stream dataStream = data.AsStream();

    response.Headers.ContentType = "image/x-png";

    await dataStream.CopyToAsync(response.Body);
});

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

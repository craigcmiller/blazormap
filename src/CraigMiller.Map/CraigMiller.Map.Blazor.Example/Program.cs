using CraigMiller.Map.Blazor.Example;
using CraigMiller.Map.Blazor.Example.Airspace;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

try
{
    AirspaceItem[] airspaceItems = await AirspaceReader.AirspaceFromEmbeddedResource();
    foreach (AirspaceItem ai in airspaceItems)
    {
        Console.WriteLine(ai.Name);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddGeolocationServices();

await builder.Build().RunAsync();

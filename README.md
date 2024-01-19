# BlazorMap
BlazorMap is a map renderer Blazor WebAssembly component that utilises the SkiaSharp graphics API.

![blazormap](https://github.com/craigcmiller/blazormap/actions/workflows/dotnet.yml/badge.svg?branch=main)

## More information
* [Example site](https://craigcmiller.github.io/blazormap/)
* [Github](https://github.com/craigcmiller/blazormap)
* [NuGet](https://www.nuget.org/packages/CraigMiller.Map.Blazor/)

## Features
* Extensible layering system with a number of pre-built layers
  * WMTS tiles
  * Route
  * Background fill
  * Grid line
  * Scale
  * Marker
  * Compass
* Rotatable map
* Animation system including a number of predefined animations

## Getting started

### 1. Install NuGet package
```
dotnet add package CraigMiller.Map.Blazor
```

### 2. Make sure HttpClient is added to your DI services
Add the following to Program.cs (if you have not already done so)
```cs
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
```

### 3. Add a map to your Blazor component or page
```cs
<Map @ref="_map" Style="height:600px" />

@code {
    Map? _map;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _map!.AddDefaultLayers();
        }
    }
}
```

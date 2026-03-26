# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Learnings

When implementing non-trivial features, save learnings under `project/learnings/<category>/<area>.md` in the memory store (`~/.claude/projects/C--code-blazormap/memory/`) and add a pointer to `MEMORY.md`. Focus on decisions, gotchas, and patterns that aren't obvious from reading the code.

## Commands

All commands run from the solution root `src/CraigMiller.Map/`.

**Build:**
```bash
dotnet build src/CraigMiller.Map/CraigMiller.Map.sln
```

**Run tests:**
```bash
dotnet test src/CraigMiller.Map/CraigMiller.Map.Core.Tests
```

**Run a single test:**
```bash
dotnet test src/CraigMiller.Map/CraigMiller.Map.Core.Tests --filter "FullyQualifiedName~TestClassName"
```

**Run the example app:**
```bash
dotnet run --project src/CraigMiller.Map/CraigMiller.Map.Blazor.Example
# Available at https://localhost:7279 or http://localhost:5059
```

**Install WASM workload (required for Blazor WASM builds):**
```bash
dotnet workload install wasm-tools
```

## Architecture

This is a Blazor WebAssembly map rendering library using SkiaSharp for 2D graphics, targeting .NET 10.

### Projects

- **CraigMiller.Map.Core** — The rendering engine. No Blazor dependencies. Contains all map logic: coordinate conversion, tile fetching, layers, animations, touch tracking.
- **CraigMiller.Map.Blazor** — Razor component library wrapping Core. Provides the `<Map>` component that hosts a SkiaSharp `SKGLView` canvas. Handles JS interop (device pixel ratio), pointer events, and exposes `MapEngine` to consumers via a `RenderContext`.
- **CraigMiller.Map.Blazor.Example** — Blazor WASM demo app. Shows real usage patterns including GPX routes, airspace YAML, geolocation, and animations.
- **CraigMiller.Map.WebApi** — ASP.NET Core minimal API for server-side map image generation. Uses AOT compilation. Endpoint: `GET /map/{w}/{h}/{lon},{lat}/{zoom}`.
- **CraigMiller.Map.Core.Tests** — XUnit tests for Core logic.

### Core Engine (`CraigMiller.Map.Core`)

**`Engine/`** is the heart of the system:
- `MapEngine` — Central controller. Owns the viewport state (center, zoom, rotation), handles mouse/touch input (drag with inertia, pinch-zoom), manages layers and animations, and drives rendering via `CanvasRenderer`.
- `CanvasRenderer` — Calls each `ILayer.Render()` in order using the current `RenderContext`.
- `GeoConverter` — Converts geographic coordinates (lat/lon) ↔ pixel coordinates using Web Mercator projection. Also handles viewport rotation transforms.
- `RenderContext` — Passed to every layer during render. Contains the SKCanvas, GeoConverter, viewport dimensions, and current MapEngine reference.

**Layer system** — Layers implement `ILayer` (render-only) or `IDataLayer` (render + data binding). Key layers:
- `TileLayer` — Fetches/caches WMTS tiles (e.g. OpenStreetMap). Tiles load asynchronously and trigger re-renders.
- `RouteLayer` — Draws `Route` objects (lists of `Location` points) as polylines.
- `PolygonLayer` — Fills/strokes polygon regions.
- `CircleMarkerLayer` — Renders point markers.
- `CompassDataLayer`, `ScaleDataLayer`, `DiagnosticsDataLayer` — UI overlays drawn on top.
- `DurationAnimatedLayer` — Wraps any layer with time-based animation.

**Animation system** (`Animation/`) — Animations implement a step-based interface. Key types: `PanAnimation`, `ZoomAnimation`, `RotationAnimation`, `PanToLocationAnimation`, `CombinedAnimation`, `DeferredAnimation`. Rate functions (easing) are in `RateFunctions`.

**Coordinate types** (`Geo/`):
- `Location` — lat/lon geographic coordinate.
- `GeoRect` — bounding box in geographic space.
- `PartLocation` — point on a route with interpolated position.

### Data flow

```
User input (mouse/touch/pointer events)
    → Map.razor (Blazor component)
    → MapEngine (processes input, updates viewport or triggers animation)
    → CanvasRenderer.Render()
    → Each ILayer.Render(RenderContext)
    → SkiaSharp draws to SKGLView canvas
```

For tile layers, tile fetches happen on background threads; completion calls `MapEngine.RequestRender()` which triggers a Blazor `StateHasChanged()`.

### Adding a new layer

1. Implement `ILayer` (or `IDataLayer<T>`) in Core.
2. Override `Render(RenderContext ctx)` — use `ctx.GeoConverter` to convert locations to screen pixels, draw with `ctx.Canvas`.
3. Add the layer instance to `MapEngine.Layers` from the Blazor component or consumer code.

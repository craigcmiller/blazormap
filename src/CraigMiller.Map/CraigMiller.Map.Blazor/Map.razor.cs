using SkiaSharp.Views.Blazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Layers;
using CraigMiller.Map.Core.Layers.Tiling;

namespace CraigMiller.Map.Blazor;

public partial class Map : ComponentBase
{
    SKGLView? _view;
    readonly string _id = $"{nameof(Map).ToLower()}_{Guid.NewGuid().ToString().Replace("-", "")}";
    readonly MapEngine _engine;

    public Map()
    {
        _engine = new MapEngine();
    }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        await using MapJsInterop mapJsInterop = new(JSRuntime!);
        await mapJsInterop.FitToContainer(_id);

        if (CenterLatitude.HasValue && CenterLongitude.HasValue)
        {
            _engine.Center = new Location(CenterLatitude.Value, CenterLongitude.Value);
        }
    }

    public void AddDefaultLayers()
    {
        _engine.AddLayer(new BackgroundFillLayer());
        _engine.AddLayer(new TileLayer(new HttpTileLoader(HttpClient!)));
        _engine.AddLayer(new GridLineLayer());
        _engine.AddLayer(new ScaleLayer());
    }

    public void AddDebugLayers()
    {
        AddDefaultLayers();

        _engine.AddLayer(new CircleMarkerLayer
        {
            Locations = new List<Location> {
                new Location(51,0),
                new Location(80,-170),
                new Location(80,170),
                new Location(-80, -170),
                new Location(-80, 170)
            }
        });

        _engine.AddLayer(new DiagnosticsLayer());
    }

    public void AddMapLayer(ILayer layer) => _engine.AddLayer(layer);

    public async Task AddAsyncRenderMapLayer(params ILayer[] layers)
    {
        try
        {
            var asyncLayer = new AsynchronousLayer(layers);

            AddMapLayer(asyncLayer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public MapEngine Engine => _engine;

    private void OnPaintSurface(SKPaintGLSurfaceEventArgs paintEventArgs)
    {
        _engine.AreaView.CanvasWidth = paintEventArgs.Info.Width;
        _engine.AreaView.CanvasHeight = paintEventArgs.Info.Height;

        _engine.UpdateAnimations();

        _engine.Draw(paintEventArgs.Surface.Canvas);
    }

    private void OnMouseDown(MouseEventArgs args)
    {
        switch (args.Button)
        {
            case 0:
                _engine.PrimaryMouseDown(args.OffsetX, args.OffsetY);
                break;
        }
    }

    private void OnMouseUp(MouseEventArgs args)
    {
        _engine.PrimaryMouseUp(args.OffsetX, args.OffsetY);
    }

    private void OnMouseMove(MouseEventArgs args)
    {
        if (args.Button == 0)
        {
            _engine.PrimaryMouseMove(args.OffsetX, args.OffsetY);
        }
    }

    private void OnMouseWheel(WheelEventArgs args)
    {
        double zoomMultiplier = (args.DeltaY * -1.0) / 250.0 + 1.0;

        _engine.ZoomOn(args.OffsetX, args.OffsetY, zoomMultiplier, TimeSpan.FromMicroseconds(0.02));
    }

    private void OnDoubleClick(MouseEventArgs args)
    {
        _engine.ZoomOn(args.OffsetX, args.OffsetY, 2.0, TimeSpan.FromSeconds(0.5));
    }

    [Inject]
    public HttpClient? HttpClient { get; set; }

    [Inject]
    public IJSRuntime? JSRuntime { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (CenterLatitude.HasValue && CenterLongitude.HasValue)
        {
            _engine.Center = new Location(CenterLatitude.Value, CenterLongitude.Value);
        }
    }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public double? CenterLatitude { get; set; }

    [Parameter]
    public double? CenterLongitude { get; set; }
}

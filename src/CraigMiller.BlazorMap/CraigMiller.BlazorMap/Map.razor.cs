using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CraigMiller.BlazorMap.Engine;
using CraigMiller.BlazorMap.Layers;
using CraigMiller.BlazorMap.Layers.Tiling;

namespace CraigMiller.BlazorMap;

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
        //Console.WriteLine($"WV: {paintEventArgs.BackendRenderTarget.Width}, {paintEventArgs.BackendRenderTarget.Height} - {paintEventArgs.Info.Width} {paintEventArgs.Info.Height}");

        _engine.AreaView.CanvasWidth = paintEventArgs.Info.Width;
        _engine.AreaView.CanvasHeight = paintEventArgs.Info.Height;

        SKCanvas canvas = paintEventArgs.Surface.Canvas;
        //canvas.Scale(paintEventArgs.BackendRenderTarget.Width / paintEventArgs.Info.Width, paintEventArgs.BackendRenderTarget.Height / paintEventArgs.Info.Height);
        //canvas.Scale(paintEventArgs.Info.Width / paintEventArgs.BackendRenderTarget.Width, paintEventArgs.Info.Height / paintEventArgs.BackendRenderTarget.Height);

        _engine.UpdateAnimations();

        _engine.Draw(canvas);
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
        //Console.WriteLine($"Mouse: {args.OffsetX} {args.OffsetY}");

        if (args.Button == 0)
        {
            _engine.PrimaryMouseMove(args.OffsetX, args.OffsetY);
        }
    }

    private void OnMouseWheel(WheelEventArgs args)
    {
        _engine.ZoomOn(args.OffsetX, args.OffsetY, -args.DeltaY / 240d / 10d);
    }

    [Inject]
    public HttpClient? HttpClient { get; set; }

    [Inject]
    public IJSRuntime? JSRuntime { get; set; }

    [Parameter]
    public string? Style { get; set; }
}

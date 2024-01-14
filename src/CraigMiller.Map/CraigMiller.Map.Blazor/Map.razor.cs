using SkiaSharp.Views.Blazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Layers;
using CraigMiller.Map.Core.Layers.Tiling;
using CraigMiller.Map.Core.DataLayers;
using CraigMiller.Map.Core.Geo;
using SkiaSharp;

namespace CraigMiller.Map.Blazor;

public partial class Map : ComponentBase
{
    SKGLView? _view;
    readonly string _id = $"{nameof(Map).ToLower()}_{Guid.NewGuid().ToString().Replace("-", "")}";
    readonly MapEngine _engine;
    double _devicePixelRatio;

    public Map()
    {
        _engine = new MapEngine();
    }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();

        await using MapJsInterop mapJsInterop = new(JSRuntime!);

        _devicePixelRatio = await mapJsInterop.GetDevicePixelRatio();

        await mapJsInterop.DisableEventListeners(
            _id,
            "wheel",
            "contextmenu",
            "click",
            "dblclick",
            "pointerdown",
            "pointerup",
            "mousedown",
            "mouseup",
            "touchstart",
            "touchend",
            "touchmove",
            "touchcancel");

        await mapJsInterop.FitToContainer(_id);

        ElementBoundingRect boundingRect = await mapJsInterop.GetElementBoundingClientRect(_id);
        _engine.AreaView.CanvasWidth = boundingRect.Width;
        _engine.AreaView.CanvasHeight = boundingRect.Height;

        _engine.Zoom = Tile.GetZoomScale(InitalZoomLevel);
        _engine.Center = InitialLatitude.HasValue && InitialLongitude.HasValue ? new Location(InitialLatitude.Value, InitialLongitude.Value) : Location.NullIsland;
    }

    public void AddDefaultLayers()
    {
        _engine.AddLayer(new BackgroundFillLayer());
        _engine.AddLayer(new TileLayer(new HttpTileLoader(HttpClient!)));
        _engine.AddLayer(new GridLineLayer());
        _engine.AddDataLayer(new ScaleDataLayer());
        _engine.AddDataLayer(new CompassDataLayer());

        AddZoomButtons();
    }

    public void AddZoomButtons()
    {
        const float width = 26f;

        var zoomInButton = new ButtonInteractiveDataLayer
        {
            X = 10,
            Y = 30,
            Text = "+",
            Alpha = 200,
            MinWidth = width,
        };
        zoomInButton.Clicked += (_, _) =>
        {
            _engine.ZoomOn((_engine.AreaView.CanvasWidth - _engine.CanvasWidthOffset) / 2.0, (_engine.AreaView.CanvasHeight - _engine.CanvasHeightOffset) / 2.0, 2.0, TimeSpan.FromSeconds(1));
        };

        _engine.AddDataLayer(zoomInButton);

        var zoomOutButton = new ButtonInteractiveDataLayer
        {
            X = 10,
            Y = 65,
            Text = "-",
            Alpha = 200,
            MinWidth = width,
        };
        zoomOutButton.Clicked += (_, _) =>
        {
            _engine.ZoomOn((_engine.AreaView.CanvasWidth - _engine.CanvasWidthOffset) / 2.0, (_engine.AreaView.CanvasHeight - _engine.CanvasHeightOffset) / 2.0, 0.5, TimeSpan.FromSeconds(1));
        };

        _engine.AddDataLayer(zoomOutButton);
    }

    public void AddDebugLayers()
    {
        AddDefaultLayers();

        _engine.AddLayer(new CircleMarkerLayer
        {
            Locations = new List<Location> {
                new Location(51, 0),
                new Location(80, -170),
                new Location(80, 170),
                new Location(-80, -170),
                new Location(-80, 170)
            }
        });

        _engine.AddDataLayer(new DiagnosticsDataLayer());
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

    void OnPaintSurface(SKPaintGLSurfaceEventArgs paintEventArgs)
    {
        //Console.WriteLine($"{_devicePixelRatio} - {paintEventArgs.Info.Width}, {paintEventArgs.Info.Height}, {paintEventArgs.RawInfo.Width}, {paintEventArgs.RawInfo.Height}");

        _engine.AreaView.CanvasWidth = paintEventArgs.Info.Width / _devicePixelRatio;
        _engine.AreaView.CanvasHeight = paintEventArgs.Info.Height / _devicePixelRatio;

        SKCanvas canvas = paintEventArgs.Surface.Canvas;
        canvas.Scale((float)_devicePixelRatio);

        _engine.Draw(canvas);
    }

    void OnPointerDown(PointerEventArgs args)
    {
        switch (args.Button)
        {
            case 0:
                _engine.PrimaryMouseDown(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio);
                break;
        }
    }

    void OnPointerUp(PointerEventArgs args)
    {
        _engine.PrimaryMouseUp(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio);
    }

    void OnPointerMove(PointerEventArgs args)
    {
        _engine.PrimaryMouseMove(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio);
    }

    void OnMouseWheel(WheelEventArgs args)
    {
        double zoomMultiplier = (args.DeltaY * -1.0) / 250.0 + 1.0;

        _engine.ZoomOn(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio, zoomMultiplier, TimeSpan.FromSeconds(0.02));
    }

    void OnClick(MouseEventArgs args)
    {
        _engine.PrimaryMouseClick(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio);
    }

    void OnDoubleClick(MouseEventArgs args)
    {
        _engine.ZoomOn(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio, 2.0, TimeSpan.FromSeconds(0.5));
    }

    void OnContextMenu(MouseEventArgs args)
    {
        _engine.SecondaryMouseClick(args.OffsetX * _devicePixelRatio, args.OffsetY * _devicePixelRatio);
    }

    [Inject]
    public HttpClient? HttpClient { get; set; }

    [Inject]
    public IJSRuntime? JSRuntime { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        if (InitialLatitude.HasValue && InitialLongitude.HasValue)
        {
            _engine.Center = new Location(InitialLatitude.Value, InitialLongitude.Value);
        }
    }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public double? InitialLatitude { get; set; }

    [Parameter]
    public double? InitialLongitude { get; set; }

    [Parameter]
    public int InitalZoomLevel { get; set; } = 4;
}

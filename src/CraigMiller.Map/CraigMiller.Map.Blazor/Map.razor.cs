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
    ElementBoundingRect _boundingRect;

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
            //"pointerdown",
            //"pointerup",
            "mousedown",
            "mouseup",
            "touchstart",
            "touchend",
            "touchmove",
            "touchcancel");

        await mapJsInterop.FitToContainer(_id);

        _boundingRect = await mapJsInterop.GetElementBoundingClientRect(_id);
        _engine.AreaView.CanvasWidth = _boundingRect.Width;
        _engine.AreaView.CanvasHeight = _boundingRect.Height;

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
        _engine.AreaView.CanvasWidth = paintEventArgs.Info.Width / _devicePixelRatio;
        _engine.AreaView.CanvasHeight = paintEventArgs.Info.Height / _devicePixelRatio;

        SKCanvas canvas = paintEventArgs.Surface.Canvas;

        var paint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.StrokeAndFill
        };

        canvas.DrawLine(100, 100, 400, 100, paint);
        canvas.DrawLine(100, 120, 400, 120, paint);

        _engine.Draw(canvas, (float)_devicePixelRatio);
    }

    void OnMouseDown(MouseEventArgs args)
    {
        switch (args.Button)
        {
            case 0:
                _engine.PrimaryMouseDown(args.OffsetX , args.OffsetY);
                break;
        }
    }

    void OnMouseUp(MouseEventArgs args)
    {
        _engine.PrimaryMouseUp(args.OffsetX, args.OffsetY );
    }

    void OnMouseMove(MouseEventArgs args)
    {
        _engine.PrimaryMouseMove(args.OffsetX , args.OffsetY);
    }

    void OnMouseWheel(WheelEventArgs args)
    {
        double zoomMultiplier = (args.DeltaY * -1.0) / 250.0 + 1.0;

        _engine.ZoomOn(args.OffsetX, args.OffsetY, zoomMultiplier, TimeSpan.FromSeconds(0.02));
    }

    void OnClick(MouseEventArgs args)
    {
        _engine.PrimaryMouseClick(args.OffsetX , args.OffsetY);
    }

    void OnDoubleClick(MouseEventArgs args) => DoubleClickZoom(args.OffsetX , args.OffsetY );

    void DoubleClickZoom(double x, double y)
    {
        _engine.ZoomOn(x, y, 2.0, TimeSpan.FromSeconds(0.5));
    }

    void OnContextMenu(MouseEventArgs args)
    {
        _engine.SecondaryMouseClick(args.OffsetX , args.OffsetY );
    }

    readonly IDictionary<long, TouchTracking> _activeTouches = new Dictionary<long, TouchTracking>();
    readonly IList<TouchTracking> _recentTaps = new List<TouchTracking>();

    void OnTouchStart(TouchEventArgs args)
    {
        foreach (TouchPoint touchPoint in args.ChangedTouches)
        {
            _activeTouches.Add(touchPoint.Identifier, new TouchTracking(touchPoint));
        }

        if (args.Touches.Length == 1)
        {
            ToOffset(args.Touches[0].ClientX, args.Touches[0].ClientY, out double offsetX, out double offsetY);

            _engine.PrimaryMouseDown(offsetX, offsetY);
        }

        Console.WriteLine($"TS {args.Touches.Length} {args.TargetTouches.Length} {args.ChangedTouches.Length}, {args.Detail}, {_activeTouches.Count}");
    }

    void OnTouchEnd(TouchEventArgs args)
    {
        DateTime now = DateTime.UtcNow;

        foreach (TouchPoint touchPoint in args.ChangedTouches)
        {
            TouchTracking tracking = _activeTouches[touchPoint.Identifier];
            if ((now - tracking.DownTimestamp).TotalSeconds < TapIntervalSeconds
                && Math.Abs(tracking.DownPoint.ClientX - touchPoint.ClientX) < 3.0
                && Math.Abs(tracking.DownPoint.ClientY - touchPoint.ClientY) < 3.0)
            {
                ToOffset(tracking.DownPoint.ClientX, tracking.DownPoint.ClientY, out double offsetX, out double offsetY);

                if (_recentTaps.Any(rt => (now - rt.DownTimestamp).TotalSeconds < TapIntervalSeconds
                    && Math.Abs(rt.DownPoint.ClientX - touchPoint.ClientX) < 3.0
                    && Math.Abs(rt.DownPoint.ClientY - touchPoint.ClientY) < 3.0))
                {
                    DoubleClickZoom(offsetX, offsetY);
                }
                else
                {
                    _engine.PrimaryMouseClick(offsetX, offsetY);
                }

                _recentTaps.Add(new TouchTracking(touchPoint));
            }

            _activeTouches.Remove(touchPoint.Identifier);
        }

        if (args.Touches.Length == 0 && args.ChangedTouches.Length == 1)
        {
            ToOffset(args.ChangedTouches[0].ClientX, args.ChangedTouches[0].ClientY, out double offsetX, out double offsetY);

            _engine.PrimaryMouseUp(offsetX, offsetY);
        }

        // Discard all taps over 5 seconds old
        foreach (TouchTracking recentTap in _recentTaps.Where(tap => (now - tap.DownTimestamp).TotalSeconds > 5).ToArray())
        {
            _recentTaps.Remove(recentTap);
        }

        Console.WriteLine($"TE {args.Touches.Length} {args.TargetTouches.Length} {args.ChangedTouches.Length}, {args.Detail}, {_activeTouches.Count}");
    }

    void OnTouchMove(TouchEventArgs args)
    {
        Console.WriteLine($"TM {args.Touches.Length} {args.TargetTouches.Length} {args.ChangedTouches.Length}, {args.Detail}, {_activeTouches.Count}");

        if (args.ChangedTouches.Length == 1)
        {
            ToOffset(args.ChangedTouches[0].ClientX, args.ChangedTouches[0].ClientY, out double offsetX, out double offsetY);

            _engine.PrimaryMouseMove(offsetX, offsetY);
        }
    }

    void ToOffset(double clientX, double clientY, out double offsetX, out double offsetY)
    {
        offsetX = (clientX - _boundingRect.Left) ;
        offsetY = (clientY - _boundingRect.Top) ;
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

    [Parameter]
    public double TapIntervalSeconds { get; set; } = 0.5;
}

class TouchTracking
{
    public TouchTracking(TouchPoint downPoint)
    {
        DownPoint = downPoint;
        DownTimestamp = DateTime.UtcNow;
    }

    public TouchPoint DownPoint { get; }

    public DateTime DownTimestamp { get; }
}

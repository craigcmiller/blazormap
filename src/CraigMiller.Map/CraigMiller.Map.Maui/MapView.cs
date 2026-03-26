using CraigMiller.Map.Core.DataLayers;
using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;
using CraigMiller.Map.Core.Layers;
using CraigMiller.Map.Core.Layers.Tiling;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace CraigMiller.Map.Maui;

public class MapView : SKCanvasView
{
    readonly MapEngine _engine;
    readonly HttpClient _httpClient;
    IDispatcherTimer? _renderTimer;
    bool _setupInvoked;

    // Single-touch pan tracking
    bool _isTouching;
    SKPoint _touchDownPoint;
    DateTime _lastTapTime = DateTime.MinValue;
    SKPoint _lastTapPoint;

    // Multi-touch tracking for pinch zoom
    readonly Dictionary<long, SKPoint> _activeTouches = new();

    public MapView() : this(new HttpClient()) { }

    public MapView(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _engine = new MapEngine();
        EnableTouchEvents = true;
        Touch += OnTouch;
    }

    /// <summary>
    /// Invoked once after the first frame is rendered. Add custom layers or configure the engine here.
    /// If not set, default layers (background, tiles, grid, scale, compass, zoom buttons) are added automatically.
    /// </summary>
    public Action<MapView, MapEngine>? Setup { get; set; }

    public MapEngine Engine => _engine;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            if (Setup is null)
                AddDefaultLayers();

            _renderTimer = Dispatcher.CreateTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 fps
            _renderTimer.Tick += (_, _) => InvalidateSurface();
            _renderTimer.Start();
        }
        else
        {
            _renderTimer?.Stop();
            _renderTimer = null;
            _engine.Dispose();
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var info = e.Info;
        if (info.Width == 0 || info.Height == 0)
            return;

        float density = (float)DeviceDisplay.Current.MainDisplayInfo.Density;

        _engine.AreaView.CanvasWidth = info.Width / density;
        _engine.AreaView.CanvasHeight = info.Height / density;

        _engine.Draw(e.Surface.Canvas, density);

        if (!_setupInvoked && Setup is not null)
        {
            _setupInvoked = true;
            Setup(this, _engine);
        }
    }

    void OnTouch(object? sender, SKTouchEventArgs e)
    {
        float density = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
        float x = e.Location.X / density;
        float y = e.Location.Y / density;

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _activeTouches[e.Id] = e.Location;
                if (_activeTouches.Count == 1)
                {
                    _isTouching = true;
                    _touchDownPoint = e.Location;
                    _engine.PrimaryMouseDown(x, y);
                }
                break;

            case SKTouchAction.Moved:
                if (_activeTouches.ContainsKey(e.Id))
                {
                    if (_activeTouches.Count == 1)
                    {
                        _activeTouches[e.Id] = e.Location;
                        _engine.PrimaryMouseMove(x, y);
                    }
                    else if (_activeTouches.Count == 2)
                    {
                        var oldPoints = _activeTouches.Values.ToArray();
                        float oldDist = Distance(oldPoints[0], oldPoints[1]);

                        _activeTouches[e.Id] = e.Location;

                        var newPoints = _activeTouches.Values.ToArray();
                        float newDist = Distance(newPoints[0], newPoints[1]);

                        if (oldDist > 0)
                        {
                            double zoom = newDist / oldDist;
                            float cx = (newPoints[0].X + newPoints[1].X) / 2f / density;
                            float cy = (newPoints[0].Y + newPoints[1].Y) / 2f / density;
                            _engine.ZoomOn(cx, cy, zoom, TimeSpan.Zero);
                        }
                    }
                }
                break;

            case SKTouchAction.WheelChanged:
                // Match Blazor's wheel zoom feel: ~40% zoom per standard notch (±120 delta units)
                double zoomMultiplier = (e.WheelDelta * 1.0) / 300.0 + 1.0;
                _engine.ZoomOn(x, y, zoomMultiplier, TimeSpan.FromSeconds(0.15));
                break;

            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
                _activeTouches.Remove(e.Id);

                if (_activeTouches.Count == 0 && _isTouching)
                {
                    _isTouching = false;
                    _engine.PrimaryMouseUp(x, y);

                    // Detect tap (minimal movement from press point)
                    if (Math.Abs(e.Location.X - _touchDownPoint.X) < 10 &&
                        Math.Abs(e.Location.Y - _touchDownPoint.Y) < 10)
                    {
                        var now = DateTime.UtcNow;
                        bool isDoubleTap = (now - _lastTapTime).TotalSeconds < 0.4
                            && Math.Abs(e.Location.X - _lastTapPoint.X) < 20
                            && Math.Abs(e.Location.Y - _lastTapPoint.Y) < 20;

                        if (isDoubleTap)
                            _engine.ZoomOn(x, y, 2.0, TimeSpan.FromSeconds(0.5));
                        else
                            _engine.PrimaryMouseClick(x, y);

                        _lastTapTime = now;
                        _lastTapPoint = e.Location;
                    }
                }
                break;
        }

        e.Handled = true;
    }

    public void AddDefaultLayers()
    {
        _engine.AddLayer(new BackgroundFillLayer());

        var tileLayerHttpClient = new HttpClient();
        tileLayerHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/122.0.0.0 Safari/537.36");
        tileLayerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        tileLayerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-GB,en;q=0.9");
        tileLayerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
        tileLayerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
        tileLayerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

        _engine.AddLayer(new TileLayer(new HttpTileLoader(tileLayerHttpClient)));
        _engine.AddLayer(new GridLineLayer());
        _engine.AddDataLayer(new ScaleDataLayer());
        _engine.AddDataLayer(new CompassDataLayer());

        AddZoomButtons();
    }

    public void AddZoomButtons()
    {
        const float width = 44f; // Larger tap targets on mobile

        var zoomInButton = new ButtonInteractiveDataLayer
        {
            X = 10,
            Y = 30,
            Text = "+",
            Alpha = 200,
            MinWidth = width,
        };
        zoomInButton.Clicked += (_, _) =>
            _engine.ZoomOn(CenterX, CenterY, 2.0, TimeSpan.FromSeconds(1));

        _engine.AddDataLayer(zoomInButton);

        var zoomOutButton = new ButtonInteractiveDataLayer
        {
            X = 10,
            Y = 82,
            Text = "-",
            Alpha = 200,
            MinWidth = width,
        };
        zoomOutButton.Clicked += (_, _) =>
            _engine.ZoomOn(CenterX, CenterY, 0.5, TimeSpan.FromSeconds(1));

        _engine.AddDataLayer(zoomOutButton);
    }

    double CenterX => (_engine.AreaView.CanvasWidth - _engine.CanvasWidthOffset) / 2.0;

    double CenterY => (_engine.AreaView.CanvasHeight - _engine.CanvasHeightOffset) / 2.0;

    static float Distance(SKPoint a, SKPoint b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}

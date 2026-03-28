using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace CraigMiller.Map.Maui;

public class MapGlView : SKGLView
{
    readonly MauiMap _map;

    public MapGlView()
    {
        _map = new MauiMap();

        EnableTouchEvents = true;
        Touch += _map.OnTouch;
    }

    public MauiMap Map => _map;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        _map.OnHandlerChanged(Handler, Dispatcher, InvalidateSurface);
    }

    protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
        _map.OnPaintSurface(e.Info, e.Surface.Canvas);
    }
}

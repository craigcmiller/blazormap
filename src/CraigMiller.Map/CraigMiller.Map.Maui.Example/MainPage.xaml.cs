using CraigMiller.Map.Core.Animation;
using CraigMiller.Map.Core.Geo;
using CraigMiller.Map.Core.Layers.Tiling;

namespace CraigMiller.Map.Maui.Example;

using Location = CraigMiller.Map.Core.Geo.Location;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        MapView.Map.AddDefaultLayers();

        MapView.Map.Setup = (_, engine) =>
        {
            engine.Center = new Location(51.5, -0.1); // London
            engine.Zoom = Tile.GetZoomScale(5);
        };
    }

    void OnLondonClicked(object? sender, EventArgs e) =>
        NavigateTo(new Location(51.505, -0.09), 12);

    void OnNewYorkClicked(object? sender, EventArgs e) =>
        NavigateTo(new Location(40.712, -74.006), 12);

    void OnSydneyClicked(object? sender, EventArgs e) =>
        NavigateTo(new Location(-33.868, 151.209), 12);

    void OnWorldClicked(object? sender, EventArgs e) =>
        NavigateTo(new Location(20.0, 0.0), 3);

    void NavigateTo(Location location, int zoomLevel)
    {
        var engine = MapView.Map.Engine;

        engine.ClearAnimations();
        engine.EnqueueAnimation(new PanToLocationAnimation(location, TimeSpan.FromSeconds(1.5), RateFunctions.AccelerateDecelerate));
        engine.EnqueueAnimation(new ZoomAnimation(Tile.GetZoomScale(zoomLevel), TimeSpan.FromSeconds(1), RateFunctions.AccelerateDecelerate));
    }
}

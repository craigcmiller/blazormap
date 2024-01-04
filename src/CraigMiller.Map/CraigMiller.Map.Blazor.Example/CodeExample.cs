static class CodeExample
{
    public const string Simple = """
        <Map @ref="_map" Style="height:500px" />

        @code {
            Map? _map;

            protected override void OnAfterRender(bool firstRender)
            {
                if (firstRender)
                {
                    _map!.AddDefaultLayers();

                    _map!.Engine.Zoom = Tile.GetZoomScale(2);

                    _map!.Engine.EnqueueAnimation(
                        new DeferredAnimation(
                            new CombinedAnimation(
                                new ZoomAnimation(Tile.GetZoomScale(8), TimeSpan.FromSeconds(2), RateFunctions.Linear),
                                new PanToLocationAnimation(new Core.Geo.Location(51, 0), TimeSpan.FromSeconds(2), RateFunctions.AccelerateDecelerate)),
                        TimeSpan.FromSeconds(1)));
                }
            }
        }
        """;

    public const string MyLocation = """
        <Map @ref="_map" Style="height:500px" />

        <p class="alert alert-info mt-3">Location: @_lastLocationStr</p>

        @code {
            Map? _map;
            string _lastLocationStr = "Waiting...";

            protected override void OnAfterRender(bool firstRender)
            {
                if (firstRender)
                {
                    _map!.AddDefaultLayers();

                    _map!.Engine.Zoom = Tile.GetZoomScale(7);

                    if (GeolocationService is not null)
                    {
                        GeolocationService.WatchPosition(pos =>
                        {
                            Location location = new Location(pos.Coords.Latitude, pos.Coords.Longitude);

                            _map!.Engine.EnqueueAnimation(new PanToLocationAnimation(location, TimeSpan.FromSeconds(2), RateFunctions.AccelerateDecelerate));

                            var circleMarkerLayer = new CircleMarkerLayer
                            {
                                Locations = [location]
                            };
                            DurationAnimatedLayer<CircleMarkerLayer> animatedCircleMarker = CircleMarkerLayer.CreateAnimated(circleMarkerLayer, TimeSpan.FromSeconds(1), 4, 24);
                            animatedCircleMarker.MaxRepetitions = int.MaxValue;

                            _map!.Engine.RemoveLayers<DurationAnimatedLayer<CircleMarkerLayer>>();
                            _map!.Engine.AddLayer(animatedCircleMarker);

                            _lastLocationStr = location.ToDegreesMinutesSecondsString();
                            StateHasChanged();
                        });
                    }
                }
            }

            [Inject]
            IGeolocationService? GeolocationService { get; set; }
        }
        """;
}

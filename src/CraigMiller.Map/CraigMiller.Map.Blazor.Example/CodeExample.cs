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
}

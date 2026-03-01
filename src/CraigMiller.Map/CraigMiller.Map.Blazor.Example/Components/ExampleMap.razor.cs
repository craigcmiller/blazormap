using CraigMiller.Map.Core.Animation;
using CraigMiller.Map.Core.Geo;
using CraigMiller.Map.Core.Layers;
using CraigMiller.Map.Core.Routes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NetTopologySuite.IO;
using System.Xml;

namespace CraigMiller.Map.Blazor.Example.Components
{
    public partial class ExampleMap
    {
        void ClearRoute()
        {
            Map!.Engine.GetLayers<RouteLayer>().FirstOrDefault()?.Route.Clear();
        }

        void NorthToTop()
        {
            Map!.Engine.EnqueueAnimation(new RotationAnimation(TimeSpan.FromSeconds(1), 0, RateFunctions.AccelerateDecelerate));
        }

        public Map? Map { get; private set; }

        async Task LoadFile(InputFileChangeEventArgs e)
        {
            using Stream fileStream = e.File.OpenReadStream();
            using MemoryStream memoryStream = new();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            GpxFile gpxFile = GpxFile.ReadFrom(XmlReader.Create(memoryStream), new GpxReaderSettings());

            GpxRoute? gpxRoute = gpxFile.Routes.FirstOrDefault();
            if (gpxRoute is null)
            {
                return;
            }

            RouteLayer? routeLayer = Map!.Engine.GetLayers<RouteLayer>().FirstOrDefault();
            if (routeLayer is null)
            {
                Map!.Engine.AddLayer(routeLayer = new RouteLayer());
            }

            routeLayer.Route.Clear();

            IEnumerable<Location> routeWaypoints = gpxRoute.Waypoints.Select(gpxWaypoint => new Location(gpxWaypoint.Latitude.Value, gpxWaypoint.Longitude.Value));
            routeLayer.Route.AddWaypoints(routeWaypoints.Select(loc => new Waypoint(loc)));

            Map!.Engine.EnqueueAnimation(new PanToAreaAnimation(GeoRect.EncompassingLocations(routeWaypoints), TimeSpan.FromSeconds(3), RateFunctions.AccelerateDecelerate));
        }

        [Parameter]
        public string? SourceLink { get; set; }
    }
}

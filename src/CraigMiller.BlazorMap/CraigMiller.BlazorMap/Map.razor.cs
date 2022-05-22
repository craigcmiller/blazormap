using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CraigMiller.BlazorMap.Engine;
using CraigMiller.BlazorMap.Layers;
using CraigMiller.BlazorMap.Layers.Tiling;

namespace CraigMiller.BlazorMap
{
    public partial class Map
    {
        private SKGLView? _view;
        private readonly string _id = $"{nameof(Map).ToLower()}_{Guid.NewGuid().ToString().Replace("-", "")}";
        private readonly MapEngine _map;

        public Map()
        {
            _map = new MapEngine();
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            AddMapLayers();

            await using MapJsInterop mapJsInterop = new(JSRuntime!);
            await mapJsInterop.FitToContainer(_id);
        }

        private void AddMapLayers()
        {
            _map.AddLayer(new BackgroundFillLayer());

            _map.AddLayer(new TileLayer(new HttpTileLoader(HttpClient!)));

            _map.AddLayer(new GridLineLayer());

            _map.AddLayer(new CircleMarkerLayer
            {
                Locations = new List<Location> {
                    new Location(51,0),
                    new Location(80,-170),
                    new Location(80,170),
                    new Location(-80, -170),
                    new Location(-80, 170)
                }
            });

            _map.AddLayer(new DiagnosticsLayer());
        }

        public MapEngine Engine => _map;

        private void OnPaintSurface(SKPaintGLSurfaceEventArgs paintEventArgs)
        {
            //Console.WriteLine($"WV: {paintEventArgs.BackendRenderTarget.Width}, {paintEventArgs.BackendRenderTarget.Height} - {paintEventArgs.Info.Width} {paintEventArgs.Info.Height}");

            _map.AreaView.CanvasWidth = paintEventArgs.Info.Width;
            _map.AreaView.CanvasHeight = paintEventArgs.Info.Height;

            SKCanvas canvas = paintEventArgs.Surface.Canvas;
            //canvas.Scale(paintEventArgs.BackendRenderTarget.Width / paintEventArgs.Info.Width, paintEventArgs.BackendRenderTarget.Height / paintEventArgs.Info.Height);
            //canvas.Scale(paintEventArgs.Info.Width / paintEventArgs.BackendRenderTarget.Width, paintEventArgs.Info.Height / paintEventArgs.BackendRenderTarget.Height);

            _map.Paint(canvas);
        }

        private void OnMouseDown(MouseEventArgs args)
        {
            switch (args.Button)
            {
                case 0:
                    _map.PrimaryMouseDown(args.OffsetX, args.OffsetY);
                    break;
            }
        }

        private void OnMouseUp(MouseEventArgs args)
        {
            _map.PrimaryMouseUp();
        }

        private void OnMouseMove(MouseEventArgs args)
        {
            //Console.WriteLine($"Mouse: {args.OffsetX} {args.OffsetY}");

            if (args.Button == 0)
            {
                _map.PrimaryMouseMove(args.OffsetX, args.OffsetY);
            }
        }

        private void OnMouseWheel(WheelEventArgs args)
        {
            _map.ZoomOn(args.OffsetX, args.OffsetY, -args.DeltaY / 240d / 20d);
        }

        [Inject]
        public HttpClient? HttpClient { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
    }
}

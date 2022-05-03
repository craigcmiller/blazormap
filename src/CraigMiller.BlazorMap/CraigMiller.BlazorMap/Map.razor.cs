using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CraigMiller.BlazorMap.Engine;

namespace CraigMiller.BlazorMap
{
    public partial class Map
    {
        private SKGLView? _view;
        private readonly string _id = $"{nameof(Map).ToLower()}_{Guid.NewGuid().ToString().Replace("-", "")}";
        private readonly GeoConverter _worldView;
        private bool _isDragging;
        private PointD _previousMousePosition;
        private DateTime _previousMouseDragTime;

        public Map()
        {
            _worldView = new GeoConverter(new SmcProjection())
            {
                ProjectedX = SmcProjection.WorldMin,
                ProjectedY = SmcProjection.WorldMin,
                Zoom = 0.0001
            };
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            await using MapJsInterop mapJsInterop = new(JSRuntime!);
            await mapJsInterop.FitToContainer(_id);
        }

        private void OnPaintSurface(SKPaintGLSurfaceEventArgs paintEventArgs)
        {
            //Console.WriteLine($"WV: {paintEventArgs.BackendRenderTarget.Width}, {paintEventArgs.BackendRenderTarget.Height} - {paintEventArgs.Info.Width} {paintEventArgs.Info.Height}");

            _worldView.CanvasWidth = paintEventArgs.Info.Width;
            _worldView.CanvasHeight = paintEventArgs.Info.Height;

            SKCanvas canvas = paintEventArgs.Surface.Canvas;
            //canvas.Scale(paintEventArgs.BackendRenderTarget.Width / paintEventArgs.Info.Width, paintEventArgs.BackendRenderTarget.Height / paintEventArgs.Info.Height);
            //canvas.Scale(paintEventArgs.Info.Width / paintEventArgs.BackendRenderTarget.Width, paintEventArgs.Info.Height / paintEventArgs.BackendRenderTarget.Height);
            canvas.Clear(SKColors.LightBlue);

            using var gridPaint = new SKPaint
            {
                Color = SKColors.DarkGray,
                StrokeWidth = 2f,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Draw latitude grid lines
            for (double lat = -80; lat <= 80; lat += 10)
            {
                _worldView.LatLonToCanvas(lat, 0, out _, out float cnvY);

                canvas.DrawLine(0, cnvY, (float)_worldView.CanvasWidth, cnvY, gridPaint);
            }

            // Draw longitude grid lines
            for (double lon = -180; lon <= 180; lon += 10)
            {
                _worldView.LatLonToCanvas(0, lon, out float cnvX, out _);

                canvas.DrawLine(cnvX, 0, cnvX, (float)_worldView.CanvasHeight, gridPaint);
            }

            DrawMarker(canvas, 51, 0);
            DrawMarker(canvas, 80, -170);
            DrawMarker(canvas, 80, 170);
            DrawMarker(canvas, -80, -170);
            DrawMarker(canvas, -80, 170);
        }

        private void DrawMarker(SKCanvas canvas, double lat, double lon)
        {
            _worldView.LatLonToCanvas(lat, lon, out float x, out float y);
            canvas.DrawCircle(x, y, 10f, new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3f
            });
        }

        private void OnMouseDown(MouseEventArgs args)
        {
            switch (args.Button)
            {
                case 0:
                    _isDragging = true;
                    _previousMousePosition = new PointD(args.OffsetX, args.OffsetY);
                    _previousMouseDragTime = DateTime.UtcNow;

                    break;
            }
        }

        private void OnMouseUp(MouseEventArgs args)
        {
            _isDragging = false;
        }

        private void OnMouseMove(MouseEventArgs args)
        {
            //Console.WriteLine($"Mouse: {args.OffsetX} {args.OffsetY}");

            if (_isDragging && args.Button == 0)
            {
                double mouseX = args.OffsetX;
                double mouseY = args.OffsetY;

                double xDiff = mouseX - _previousMousePosition.X;
                double yDiff = mouseY - _previousMousePosition.Y;

                _worldView.CanvasToProjected(0d, 0d, out double projectedLeft, out double projectedTop);
                _worldView.CanvasToProjected(xDiff, yDiff, out double projectedDiffX, out double projectedDiffY);

                _worldView.ProjectedX -= projectedDiffX - projectedLeft;
                _worldView.ProjectedY -= projectedDiffY - projectedTop;

                //const currentDragTime = (new Date()).getTime();

                //const dragTimeDelta = (currentDragTime - this.previousMouseDragTime) / 1000.0 * this.inertialFramePerSec;

                // Store mouse deltas for inertial pan
                //this.inertialX = (mousePosition.x - this.previousMousePosition.x) * dragTimeDelta;
                //this.inertialY = (mousePosition.y - this.previousMousePosition.y) * dragTimeDelta;

                _previousMousePosition = new PointD(mouseX, mouseY);
                //_previousMouseDragTime = currentDragTime;
            }
        }

        private void OnMouseWheel(WheelEventArgs args)
        {
            Console.WriteLine($"{args.OffsetX} {args.OffsetY}");

            //_worldView.CanvasToProjected(0.0, 0.0, out double left, out double top);

            // Record the projected position of the mouse
            _worldView.CanvasToProjected(args.OffsetX, args.OffsetY, out double projectedMouseX, out double projectedMouseY);

            // Change the zoom level
            _worldView.Zoom += (-args.DeltaY / 240d / 20d) * _worldView.Zoom;

            // Get where the mouse now is
            _worldView.CanvasToProjected(args.OffsetX, args.OffsetY, out double offsetPrjX, out double offsetPrjY);

            _worldView.ProjectedX = projectedMouseX + (projectedMouseX - offsetPrjX);
            _worldView.ProjectedY = projectedMouseY - (projectedMouseY - offsetPrjY);
        }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
    }
}

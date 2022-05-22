using SkiaSharp;

namespace CraigMiller.BlazorMap.Engine
{
    public class MapEngine
    {
        private readonly GeoConverter _areaView;
        private bool _isDragging;
        private PointD _previousMousePosition;
        private DateTime _previousMouseDragTime;

        public MapEngine()
        {
            _areaView = new GeoConverter(new SmcProjection())
            {
                ProjectedLeft = SmcProjection.WorldMin,
                ProjectedBottom = SmcProjection.WorldMin,
                Zoom = 0.0001
            };

            Layers = new List<RenderableLayer>();
        }

        public GeoConverter AreaView => _areaView;

        public IList<RenderableLayer> Layers { get; }

        public void AddLayer(ILayer layer) => Layers.Add(new RenderableLayer(layer));

        public void Paint(SKCanvas canvas)
        {
            foreach (RenderableLayer renderableLayer in Layers)
            {
                if (renderableLayer.ShouldRender)
                {
                    renderableLayer.Layer.DrawLayer(canvas, _areaView);
                }
            }
        }

        public void PrimaryMouseDown(double x, double y)
        {
            _isDragging = true;
            _previousMousePosition = new PointD(x, y);
            _previousMouseDragTime = DateTime.UtcNow;
        }

        public void PrimaryMouseUp()
        {
            _isDragging = false;
        }

        public void PrimaryMouseMove(double x, double y)
        {
            if (_isDragging)
            {
                double mouseX = x;
                double mouseY = y;

                double xDiff = mouseX - _previousMousePosition.X;
                double yDiff = mouseY - _previousMousePosition.Y;

                _areaView.CanvasToProjected(0d, 0d, out double projectedLeft, out double projectedTop);
                _areaView.CanvasToProjected(xDiff, yDiff, out double projectedDiffX, out double projectedDiffY);

                _areaView.ProjectedLeft -= projectedDiffX - projectedLeft;
                _areaView.ProjectedBottom -= projectedDiffY - projectedTop;

                //const currentDragTime = (new Date()).getTime();

                //const dragTimeDelta = (currentDragTime - this.previousMouseDragTime) / 1000.0 * this.inertialFramePerSec;

                // Store mouse deltas for inertial pan
                //this.inertialX = (mousePosition.x - this.previousMousePosition.x) * dragTimeDelta;
                //this.inertialY = (mousePosition.y - this.previousMousePosition.y) * dragTimeDelta;

                _previousMousePosition = new PointD(mouseX, mouseY);
                //_previousMouseDragTime = currentDragTime;
            }
        }

        /// <summary>
        /// Zooms keeping canvas point at <paramref name="x"/> and <paramref name="y"/> in the same position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoomBy"></param>
        public void ZoomOn(double x, double y, double zoomBy)
        {
            // Record the projected position of the mouse
            _areaView.CanvasToProjected(x, y, out double projectedMouseX, out double projectedMouseY);

            // Change the zoom level
            _areaView.Zoom += zoomBy * AreaView.Zoom;

            // Get where the mouse now is
            _areaView.CanvasToProjected(x, y, out double offsetPrjX, out double offsetPrjY);

            // Move the projected position to keep the mouse position at the same location
            _areaView.ProjectedLeft += projectedMouseX - offsetPrjX;
            _areaView.ProjectedBottom += projectedMouseY - offsetPrjY;
        }

        public Location Center
        {
            get => _areaView.CenterLocation;
            set => _areaView.CenterLocation = value;
        }

        public double Zoom
        {
            get => _areaView.Zoom;
            set => _areaView.Zoom = value;
        }
    }
}

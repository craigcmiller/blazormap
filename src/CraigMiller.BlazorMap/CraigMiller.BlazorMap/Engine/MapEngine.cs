namespace CraigMiller.BlazorMap.Engine
{
    /// <summary>
    /// Canvas renderer with mouse interaction
    /// </summary>
    public class MapEngine : CanvasRenderer
    {
        private bool _isDragging;
        private PointD _previousMousePosition;
        private DateTime _previousMouseDragTime;

        public MapEngine()
        {
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

                AreaView.CanvasToProjected(0d, 0d, out double projectedLeft, out double projectedTop);
                AreaView.CanvasToProjected(xDiff, yDiff, out double projectedDiffX, out double projectedDiffY);

                AreaView.ProjectedLeft -= projectedDiffX - projectedLeft;
                AreaView.ProjectedBottom -= projectedDiffY - projectedTop;

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
            AreaView.CanvasToProjected(x, y, out double projectedMouseX, out double projectedMouseY);

            // Change the zoom level
            AreaView.Zoom += zoomBy * AreaView.Zoom;

            // Get where the mouse now is
            AreaView.CanvasToProjected(x, y, out double offsetPrjX, out double offsetPrjY);

            // Move the projected position to keep the mouse position at the same location
            AreaView.ProjectedLeft += projectedMouseX - offsetPrjX;
            AreaView.ProjectedBottom += projectedMouseY - offsetPrjY;
        }
    }
}

using System.Threading;

namespace CraigMiller.BlazorMap.Engine
{
    /// <summary>
    /// Canvas renderer with mouse interaction
    /// </summary>
    public class MapEngine : CanvasRenderer
    {
        bool _isDragging;
        PanPosition _lastMousePosition, _lastMousePositionForInertiaCalculation;
        bool _isInertialPanning;
        DateTime _lastInertialPanUpdate;
        double _inertialXPerSecond, _inertialYPerSecond;

        record struct PanPosition(double X, double Y, DateTime Timestamp);

        public MapEngine()
        {
        }

        public void PrimaryMouseDown(double x, double y)
        {
            _isInertialPanning = false;
            _isDragging = true;
            _lastMousePosition = _lastMousePositionForInertiaCalculation = new PanPosition(x, y, DateTime.UtcNow);
        }

        public void PrimaryMouseUp(double x, double y)
        {
            _isDragging = false;

            double secondsDelta = (DateTime.UtcNow - _lastMousePosition.Timestamp).TotalSeconds;

            if (secondsDelta < 0.02 && (Math.Abs(_inertialXPerSecond) > 0.1 || Math.Abs(_inertialYPerSecond) > 0.1))
            {
                _isInertialPanning = true;
                _lastInertialPanUpdate = _lastMousePosition.Timestamp;

                _inertialXPerSecond *= 0.5;
                _inertialYPerSecond *= 0.5;
            }
        }

        public void PrimaryMouseMove(double mouseX, double mouseY)
        {
            if (_isDragging)
            {
                double xDiff = mouseX - _lastMousePosition.X;
                double yDiff = mouseY - _lastMousePosition.Y;

                AreaView.MoveBy(xDiff, yDiff);

                DateTime now = DateTime.UtcNow;

                UpdateInertialPanSpeed(mouseX, mouseY, now);

                _lastMousePosition = new PanPosition(mouseX, mouseY, now);
            }
        }

        private void UpdateInertialPanSpeed(double mouseX, double mouseY, DateTime now)
        {
            double secondsDelta = (now - _lastMousePositionForInertiaCalculation.Timestamp).TotalSeconds;
            double xDelta = mouseX - _lastMousePositionForInertiaCalculation.X;
            double yDelta = mouseY - _lastMousePositionForInertiaCalculation.Y;

            _inertialXPerSecond = xDelta / secondsDelta;
            _inertialYPerSecond = yDelta / secondsDelta;

            //Console.WriteLine($"{_inertialXPerSecond}, {_inertialYPerSecond}, {secondsDelta}, {xDelta}, {yDelta}");

            if ((now - _lastMousePositionForInertiaCalculation.Timestamp).TotalSeconds > 0.05)
            {
                _lastMousePositionForInertiaCalculation = _lastMousePosition;
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

        public void InertialPanUpdateScene()
        {
            if (_isInertialPanning)
            {
                DateTime now = DateTime.UtcNow;
                double secondsSinceLastPan = (now - _lastInertialPanUpdate).TotalSeconds;

                AreaView.MoveBy(_inertialXPerSecond * secondsSinceLastPan, _inertialYPerSecond * secondsSinceLastPan);

                _inertialXPerSecond *= 0.9;
                _inertialYPerSecond *= 0.9;

                if (Math.Abs(_inertialXPerSecond) < 0.1 && Math.Abs(_inertialYPerSecond) < 0.1)
                {
                    _isInertialPanning = false;
                    _lastInertialPanUpdate = now;
                }
            }
        }
    }
}

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
        DateTime _lastInertialPanSetTime, _lastInertialPanUpdate;
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

            DateTime now = DateTime.UtcNow;

            double secondsSinceLastMouseMove = (now - _lastMousePosition.Timestamp).TotalSeconds;

            if (secondsSinceLastMouseMove < 0.02)
            {
                double xDelta = x - _lastMousePositionForInertiaCalculation.X;
                double yDelta = y - _lastMousePositionForInertiaCalculation.Y;

                double secondsSinceLastPanRecord = (now - _lastMousePositionForInertiaCalculation.Timestamp).TotalSeconds;

                _inertialXPerSecond = xDelta / secondsSinceLastPanRecord;
                _inertialYPerSecond = yDelta / secondsSinceLastPanRecord;

                if (Math.Abs(_inertialXPerSecond) > 0.1 || Math.Abs(_inertialYPerSecond) > 0.1)
                {
                    _isInertialPanning = true;
                    _lastInertialPanUpdate = _lastInertialPanSetTime = now;
                }
            }
        }

        public void PrimaryMouseMove(double x, double y)
        {
            if (_isDragging)
            {
                double xDiff = x - _lastMousePosition.X;
                double yDiff = y - _lastMousePosition.Y;

                AreaView.MoveBy(xDiff, yDiff);

                DateTime now = DateTime.UtcNow;

                UpdateInertialPanSpeed(now);

                _lastMousePosition = new PanPosition(x, y, now);
            }
        }

        private void UpdateInertialPanSpeed(DateTime now)
        {
            if ((now - _lastMousePositionForInertiaCalculation.Timestamp).TotalSeconds > 0.1)
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
                double secondsSinceLastPan = (now - _lastInertialPanSetTime).TotalSeconds;
                double ratioOfPan = 1.0 - (secondsSinceLastPan / InertialPanPeriod.TotalSeconds);

                // If ratio of pan is under zero then stop the pan
                if (ratioOfPan < 0.0)
                {
                    _isInertialPanning = false;
                    _lastInertialPanSetTime = now;
                }
                else
                {
                    double secondsSinceLastSceneUpdate = (now - _lastInertialPanUpdate).TotalSeconds;

                    double moveXBy = _inertialXPerSecond * secondsSinceLastSceneUpdate * ratioOfPan;
                    double moveYBy = _inertialYPerSecond * secondsSinceLastSceneUpdate * ratioOfPan;

                    AreaView.MoveBy(moveXBy, moveYBy);

                    _lastInertialPanUpdate = now;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time an inertial pan happens for
        /// </summary>
        public TimeSpan InertialPanPeriod { get; set; } = TimeSpan.FromSeconds(1.0);
    }
}

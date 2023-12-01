using CraigMiller.Map.Core.Animation;

namespace CraigMiller.Map.Core.Engine
{
    /// <summary>
    /// Canvas renderer with mouse interaction
    /// </summary>
    public class MapEngine : CanvasRenderer
    {
        bool _isDragging;
        PanPosition _lastMousePosition, _lastMousePositionForInertiaCalculation;
        MapAnimation? _activeAnimation;
        readonly Queue<MapAnimation> _animations = new Queue<MapAnimation>();

        record struct PanPosition(double X, double Y, DateTime Timestamp);

        public MapEngine()
        {
        }

        public void PrimaryMouseDown(double x, double y)
        {
            _isDragging = true;
            _lastMousePosition = _lastMousePositionForInertiaCalculation = new PanPosition(x, y, DateTime.UtcNow);

            _activeAnimation = null;
            _animations.Clear();
        }

        public void PrimaryMouseUp(double x, double y)
        {
            _isDragging = false;

            CreateInertialPanAnimation(x, y);
        }

        private void CreateInertialPanAnimation(double x, double y)
        {
            DateTime now = DateTime.UtcNow;

            double secondsSinceLastMouseMove = (now - _lastMousePosition.Timestamp).TotalSeconds;

            if (secondsSinceLastMouseMove < 0.02)
            {
                double xDelta = x - _lastMousePositionForInertiaCalculation.X;
                double yDelta = y - _lastMousePositionForInertiaCalculation.Y;

                double secondsSinceLastPanRecord = (now - _lastMousePositionForInertiaCalculation.Timestamp).TotalSeconds;

                double inertialXPerSecond = xDelta / secondsSinceLastPanRecord;
                double inertialYPerSecond = yDelta / secondsSinceLastPanRecord;

                if (Math.Abs(inertialXPerSecond) > 0.1 || Math.Abs(inertialYPerSecond) > 0.1)
                {
                    SetActiveAnimation(new PanAnimation(inertialXPerSecond, inertialYPerSecond, InertialPanDuration, RateFunctions.DecelerateToStop));
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
        /// <param name="animateDuration"></param>
        public void ZoomOn(double x, double y, double zoomBy, TimeSpan? animateDuration)
        {
            if (animateDuration.HasValue)
            {
                SetActiveAnimation(new ZoomHoldingPointAnimation(zoomBy * AreaView.Zoom, new PointD(x, y), TimeSpan.FromSeconds(0.5), ratio => ratio));
            }
            else
            {
                ZoomOn(x, y, zoomBy);
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

        private void SetActiveAnimation(MapAnimation animation)
        {
            _activeAnimation = animation;
            _activeAnimation.BeginAnimation(AreaView, DateTime.UtcNow);
        }

        public void UpdateAnimations()
        {
            if (_activeAnimation is null)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            if (_activeAnimation.Update(AreaView, now))
            {
                if (_animations.TryDequeue(out MapAnimation? nextAnimation))
                {
                    SetActiveAnimation(nextAnimation);
                }
            }
        }

        /// <summary>
        /// Enqueues an animation to be executed after all current animations have completed
        /// </summary>
        /// <param name="animation"></param>
        public void EnqueueAnimation(MapAnimation animation)
        {
            if (_activeAnimation is null)
            {
                SetActiveAnimation(animation);
            }
            else
            {
                _animations.Enqueue(animation);
            }
        }

        /// <summary>
        /// Gets or sets the time an inertial pan happens for
        /// </summary>
        public TimeSpan InertialPanDuration { get; set; } = TimeSpan.FromSeconds(1.0);
    }
}

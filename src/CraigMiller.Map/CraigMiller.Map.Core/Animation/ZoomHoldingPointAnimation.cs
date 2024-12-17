using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    /// <summary>
    /// Animation that zooms keeping the given canvas point in the same location.
    /// </summary>
    public class ZoomHoldingPointAnimation : DurationAnimation
    {
        readonly double _targetZoom;
        readonly PointD _zoomOnPoint;
        readonly RateFunction _ratioToEndOfDistance;
        double _initialZoom, _zoomDelta;
        double _projectedCanvasX, _projectedCanvasY;

        public ZoomHoldingPointAnimation(double targetZoom, PointD canvasPoint, TimeSpan duration, RateFunction ratioToEndOfDistance)
            : base(duration)
        {
            _targetZoom = targetZoom;
            _zoomOnPoint = canvasPoint;
            _ratioToEndOfDistance = ratioToEndOfDistance;
        }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialZoom = areaView.Zoom;

            _zoomDelta = _targetZoom - _initialZoom;

            // Record the projected position of the mouse
            areaView.CanvasToProjected(_zoomOnPoint.X, _zoomOnPoint.Y, out _projectedCanvasX, out _projectedCanvasY);
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double ratioOfZoom = _ratioToEndOfDistance(ratioOfDuration);

            // Change the zoom level
            areaView.Zoom = _initialZoom + (ratioOfZoom * _zoomDelta);

            // Get where the mouse now is
            areaView.CanvasToProjected(_zoomOnPoint.X, _zoomOnPoint.Y, out double offsetPrjX, out double offsetPrjY);

            // Move the projected position to keep the mouse position at the same location
            areaView.ProjectedLeft += _projectedCanvasX - offsetPrjX;
            areaView.ProjectedBottom += _projectedCanvasY - offsetPrjY;
        }
    }
}

using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public class ZoomAnimation : DurationAnimation
    {
        readonly double _targetZoom;
        readonly RateFunction _ratioToEndOfDistance;
        double _initialZoom, _zoomDelta;

        public ZoomAnimation(double targetZoom, TimeSpan duration, RateFunction ratioToEndOfDistance)
            : base(duration)
        {
            _targetZoom = targetZoom;
            _ratioToEndOfDistance = ratioToEndOfDistance;
        }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialZoom = areaView.Zoom;

            _zoomDelta = _targetZoom - _initialZoom;
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double ratioOfZoom = _ratioToEndOfDistance(ratioOfDuration);

            areaView.Zoom = _initialZoom + (ratioOfZoom * _zoomDelta);
        }
    }
}

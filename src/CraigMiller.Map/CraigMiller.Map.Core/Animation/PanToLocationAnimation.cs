using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;

namespace CraigMiller.Map.Core.Animation
{
    public class PanToLocationAnimation : DurationAnimation
    {
        readonly Location _location;
        readonly RateFunction _ratioToEndOfDistance;
        Location _initialLocation;
        double _latDistance, _lonDistance;

        public PanToLocationAnimation(Location location, TimeSpan duration, RateFunction ratioToEndOfDistance)
            : base(duration)
        {
            _location = location;
            _ratioToEndOfDistance = ratioToEndOfDistance;
        }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialLocation = areaView.CenterLocation;

            _latDistance = _location.Latitude - _initialLocation.Latitude;
            _lonDistance = _location.Longitude - _initialLocation.Longitude;
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double ratioOfDistance = _ratioToEndOfDistance(ratioOfDuration);

            areaView.CenterLocation = new Location(_latDistance * ratioOfDistance + _initialLocation.Latitude, _lonDistance * ratioOfDistance + _initialLocation.Longitude);
        }

        public Location ToLocation => _location;
    }
}

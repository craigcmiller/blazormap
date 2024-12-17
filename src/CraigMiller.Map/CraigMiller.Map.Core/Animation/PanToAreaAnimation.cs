using CraigMiller.Map.Core.Engine;
using CraigMiller.Map.Core.Geo;

namespace CraigMiller.Map.Core.Animation
{
    public class PanToAreaAnimation : DurationAnimation
    {
        readonly GeoRect _destRect;
        double _destNorthPrj, _destSouthPrj, _destWestPrj, _destEastPrj;
        readonly double _paddingMultiplier;
        readonly RateFunction _ratioToEndOfDistance;
        ProjectedRect _initialRect;
        double _widthDiff, _heightDiff;
        double _centerXDiff, _centerYDiff;

        public PanToAreaAnimation(GeoRect areaRect, TimeSpan duration, RateFunction rationToEndOfDistance, double paddingMultiplier = 1.25)
            : base(duration)
        {
            _destRect = areaRect;
            _ratioToEndOfDistance = rationToEndOfDistance;
            _paddingMultiplier = paddingMultiplier;
        }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialRect = areaView.ProjectedRect;

            areaView.Projection.ToProjected(_destRect.NorthLatitude, _destRect.WestLongitude, out _destWestPrj, out _destNorthPrj);
            areaView.Projection.ToProjected(_destRect.SouthLatitude, _destRect.EastLongitude, out _destEastPrj, out _destSouthPrj);

            double destWidthDiff = _destEastPrj - _destWestPrj;
            double destHeightDiff = _destNorthPrj - _destSouthPrj;

            double maxDiff = Math.Sqrt((destWidthDiff * destWidthDiff) + (destHeightDiff * destHeightDiff));

            if (_initialRect.Width < _initialRect.Height)
            {
                _widthDiff = maxDiff;
                _heightDiff = maxDiff * (_initialRect.Height / _initialRect.Width);
            }
            else
            {
                _widthDiff = maxDiff * (_initialRect.Width / _initialRect.Height);
                _heightDiff = maxDiff;
            }
            _widthDiff = _initialRect.Width - _widthDiff * 3.0;
            _heightDiff -= _initialRect.Height - _heightDiff * 3.0;

            double destCenterX = ((_destEastPrj - _destWestPrj) / 2.0) + _destWestPrj;
            double destCenterY = ((_destNorthPrj - _destSouthPrj) / 2.0) + _destSouthPrj;

            _centerXDiff = destCenterX - _initialRect.Center.X;
            _centerYDiff = destCenterY - _initialRect.Center.Y;
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double ratio = _ratioToEndOfDistance(ratioOfDuration);

            //Console.WriteLine($"{areaView.CanvasWidth}, {areaView.CanvasHeight} | {areaView.CanvasWidth / (_initialRect.Width - (ratio * _widthDiff))}, {areaView.CanvasHeight / (_initialRect.Height + (ratio * _heightDiff))}");

            areaView.ProjectedWidth = _initialRect.Width - (ratio * _widthDiff);
            areaView.ProjectedHeight = _initialRect.Height + (ratio * _heightDiff);

            areaView.ProjectedCenter = new PointD(_initialRect.Center.X + (_centerXDiff * ratio), _initialRect.Center.Y + (_centerYDiff * ratio));
        }
    }
}

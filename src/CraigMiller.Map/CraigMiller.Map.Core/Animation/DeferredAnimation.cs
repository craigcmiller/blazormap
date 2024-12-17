using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public class DeferredAnimation : MapAnimation
    {
        readonly MapAnimation _animation;
        double _deferBySeconds;

        public DeferredAnimation(MapAnimation animation, TimeSpan deferBy)
        {
            _animation = animation;
            DeferBy = deferBy;
        }

        public TimeSpan DeferBy
        {
            get => TimeSpan.FromSeconds(_deferBySeconds);
            set => _deferBySeconds = value.TotalSeconds;
        }

        public bool HasBegun { get; private set; }

        public override bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate)
        {
            if (HasBegun)
            {
                return _animation.Update(areaView, secondsSinceStart - _deferBySeconds, secondsSinceLastUpdate);
            }
            else if (secondsSinceStart >= _deferBySeconds)
            {
                HasBegun = true;

                _animation.BeginAnimation(areaView, StartTime.AddSeconds(_deferBySeconds));
            }

            return false;
        }
    }
}

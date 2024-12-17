using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public abstract class DurationAnimation : MapAnimation
    {
        double _durationSeconds;

        public DurationAnimation(TimeSpan duration)
        {
            Duration = duration;
        }

        public override bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate)
        {
            double ratioOfDuration = secondsSinceStart / _durationSeconds;

            if (ratioOfDuration > 1.0 || Abort)
            {
                return true;
            }

            Update(areaView, secondsSinceStart, secondsSinceLastUpdate, ratioOfDuration);

            return false;
        }

        public abstract void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration);

        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds(_durationSeconds);
            set => _durationSeconds = value.TotalSeconds;
        }

        protected bool Abort { get; set; }
    }
}

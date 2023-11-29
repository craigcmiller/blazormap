using CraigMiller.BlazorMap.Engine;

namespace CraigMiller.BlazorMap.Animation
{
    public abstract class MapAnimation
    {
        DateTime _lastUpdate;

        public DateTime StartTime { get; private set; }

        internal void BeginAnimation(DateTime start)
        {
            _lastUpdate = StartTime = start;
        }

        internal bool Update(GeoConverter areaView, DateTime currentTime)
        {
            bool isComplete = Update(areaView, (currentTime - StartTime).TotalSeconds, (currentTime - _lastUpdate).TotalSeconds);

            _lastUpdate = currentTime;

            return isComplete;
        }

        public abstract bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate);
    }
}

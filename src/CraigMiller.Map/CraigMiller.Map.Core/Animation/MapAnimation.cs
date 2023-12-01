using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public abstract class MapAnimation
    {
        DateTime _lastUpdate;

        public event Action<MapAnimationCompletedEventArgs>? Completed;

        public DateTime StartTime { get; private set; }

        internal virtual void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            _lastUpdate = StartTime = start;
        }

        internal bool Update(GeoConverter areaView, DateTime currentTime)
        {
            bool isComplete = Update(areaView, (currentTime - StartTime).TotalSeconds, (currentTime - _lastUpdate).TotalSeconds);

            _lastUpdate = currentTime;

            Completed?.Invoke(new MapAnimationCompletedEventArgs(this, currentTime));

            return isComplete;
        }

        public abstract bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate);
    }

    public class MapAnimationCompletedEventArgs : EventArgs
    {
        public MapAnimationCompletedEventArgs(MapAnimation animation, DateTime completedTime)
        {
            Animation = animation;
            CompletedTime = completedTime;
        }   

        public MapAnimation Animation { get; }

        public DateTime CompletedTime { get; }
    }
}

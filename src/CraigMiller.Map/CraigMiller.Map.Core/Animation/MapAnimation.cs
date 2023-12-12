using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public abstract class MapAnimation : IAnimation
    {
        DateTime _lastUpdate;

        public event Action<MapAnimationEventArgs>? Began;

        public event Action<MapAnimationCompletedEventArgs>? Completed;

        public DateTime StartTime { get; private set; }

        internal virtual void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            _lastUpdate = StartTime = start;

            Began?.Invoke(new MapAnimationEventArgs(this));
        }

        public bool Update(GeoConverter areaView, DateTime currentTime)
        {
            bool isComplete = Update(areaView, (currentTime - StartTime).TotalSeconds, (currentTime - _lastUpdate).TotalSeconds);

            _lastUpdate = currentTime;

            Completed?.Invoke(new MapAnimationCompletedEventArgs(this, currentTime));

            return isComplete;
        }

        public abstract bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate);
    }

    public class MapAnimationEventArgs : EventArgs
    {
        public MapAnimationEventArgs(MapAnimation animation)
        {
            Animation = animation;
        }

        public MapAnimation Animation { get; }
    }

    public class MapAnimationCompletedEventArgs : MapAnimationEventArgs
    {
        public MapAnimationCompletedEventArgs(MapAnimation animation, DateTime completedTime)
            :base(animation)
        {
            CompletedTime = completedTime;
        }

        public DateTime CompletedTime { get; }
    }
}

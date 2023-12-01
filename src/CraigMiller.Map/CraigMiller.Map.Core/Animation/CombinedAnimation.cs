using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public class CombinedAnimation : MapAnimation
    {
        readonly IList<MapAnimation> _animations;

        public CombinedAnimation(params MapAnimation[] animations)
        {
            _animations = new List<MapAnimation>(animations);
        }

        public void AddAnimation(MapAnimation animation)
        {
            _animations.Add(animation);
        }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            foreach (MapAnimation animation in _animations)
            {
                animation.BeginAnimation(areaView, start);
            }
        }

        public override bool Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate)
        {
            foreach (MapAnimation animation in _animations)
            {
                if (animation.Update(areaView, secondsSinceStart, secondsSinceLastUpdate))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

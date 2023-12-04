using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public interface IAnimation
    {
        bool Update(GeoConverter areaView, DateTime currentTime);
    }
}

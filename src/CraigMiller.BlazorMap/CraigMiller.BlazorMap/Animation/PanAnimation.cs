using CraigMiller.BlazorMap.Engine;

namespace CraigMiller.BlazorMap.Animation
{
    public class PanAnimation : DurationAnimation
    {
        public PanAnimation(double xPerSecond, double yPerSecond, TimeSpan duration)
        {
            XPerSecond = xPerSecond;
            YPerSecond = yPerSecond;
            Duration = duration;
        }

        public double XPerSecond { get; set; }

        public double YPerSecond { get; set; }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double ratioOfPan = 1.0 - ratioOfDuration;

            double moveXBy = XPerSecond * secondsSinceLastUpdate * ratioOfPan;
            double moveYBy = YPerSecond * secondsSinceLastUpdate * ratioOfPan;

            areaView.MoveBy(moveXBy, moveYBy);
        }
    }
}

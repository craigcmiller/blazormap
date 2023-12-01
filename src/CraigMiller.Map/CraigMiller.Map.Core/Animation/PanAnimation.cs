using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public class PanAnimation : DurationAnimation
    {
        readonly RateFunction _accelerateFunction;

        public PanAnimation(double xPerSecond, double yPerSecond, TimeSpan duration, RateFunction accelerateFunction)
            : base(duration)
        {
            XPerSecond = xPerSecond;
            YPerSecond = yPerSecond;
            _accelerateFunction = accelerateFunction;
        }

        public double XPerSecond { get; set; }

        public double YPerSecond { get; set; }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            double acceleration = _accelerateFunction(ratioOfDuration);

            double moveXBy = XPerSecond * secondsSinceLastUpdate * acceleration;
            double moveYBy = YPerSecond * secondsSinceLastUpdate * acceleration;

            areaView.MoveBy(moveXBy, moveYBy);
        }
    }
}

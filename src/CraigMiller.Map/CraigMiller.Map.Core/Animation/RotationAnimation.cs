using CraigMiller.Map.Core.Engine;

namespace CraigMiller.Map.Core.Animation
{
    public class RotationAnimation : DurationAnimation
    {
        readonly RateFunction _ratioToEndOfDistance;
        float _initialRotation, _rotationDelta;

        public RotationAnimation(TimeSpan duration, float endingDirectionDegrees, RateFunction ratioToEndOfDistance)
            : base(duration)
        {
            EndingDirectionDegrees = endingDirectionDegrees;
            _ratioToEndOfDistance = ratioToEndOfDistance;
        }

        public float EndingDirectionDegrees
        {
            get => MathHelper.RadsToDegs(EndingDirectionRadians);
            set => EndingDirectionRadians = MathHelper.DegsToRads(value);
        }

        public float EndingDirectionRadians { get; set; }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialRotation = areaView.RotationRadians;

            _rotationDelta = (float)MathHelper.AngleBetween(_initialRotation, EndingDirectionRadians);
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            areaView.RotationRadians = _initialRotation + (float)_ratioToEndOfDistance(ratioOfDuration) * _rotationDelta;
        }
    }
}

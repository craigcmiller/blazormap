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
            get => EndingDirectionRadians / (float)Math.PI * 180f;
            set => EndingDirectionRadians = value / 180.0f * (float)Math.PI;
        }

        public float EndingDirectionRadians { get; set; }

        internal override void BeginAnimation(GeoConverter areaView, DateTime start)
        {
            base.BeginAnimation(areaView, start);

            _initialRotation = areaView.RotationRadians;

            _rotationDelta = Math.Abs(EndingDirectionRadians - areaView.RotationRadians);
            if (_rotationDelta > Math.PI)
            {
                _rotationDelta = ((float)Math.Tau - _rotationDelta) * -1f;
            }
        }

        public override void Update(GeoConverter areaView, double secondsSinceStart, double secondsSinceLastUpdate, double ratioOfDuration)
        {
            areaView.RotationRadians = _initialRotation + (float)_ratioToEndOfDistance(ratioOfDuration) * _rotationDelta;

            Console.WriteLine($"{areaView.RotationDegrees:000} - {_rotationDelta / Math.PI * 180:000}");
        }
    }
}

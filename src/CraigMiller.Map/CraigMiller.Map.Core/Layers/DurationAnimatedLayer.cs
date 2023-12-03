using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class DurationAnimatedLayer<TLayer> : ILayer where TLayer : ILayer
    {
        double _durationSeconds;
        readonly TLayer _layer;
        DateTime _startTime;
        readonly DurationAnimatedLayerUpdate<TLayer> _updateLayer;
        bool reversed;
        int _repeats;

        public DurationAnimatedLayer(TLayer layer, TimeSpan duration, DurationAnimatedLayerUpdate<TLayer> updateLayer)
        {
            _layer = layer;
            Duration = duration;
            _startTime = DateTime.UtcNow;
            _updateLayer = updateLayer;
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            DateTime now = DateTime.UtcNow;

            double secondsSinceStart = (now - _startTime).TotalSeconds;
            double ratioOfDuration = secondsSinceStart / _durationSeconds;

            _updateLayer(_layer, converter, secondsSinceStart, reversed ? 1.0 - ratioOfDuration : ratioOfDuration);

            _layer.DrawLayer(canvas, converter);

            if (ratioOfDuration >= 1.0)
            {
                _startTime = now;
                _repeats++;

                if (ReverseOnCompletion)
                {
                    reversed = !reversed;
                }
            }
        }

        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds(_durationSeconds);
            set => _durationSeconds = value.TotalSeconds;
        }

        public bool ReverseOnCompletion { get; set; } = true;

        public int MaxRepeats { get; set; } = 1;
    }

    public delegate void DurationAnimatedLayerUpdate<TLayer>(TLayer layer, GeoConverter converter, double secondsSinceStart, double ratioOfDuration) where TLayer : ILayer;
}

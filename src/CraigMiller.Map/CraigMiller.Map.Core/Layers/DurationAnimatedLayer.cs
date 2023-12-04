using CraigMiller.Map.Core.Engine;
using SkiaSharp;

namespace CraigMiller.Map.Core.Layers
{
    public class DurationAnimatedLayer<TLayer> : IAnimatedLayer where TLayer : ILayer
    {
        double _durationSeconds;
        readonly TLayer _layer;
        DateTime _startTime, _lastUpdate;
        double _secondsSinceStart, _ratioOfDuration;
        readonly DurationAnimatedLayerUpdate<TLayer> _updateLayer;
        bool reversed;
        int _repetitions;

        public DurationAnimatedLayer(TLayer layer, TimeSpan duration, DurationAnimatedLayerUpdate<TLayer> updateLayer)
        {
            _layer = layer;
            Duration = duration;
            _startTime = DateTime.UtcNow;
            _updateLayer = updateLayer;
        }

        public bool Update(GeoConverter areaView, DateTime currentTime)
        {
            _lastUpdate = currentTime;

            _secondsSinceStart = (_lastUpdate - _startTime).TotalSeconds;
            _ratioOfDuration = _secondsSinceStart / _durationSeconds;

            if (_ratioOfDuration >= 1.0)
            {
                _startTime = currentTime;
                _secondsSinceStart = _ratioOfDuration = 0.0;

                if (_repetitions++ >= MaxRepetitions)
                {
                    return true;
                }

                if (ReverseOnCompletion)
                {
                    reversed = !reversed;
                }
            }

            return false;
        }

        public void DrawLayer(SKCanvas canvas, GeoConverter converter)
        {
            DateTime now = DateTime.UtcNow;

            _updateLayer(_layer, converter, _secondsSinceStart, reversed ? 1.0 - _ratioOfDuration : _ratioOfDuration);

            _layer.DrawLayer(canvas, converter);
        }

        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds(_durationSeconds);
            set => _durationSeconds = value.TotalSeconds;
        }

        public bool ReverseOnCompletion { get; set; } = true;

        public int MaxRepetitions { get; set; }
    }

    public delegate void DurationAnimatedLayerUpdate<TLayer>(TLayer layer, GeoConverter converter, double secondsSinceStart, double ratioOfDuration) where TLayer : ILayer;
}

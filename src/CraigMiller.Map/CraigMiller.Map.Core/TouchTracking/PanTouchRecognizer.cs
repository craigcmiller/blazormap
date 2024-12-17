using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigMiller.Map.Core.TouchTracking;

public class PanTouchRecognizer : ITouchRecognizer
{
    public bool TouchEnd(TrackedTouches touches)
    {
        throw new NotImplementedException();
    }

    public bool TouchMove(TrackedTouches touches)
    {
        throw new NotImplementedException();
    }

    public bool TouchStart(TrackedTouches touches)
    {
        throw new NotImplementedException();
    }
}

public interface ITouchRecognizer
{
    bool TouchStart(TrackedTouches touches);

    bool TouchMove(TrackedTouches touches);

    bool TouchEnd(TrackedTouches touches);
}

public class TouchTracker
{
    readonly IList<ITouchRecognizer> _recognizers;
    readonly IDictionary<long, TouchInfo> _activeTouches = new Dictionary<long, TouchInfo>();

    public TouchTracker()
    {
        _recognizers = new List<ITouchRecognizer>();
    }

    public void AddRecognizer(ITouchRecognizer recognizer) => _recognizers.Add(recognizer);

    void IterateRecognizers(Func<ITouchRecognizer, Func<TrackedTouches, bool>> action, TrackedTouches touches)
    {
        for (int i = _recognizers.Count - 1; i >= 0; i--)
        {
            if (action(_recognizers[i])(touches))
            {
                return;
            }
        }
    }

    public void TouchStart(TrackedTouches touches)
    {
        IterateRecognizers(tr => tr.TouchStart, touches);
    }

    public void TouchMove(TrackedTouches touches)
    {
        IterateRecognizers(tr => tr.TouchMove, touches);
    }

    public void TouchEnd(TrackedTouches touches)
    {
        IterateRecognizers(tr => tr.TouchEnd, touches);
    }
}

public sealed class TrackedTouches
{
    public TrackedTouches()
    {
        ActiveTouches = new Dictionary<int, TouchInfo>();
        ChangedTouches = new Dictionary<int, TouchInfo>();
    }

    public IDictionary<int, TouchInfo> ActiveTouches {  get; }

    public IDictionary<int, TouchInfo> ChangedTouches { get; }
}

public readonly struct TouchInfo
{
    public readonly int Id;
    public readonly double X;
    public readonly double Y;

    public TouchInfo(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
    }
}

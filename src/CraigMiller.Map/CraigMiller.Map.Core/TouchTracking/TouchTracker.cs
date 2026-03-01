using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigMiller.Map.Core.TouchTracking;

public class TouchTracker
{
    readonly IDictionary<long, TouchInfo> _activeTouches = new Dictionary<long, TouchInfo>();
    readonly IList<TouchInfo> _movedInFrame = new List<TouchInfo>();

    public TouchTracker()
    {
    }

    public void StartTouch(long id, double x, double y)
    {
        _activeTouches.Add(id, new TouchInfo(id)
        {
            X = x,
            Y = y
        });
    }

    public void BeginFrame() => _movedInFrame.Clear();

    public void MoveTouch(long id, double x, double y)
    {
        if (_activeTouches.TryGetValue(id, out TouchInfo? touchInfo))
        {
            touchInfo.Move(x, y);
        }
    }

    public void EndTouch(long id, double x, double y)
    {
        if (_activeTouches.TryGetValue(id, out TouchInfo? touchInfo))
        {
            touchInfo.Move(x, y);

            _activeTouches.Remove(id);
        }
    }
}

public class TouchInfo
{
    public TouchInfo(long id)
    {
        Id = id;
    }

    public long Id { get; }

    public double X { get; internal set; }

    public double Y { get; internal set; }

    public double PrevX { get; internal set; }

    public double PrevY { get; internal set; }

    public void Move(double x, double y)
    {
        PrevX = x;
        PrevY = y;
        X = x;
        Y = y;
    }
}

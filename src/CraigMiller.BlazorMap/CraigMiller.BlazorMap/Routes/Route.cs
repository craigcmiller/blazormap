using CraigMiller.BlazorMap.Units;
using System.Collections;

namespace CraigMiller.BlazorMap.Routes
{
    public class Route : IEnumerable<Waypoint>
    {
        readonly IList<Waypoint> _waypoints;

        public Route()
        {
            _waypoints = new List<Waypoint>();
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            _waypoints.Add(waypoint);
        }

        public void AddWaypoint(double lat, double lon)
        {
            AddWaypoint(new Waypoint(lat, lon));
        }

        public IEnumerator<Waypoint> GetEnumerator() => _waypoints.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _waypoints.GetEnumerator();

        public int WaypointCount => _waypoints.Count;

        public Waypoint this[int index] => _waypoints[index];

        public Distance Distance
        {
            get
            {
                if (_waypoints.Count > 1)
                {
                    double totalMetres = 0;

                    Waypoint from = _waypoints[0];

                    for (int i = 1; i < _waypoints.Count; i++)
                    {
                        Waypoint to = _waypoints[i];

                        Distance between = Distance.Between(from.Location, to.Location);
                        totalMetres += between.Metres;

                        from = to;
                    }

                    return new Distance(totalMetres);
                }

                return Distance.Zero;
            }
        }
    }
}

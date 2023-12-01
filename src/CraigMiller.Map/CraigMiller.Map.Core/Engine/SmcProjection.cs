using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigMiller.Map.Core.Engine
{
    /// <summary>
    /// Spherical Mercator projection
    /// </summary>
    public class SmcProjection : IProjection
    {
        public const double WorldWidthHeight = 40075016.685578488;

        public const double WorldMin = -20037508.342789244;

        public const double WorldMax = 20037508.342789244;

        public double MaxLatitude => 85.051132202148438;

        public double MinLatitude => -85.051132202148438;

        private static double Deg2Rad(double deg) => deg / (180.0 / Math.PI);

        private static double Rad2Deg(double rad) => rad * (180.0 / Math.PI);

        private static double YToLatM(double y)
        {
            return Rad2Deg(2 * Math.Atan(Math.Exp(y / Earth.RadiusMetres)) - Math.PI / 2);
        }

        private static double XToLonM(double x)
        {
            return Rad2Deg(x / Earth.RadiusMetres);
        }

        private static double LatToYM(double lat)
        {
            return Math.Log(Math.Tan(Deg2Rad(lat) / 2 + Math.PI / 4)) * Earth.RadiusMetres;
        }

        private static double LonToXM(double lon)
        {
            return Deg2Rad(lon) * Earth.RadiusMetres;
        }

        public static void LatLonToProjected(double lat, double lon, out double prjX, out double prjY)
        {
            prjY = LatToYM(lat);
            prjX = LonToXM(lon);
        }

        public void ToProjected(double lat, double lon, out double prjX, out double prjY)
        {
            prjY = LatToYM(lat);
            prjX = LonToXM(lon);
        }

        public void ToLatLon(double prjX, double prjY, out double lat, out double lon)
        {
            lat = YToLatM(prjY);
            lon = XToLonM(prjX);
        }
    }

    public static class Earth
    {
        public const double RadiusMetres = 6378137;
    }

    public interface IProjection
    {
        double MaxLatitude { get; }

        double MinLatitude { get; }

        void ToProjected(double lat, double lon, out double prjX, out double prjY);

        void ToLatLon(double prjX, double prjY, out double lat, out double lon);
    }
}

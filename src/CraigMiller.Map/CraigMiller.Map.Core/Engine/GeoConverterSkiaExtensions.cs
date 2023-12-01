using SkiaSharp;

namespace CraigMiller.Map.Core.Engine
{
    public static class GeoConverterSkiaExtensions
    {
        public static SKPoint LatLonToCanvas(this GeoConverter converter, double latitude, double longitude)
        {
            converter.LatLonToCanvas(latitude, longitude, out float x, out float y);

            return new SKPoint(x, y);
        }

        public static SKPoint ProjectedToCanvas(this GeoConverter converter,double prjX, double prjY)
        {
            converter.ProjectedToCanvas(prjX, prjY, out double canvasX, out double canvasY);

            return new SKPoint((float)canvasX, (float)canvasY);
        }

        public static SKPoint[] ProjectedToCanvas(this GeoConverter converter, PointD[] projectedPoints)
        {
            var canvasPoints = new SKPoint[projectedPoints.Length];

            for (int i = 0; i < projectedPoints.Length; i++)
            {
                PointD prj = projectedPoints[i];

                converter.ProjectedToCanvas(prj.X, prj.Y, out double cnvX, out double cnvY);

                canvasPoints[i] = new SKPoint((float)cnvX, (float)cnvY);
            }

            return canvasPoints;
        }
    }
}

namespace CraigMiller.BlazorMap.Engine
{
    public class GeoConverter
    {
        private double _canvasWidth, _canvasHeight, _zoom;

        public GeoConverter(IProjection projection)
        {
            Projection = projection;
        }

        public IProjection Projection { get; private set; }

        public double ProjectedX { get; set; }

        public double ProjectedY { get; set; }

        public double ProjectedWidth { get; set; }

        public double ProjectedHeight { get; set; }

        public double Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;

                ProjectedWidth = _canvasWidth / _zoom;
                ProjectedHeight = _canvasHeight / _zoom;
            }
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                _canvasWidth = value;
                ProjectedWidth = value / Zoom;
            }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                _canvasHeight = value;
                ProjectedHeight = value / Zoom;
            }
        }

        public void ProjectedToCanvas(double projectedX, double projectedY, out double canvasX, out double canvasY)
        {
            canvasX = (projectedX - ProjectedX) / ProjectedWidth * CanvasWidth;
            canvasY = CanvasHeight - (projectedY - ProjectedY) / ProjectedHeight * CanvasHeight;
        }

        public void CanvasToProjected(double canvasX, double canvasY, out double projectedX, out double projectedY)
        {
            double xRatio = canvasX / CanvasWidth;
            double yRatio = 1.0 - canvasY / CanvasHeight;

            projectedX = ProjectedX + xRatio * ProjectedWidth;
            projectedY = ProjectedY + yRatio * ProjectedHeight;
        }

        public void LatLonToCanvas(double latitude, double longitude, out double canvasX, out double canvasY)
        {
            Projection.ToProjected(latitude, longitude, out double prjX, out double prjY);

            ProjectedToCanvas(prjX, prjY, out canvasX, out canvasY);
        }

        public void LatLonToCanvas(double latitude, double longitude, out float canvasX, out float canvasY)
        {
            LatLonToCanvas(latitude, longitude, out double canvasXD, out double canvasYD);

            canvasX = (float)canvasXD;
            canvasY = (float)canvasYD;
        }
    }
}

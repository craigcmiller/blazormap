using CraigMiller.Map.Core.Layers.Tiling;

namespace CraigMiller.Map.Core.Engine
{
    public class GeoConverter
    {
        double _canvasWidth, _canvasHeight, _zoom;
        float _rotationRadians;

        public GeoConverter()
        {
        }

        public GeoConverter(double projectedLeft, double projectedBottom, double canvasWidth, double canvasHeight, double zoom)
        {
            ProjectedLeft = projectedLeft;
            ProjectedBottom = projectedBottom;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            Zoom = zoom;
        }

        public GeoConverter Clone() =>
            new GeoConverter
            {
                ProjectedLeft = ProjectedLeft,
                ProjectedBottom = ProjectedBottom,
                Zoom = Zoom,
                CanvasWidth = CanvasWidth,
                CanvasHeight = CanvasHeight
            };

        public IProjection Projection { get; } = new SmcProjection();

        /// <summary>
        /// Gets or sets the projected X coordinate of the leftmost side of the area
        /// </summary>
        public double ProjectedLeft { get; set; }

        /// <summary>
        /// Gets or sets the projected Y coordinate of the bottom of the area
        /// </summary>
        public double ProjectedBottom { get; set; }

        public double ProjectedWidth { get; private set; }

        public double ProjectedHeight { get; private set; }

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
            canvasX = (projectedX - ProjectedLeft) / ProjectedWidth * CanvasWidth;
            canvasY = CanvasHeight - (projectedY - ProjectedBottom) / ProjectedHeight * CanvasHeight;
        }

        public void ProjectedToCanvas(double projectedX, double projectedY, out float canvasX, out float canvasY)
        {
            ProjectedToCanvas(projectedX, projectedY, out double x, out double y);
            canvasX = (float)x;
            canvasY = (float)y;
        }

        public void CanvasToProjected(double canvasX, double canvasY, out double projectedX, out double projectedY)
        {
            double xRatio = canvasX / CanvasWidth;
            double yRatio = 1.0 - canvasY / CanvasHeight;

            projectedX = ProjectedLeft + xRatio * ProjectedWidth;
            projectedY = ProjectedBottom + yRatio * ProjectedHeight;
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

        public void CanvasToLatLon(double canvasX, double canvasY, out double lat, out double lon)
        {
            CanvasToProjected(canvasX, canvasY, out double prjX, out double prjY);
            Projection.ToLatLon(prjX, prjY, out lat, out lon);
        }

        public void ProjectedToLatLon(double prjX, double prjY, out double lat, out double lon)
            => Projection.ToLatLon(prjX, prjY, out lat, out lon);

        public ProjectedRect ProjectedRect => new ProjectedRect(ProjectedLeft, ProjectedBottom, ProjectedWidth, ProjectedHeight);

        public GeoRect GeoRect
        {
            get
            {
                ProjectedRect pr = ProjectedRect;

                ProjectedToLatLon(ProjectedLeft, ProjectedBottom + ProjectedHeight, out double northLat, out double westLon);
                ProjectedToLatLon(ProjectedLeft + ProjectedWidth, ProjectedBottom, out double southLat, out double eastLon);

                return new GeoRect(northLat, westLon, southLat, eastLon);
            }
        }

        internal PointD ProjectedCenter
        {
            get => new PointD(ProjectedLeft + ProjectedWidth / 2.0, ProjectedBottom + ProjectedHeight / 2.0);
            set
            {
                ProjectedLeft = value.X - ProjectedWidth / 2.0;
                ProjectedBottom = value.Y - ProjectedHeight / 2.0;
            }
        }

        internal Location CenterLocation
        {
            get
            {
                PointD prjCenter = ProjectedCenter;
                ProjectedToLatLon(prjCenter.X, prjCenter.Y, out double lat, out double lon);

                return new Location(lat, lon);
            }
            set
            {
                Projection.ToProjected(value.Latitude, value.Longitude, out double prjX, out double prjY);

                ProjectedCenter = new PointD(prjX, prjY);
            }
        }

        /// <summary>
        /// Gets the closest OSM zoom level
        /// </summary>
        public int OsmZoomLevel => Tile.GetZoomLevel(Zoom);

        /// <summary>
        /// Move the visible canvas by an X and Y amount
        /// </summary>
        /// <param name="xMove"></param>
        /// <param name="yMove"></param>
        public void MoveBy(double xMove, double yMove)
        {
            CanvasToProjected(0d, 0d, out double projectedLeft, out double projectedTop);
            CanvasToProjected(xMove, yMove, out double projectedDiffX, out double projectedDiffY);

            ProjectedLeft -= projectedDiffX - projectedLeft;
            ProjectedBottom -= projectedDiffY - projectedTop;
        }

        public float RotationRadians
        {
            get => _rotationRadians;
            set
            {
                if (value < 0.0)
                {
                    value += (float)Math.Tau;
                }

                _rotationRadians = Math.Abs(value % (float)Math.Tau);
            }
        }

        public float RotationDegrees
        {
            get => RotationRadians / (float)Math.PI * 180f;
            set => RotationRadians = value / 180.0f * (float)Math.PI;
        }
    }
}

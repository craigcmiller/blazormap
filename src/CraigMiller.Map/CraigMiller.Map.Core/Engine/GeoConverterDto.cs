namespace CraigMiller.Map.Core.Engine
{
    /// <summary>
    /// Data Transfer Object for (de)serialization of GeoConverters
    /// </summary>
    public class GeoConverterDto
    {
        public GeoConverterDto() { }

        public GeoConverterDto(GeoConverter converter) 
        {
            ProjectedLeft = converter.ProjectedLeft;
            ProjectedBottom = converter.ProjectedBottom;
            CanvasHeight = converter.CanvasHeight;
            CanvasWidth = converter.CanvasWidth;
            Zoom = converter.Zoom;
        }

        public double ProjectedLeft { get; set; }

        public double ProjectedBottom { get; set; }

        public double CanvasWidth { get; set; }

        public double CanvasHeight { get; set; }

        public double Zoom { get; set; }

        public GeoConverter ToGeoConverter() => new GeoConverter(ProjectedLeft, ProjectedBottom, CanvasWidth, CanvasHeight, Zoom);
    }
}

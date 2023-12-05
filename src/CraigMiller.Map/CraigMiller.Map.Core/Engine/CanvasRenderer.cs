using SkiaSharp;
using System.Xml;

namespace CraigMiller.Map.Core.Engine
{
    public class CanvasRenderer
    {
        readonly IList<RenderableLayer> _layersToEvict;

        public CanvasRenderer()
        {
            AreaView = new GeoConverter
            {
                ProjectedLeft = SmcProjection.WorldMin,
                ProjectedBottom = SmcProjection.WorldMin,
                Zoom = 0.0001
            };

            Layers = new List<RenderableLayer>();
            _layersToEvict = new List<RenderableLayer>();
        }

        public GeoConverter AreaView { get; set; }

        public IList<RenderableLayer> Layers { get; }

        public void AddLayer(ILayer layer)
        {
            Layers.Add(new RenderableLayer(layer));
        }

        public void Draw(SKCanvas canvas)
        {
            DateTime drawStart = DateTime.UtcNow;

            canvas.Save();

            double canvasWidth = AreaView.CanvasWidth;
            double canvasHeight = AreaView.CanvasHeight;

            double maxDimension = Math.Ceiling(Math.Sqrt(canvasWidth * canvasWidth + canvasHeight * canvasHeight));
            double widthDiff = maxDimension - canvasWidth;
            double heightDiff = maxDimension - canvasHeight;
            AreaView.CanvasToProjected(-widthDiff / 2.0, -heightDiff / 2.0, out double xOffset, out double yOffset);
            //AreaView.ProjectedLeft += xOffset;
            //AreaView.ProjectedBottom += yOffset;
            AreaView.CanvasWidth = AreaView.CanvasHeight = maxDimension;

            Console.WriteLine($"Orig: {canvasWidth:0} {canvasHeight:0}, new: {maxDimension:0}, diff: {widthDiff} {heightDiff}");

            float halfWidth = (float)canvasWidth / 2f;
            float halfHeight = (float)canvasHeight / 2f;
            canvas.Translate(halfWidth, halfHeight);
            canvas.RotateRadians(RotationRadians);
            //canvas.Translate(-halfWidth, -halfHeight);
            canvas.Translate((float)-maxDimension / 2f, (float)-maxDimension / 2f);

            foreach (RenderableLayer renderableLayer in Layers)
            {
                if (renderableLayer.ShouldRender)
                {
                    if (renderableLayer.AnimatedLayer is not null)
                    {
                        if (renderableLayer.AnimatedLayer.Update(AreaView, drawStart))
                        {
                            _layersToEvict.Add(renderableLayer);

                            continue;
                        }
                    }

                    renderableLayer.Layer.DrawLayer(canvas, AreaView);
                }
            }

            canvas.Restore();

            //AreaView.ProjectedLeft -= xOffset;
            //AreaView.ProjectedBottom -= yOffset;
            AreaView.CanvasWidth = canvasWidth;
            AreaView.CanvasHeight = canvasHeight;

            if (_layersToEvict.Count > 0)
            {
                foreach (RenderableLayer renderableLayer in _layersToEvict)
                {
                    Layers.Remove(renderableLayer);
                }

                _layersToEvict.Clear();
            }
        }

        public Location Center
        {
            get => AreaView.CenterLocation;
            set => AreaView.CenterLocation = value;
        }

        public double Zoom
        {
            get => AreaView.Zoom;
            set => AreaView.Zoom = value;
        }

        public float RotationRadians { get; set; }

        public float RotationDegrees
        {
            get => RotationRadians / (float)Math.PI * 180f;
            set => RotationRadians = value / 180.0f * (float)Math.PI;
        }
    }
}

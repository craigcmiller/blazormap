using Microsoft.VisualBasic;
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

            if (_layersToEvict.Count > 0)
            {
                foreach (RenderableLayer renderableLayer in _layersToEvict)
                {
                    Layers.Remove(renderableLayer);
                }

                _layersToEvict.Clear();
            }
        }

        public double CanvasWidthOffset { get; private set; }

        public double CanvasHeightOffset { get; private set; }

        public void BeginRotation(SKCanvas canvas)
        {
            canvas.Save();

            double canvasWidth = AreaView.CanvasWidth;
            double canvasHeight = AreaView.CanvasHeight;

            double maxDimension = Math.Ceiling(Math.Sqrt(canvasWidth * canvasWidth + canvasHeight * canvasHeight));
            CanvasWidthOffset = maxDimension - canvasWidth;
            CanvasHeightOffset = maxDimension - canvasHeight;
            AreaView.CanvasWidth = AreaView.CanvasHeight = maxDimension;

            float halfWidth = (float)canvasWidth / 2f;
            float halfHeight = (float)canvasHeight / 2f;
            canvas.Translate(halfWidth, halfHeight);
            canvas.RotateRadians(AreaView.RotationRadians);

            if (canvas.TotalMatrix.TryInvert(out SKMatrix reveresedMatrix))
            {
                ReverseRotationMatrix = reveresedMatrix;
            }

            //canvas.Translate(-halfWidth, -halfHeight);
            canvas.Translate((float)-maxDimension / 2f, (float)-maxDimension / 2f);
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

        public float RotationDegrees
        {
            get => AreaView.RotationDegrees;
            set => AreaView.RotationDegrees = value;
        }

        public SKMatrix ReverseRotationMatrix { get; private set; }

        public void ReverseRotatePoint(double x, double y, out double rotatedX, out double rotatedY)
        {
            SKPoint pt = ReverseRotationMatrix.MapPoint((float)x, (float)y);
            rotatedX = pt.X;
            rotatedY = pt.Y;

            rotatedX += AreaView.CanvasWidth / 2.0;
            rotatedY += AreaView.CanvasHeight / 2.0;
        }
    }
}

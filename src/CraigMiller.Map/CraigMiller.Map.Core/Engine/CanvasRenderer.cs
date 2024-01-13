using CraigMiller.Map.Core.Geo;
using SkiaSharp;

namespace CraigMiller.Map.Core.Engine
{
    public class CanvasRenderer
    {
        readonly IList<RenderableLayer> _layers = new List<RenderableLayer>();
        readonly IList<RenderableLayer> _layersToEvict = new List<RenderableLayer>();
        readonly IList<RenderableDataLayer> _dataLayers = new List<RenderableDataLayer>();

        public CanvasRenderer()
        {
            AreaView = new GeoConverter
            {
                ProjectedLeft = SmcProjection.WorldMin,
                ProjectedBottom = SmcProjection.WorldMin,
                Zoom = 0.0001
            };
        }

        public GeoConverter AreaView { get; set; }

        public void AddLayer(ILayer layer)
        {
            _layers.Add(new RenderableLayer(layer));
        }

        /// <summary>
        /// Gets all layers of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetLayers<T>() where T : ILayer
            => _layers.Where(rl => rl.Layer is T).Select(rl => (T)rl.Layer);

        /// <summary>
        /// Removes all layers of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveLayers<T>() where T : ILayer
        {
            foreach (RenderableLayer layer in _layers.Where(rl => rl.Layer is T).ToArray())
            {
                _layers.Remove(layer);
            }
        }

        /// <summary>
        /// Gets the interactive layers in reverse order so that the most recently added will have event priority
        /// </summary>
        public IEnumerable<IInteractiveLayer> InteractiveLayers
        {
            get
            {
                for (int i = _dataLayers.Count - 1; i >= 0; i--)
                {
                    RenderableDataLayer renderableDataLayer = _dataLayers[i];

                    if (renderableDataLayer.InteractiveLayer is not null && renderableDataLayer.ShouldRender)
                    {
                        yield return renderableDataLayer.InteractiveLayer;
                    }
                }

                for (int i = _layers.Count - 1; i >= 0; i--)
                {
                    RenderableLayer renderableLayer = _layers[i];

                    if (renderableLayer.InteractiveLayer is not null && renderableLayer.ShouldRender)
                    {
                        yield return renderableLayer.InteractiveLayer;
                    }
                }
            }
        }

        public void DrawMapLayers(SKCanvas canvas)
        {
            DateTime drawStart = DateTime.UtcNow;

            foreach (RenderableLayer renderableLayer in _layers)
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
                    _layers.Remove(renderableLayer);
                }

                _layersToEvict.Clear();
            }
        }

        public void AddDataLayer(IDataLayer layer) => _dataLayers.Add(new RenderableDataLayer(layer));

        public void DrawDataLayers(SKCanvas canvas, double canvasWidth, double canvasHeight)
        {
            SKMatrix rotatedMatrix = canvas.TotalMatrix;
            //canvas.ResetMatrix();
            canvas.Restore();

            foreach (RenderableDataLayer dataLayer in _dataLayers)
            {
                if (dataLayer.ShouldRender)
                {
                    dataLayer.Layer.DrawLayer(canvas, canvasWidth, canvasHeight, rotatedMatrix, AreaView);
                }
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
            canvas.RotateRadians(-AreaView.RotationRadians);

            if (canvas.TotalMatrix.TryInvert(out SKMatrix reveresedMatrix))
            {
                ReverseRotationMatrix = reveresedMatrix;
            }

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

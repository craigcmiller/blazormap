using SkiaSharp;

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
            Console.WriteLine($"Adding layer {layer.GetType().Name}");
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
    }
}

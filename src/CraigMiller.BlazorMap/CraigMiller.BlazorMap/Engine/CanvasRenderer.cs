using SkiaSharp;

namespace CraigMiller.BlazorMap.Engine
{
    public class CanvasRenderer
    {
        public CanvasRenderer()
        {
            AreaView = new GeoConverter
            {
                ProjectedLeft = SmcProjection.WorldMin,
                ProjectedBottom = SmcProjection.WorldMin,
                Zoom = 0.0001
            };

            Layers = new List<RenderableLayer>();
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
            foreach (RenderableLayer renderableLayer in Layers)
            {
                if (renderableLayer.ShouldRender)
                {
                    renderableLayer.Layer.DrawLayer(canvas, AreaView);
                }
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

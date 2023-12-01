namespace CraigMiller.Map.Core.Engine
{
    public class RenderableLayer
    {
        public RenderableLayer(ILayer layer) => Layer = layer;

        public ILayer Layer { get; }

        public bool ShouldRender { get; set; } = true;
    }
}

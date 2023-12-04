namespace CraigMiller.Map.Core.Engine
{
    public class RenderableLayer
    {
        public RenderableLayer(ILayer layer)
        {
            Layer = layer;
            if (Layer is IAnimatedLayer animatedLayer)
            {
                AnimatedLayer = animatedLayer;
            }
        }

        public ILayer Layer { get; }

        public IAnimatedLayer? AnimatedLayer { get; }

        public bool ShouldRender { get; set; } = true;
    }
}

namespace CraigMiller.Map.Core.Engine
{
    public class RenderableLayer
    {
        public RenderableLayer(ILayer layer)
        {
            Layer = layer;

            if (Layer is IInteractiveLayer interactiveLayer)
            {
                InteractiveLayer = interactiveLayer;
            }

            if (Layer is IAnimatedLayer animatedLayer)
            {
                AnimatedLayer = animatedLayer;
            }
        }

        public ILayer Layer { get; }

        public IInteractiveLayer? InteractiveLayer { get; }

        public IAnimatedLayer? AnimatedLayer { get; }

        public bool ShouldRender { get; set; } = true;
    }
}

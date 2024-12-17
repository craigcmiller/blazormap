namespace CraigMiller.Map.Core.Engine
{
    public class RenderableDataLayer
    {
        public RenderableDataLayer(IDataLayer layer)
        {
            Layer = layer;

            if (Layer is IInteractiveLayer interactiveLayer)
            {
                InteractiveLayer = interactiveLayer;
            }
        }

        public IDataLayer Layer { get; }

        public IInteractiveLayer? InteractiveLayer { get; }

        public bool ShouldRender { get; set; } = true;
    }
}

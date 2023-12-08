namespace CraigMiller.Map.Core.Engine
{
    public interface IInteractiveLayer : ILayer
    {
        bool Clicked(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool PrimaryMouseDown(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool PrimaryMouseUp(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool MouseMoved(CanvasRenderer renderer, double canvasX, double canvasY) => false;
    }
}

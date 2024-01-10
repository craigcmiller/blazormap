namespace CraigMiller.Map.Core.Engine
{
    public interface IInteractiveLayer
    {
        bool MouseClicked(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool PrimaryMouseDown(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool PrimaryMouseUp(CanvasRenderer renderer, double canvasX, double canvasY) => false;

        bool MouseMoved(CanvasRenderer renderer, double canvasX, double canvasY) => false;
    }
}

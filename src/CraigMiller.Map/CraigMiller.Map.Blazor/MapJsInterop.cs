using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CraigMiller.Map.Blazor
{
    public class MapJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public MapJsInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/CraigMiller.Map.Blazor/mapJsInterop.js").AsTask());
        }

        public async ValueTask FitToContainer(string id)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("fitCanvasToContainer", id);
        }

        public async ValueTask DisableMouseWheelScroll(string elementId)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("disableMousewheelScroll", elementId);
        }

        public async ValueTask DisableContextMenu(string elementId)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("disableContextMenu", elementId);
        }

        public async ValueTask<ElementBoundingRect> GetElementBoundingClientRect(string elementId)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<ElementBoundingRect>("getElementBoundingClientRect", elementId);
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }

    public sealed class ElementBoundingRect
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Bottom { get; set; }

        public double Right { get; set; }

        public double Left { get; set; }
    }
}

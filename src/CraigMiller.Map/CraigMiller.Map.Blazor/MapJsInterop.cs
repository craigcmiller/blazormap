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

        public async ValueTask<string> Prompt(string message)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
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

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}

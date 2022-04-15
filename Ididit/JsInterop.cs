using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Ididit;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class Dimensions
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class NodeContent
{
    public string Kind { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
    public NodeContent[] Nodes { get; set; } = Array.Empty<NodeContent>();
}

public sealed class JsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public JsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
           "import", "./_content/Ididit/jsInterop.js").AsTask());
    }

    public async ValueTask<string> Prompt(string message)
    {
        IJSObjectReference module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    public async Task<Dimensions> GetDimensions()
    {
        IJSObjectReference module = await moduleTask.Value;
        return await module.InvokeAsync<Dimensions>("getDimensions");
    }

    public async ValueTask SetElementProperty(ElementReference element, string property, object value)
    {
        IJSObjectReference module = await moduleTask.Value;
        await module.InvokeVoidAsync("setElementProperty", element, property, value);
    }

    public async Task SaveAsUTF8(string filename, string content)
    {
        byte[] data = Encoding.UTF8.GetBytes(content);

        IJSObjectReference module = await moduleTask.Value;
        await module.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
    }

    public async ValueTask<NodeContent?> ReadDirectoryFiles()
    {
        IJSObjectReference module = await moduleTask.Value;
        try
        {
            return await module.InvokeAsync<NodeContent>("readDirectoryFiles");
        }
        catch
        {
            return null;
        }
    }

    public async ValueTask WriteDirectoryFiles(NodeContent[] nodes)
    {
        IJSObjectReference module = await moduleTask.Value;
        try
        {
            await module.InvokeVoidAsync("writeDirectoryFiles", (object)nodes); // cast nodes to object to make it a single argument (avoid using "params object?[]? args")
        }
        catch
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            IJSObjectReference module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}

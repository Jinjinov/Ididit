using Ididit.App;
using Ididit.Wasm.App;
using Ididit.Wasm.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
{
    try
    {
        string? message = error.ExceptionObject.ToString();

        System.Diagnostics.Debug.WriteLine(message);

        string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ididit", "Error.log");
        System.IO.File.WriteAllText(path, message);
    }
    catch
    {
    }
};

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddServices();
builder.Services.AddWasmServices();

WebAssemblyHost host = builder.Build();

host.Services.UseServices();

await host.RunAsync();

using Ididit.App;
using Ididit.Wasm.Persistence;
using Ididit.Wasm.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddServices();
builder.Services.AddScoped<GoogleDriveBackup>();

WebAssemblyHost host = builder.Build();

host.Services.UseServices();

await host.RunAsync();

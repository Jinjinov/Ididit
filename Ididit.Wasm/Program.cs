using Ididit.App;
using Ididit.Persistence;
using Ididit.Wasm.App;
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
builder.Services.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();
builder.Services.AddScoped<IUserDisplayName, UserDisplayName>();
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://accounts.google.com/";
    options.ProviderOptions.RedirectUri = "https://localhost:44333/authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:44333/authentication/logout-callback";
    options.ProviderOptions.ClientId = "953393400208-sab1pb4ga5jeie0g50ft6uumf4uqa6in.apps.googleusercontent.com";

    options.ProviderOptions.ResponseType = "id_token token";

    //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive"); // See, edit, create, and delete all of your Google Drive files
    //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.appdata"); // See, create, and delete its own configuration data in your Google Drive
    options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.file"); // See, edit, create, and delete only the specific Google Drive files you use with this app
    //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.install"); // Connect itself to your Google Drive
});

WebAssemblyHost host = builder.Build();

host.Services.UseServices();

await host.RunAsync();

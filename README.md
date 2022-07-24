# Ididit

Progressive Web Application that tracks when was the last time you did something.

## How to host a Blazor WASM on IIS

1. instal IIS
    - open `Control Panel`
    - open `Turn Windows features on or off`
    - make sure that `Internet Information Services` are installed
2. install `URL Rewrite` extension
    - https://www.iis.net/downloads/microsoft/url-rewrite
3. install `Windows Hosting Bundle`
    - https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle
    - https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer
4. publish the `Blazor WASM` app
    - in Visual Studio right click on the project and click on `Publish...`
    - for the target select `Folder` and click `Next`
    - set `Folder location` and click `Finish`
    - click on `Publish`
5. open `IIS Manager`
    - verify that `IIS Manager` has `AspNetCoreModuleV2` under `Modules`
    - in `IIS Manager` select `Sites` and click on `Add Website...`
    - set `Site name` and `Physical path` and click `OK`
    - go to `Application Pools` and click on the application pool with the site name
    - for `.NET CLR Version` select `No Managed Code`
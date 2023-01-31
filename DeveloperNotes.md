# Error CS0103 The name '_clientSecrets' does not exist in the current context

```
using Google.Apis.Auth.OAuth2;

namespace Ididit.WebView.Online;

public partial class GoogleDriveService
{
    private readonly ClientSecrets _clientSecrets = new()
    {
        ClientId = "",
        ClientSecret = ""
    };
}
```

# How to host a Blazor WASM on IIS

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

# The program has exited with code 2147942405 (0x80070005).

If this issue is still open https://github.com/dotnet/maui/issues/12080 then install the Microsoft.WindowsAppRuntime.1.2 redist: https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads

# Bootstrap

https://getbootstrap.com/docs/4.3/layout/grid/
Containers provide a means to center and horizontally pad your site’s contents. Use .container for a responsive pixel width or .container-fluid for width: 100% across all viewport and device sizes.
In a grid layout, content must be placed within columns and only columns may be immediate children of rows.

https://getbootstrap.com/docs/4.3/utilities/flex/

https://getbootstrap.com/docs/4.3/utilities/borders/

https://getbootstrap.com/docs/4.3/utilities/sizing/

https://getbootstrap.com/docs/4.3/utilities/overflow/

https://blazorise.com/docs/components/grid

https://blazorise.com/docs/helpers/utilities

https://bootstrapdemo.blazorise.com/tests/utilities/sizing

flex-column
    flex-direction: column!important;
    - if missing, all child nodes are in a row
d-flex
    display: flex!important;
    - if missing, whole page scrolls, including header and footer
h-100
    height: 100%!important;
    - if missing, whole page scrolls, including header and footer
container-fluid
    - if missing, page width is not 100% and horizontal scrollbar is visible
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;
    margin-right: auto;
    margin-left: auto;

flex-shrink-0
    - if missing, ???
    flex-shrink: 0!important;
row
    - if missing, header and footer have 15px margin
    display: flex;
    flex-wrap: wrap;
    margin-right: -15px;
    margin-left: -15px;

col-12
    - if missing, header and footer don't have 100% width
    position: relative;
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;

    flex: 0 0 100%;
    max-width: 100%;

row
    - if missing, the second column is not visible
    display: flex;
    flex-wrap: wrap;
    margin-right: -15px;
    margin-left: -15px;

flex-grow-1
    - if missing, and content is not 100% height, footer will not be at the bottom
    flex-grow: 1!important;

overflow-hidden
    - if missing, whole page scrolls, including header and footer
    overflow: hidden!important;

col
    - if missing, column is minimum width
    position: relative;
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;

    flex-basis: 0;
    flex-grow: 1;
    max-width: 100%;

mh-100
    - if missing, height is over 100%, but not scrollable
    max-height: 100%!important;

overflow-auto
    - if missing, no vertical scrollbar
    overflow: auto!important;
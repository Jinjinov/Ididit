ididit!
=======

Take notes, manage tasks, track habits.

* * *

ididit! is free, open source and works on Web and Windows. Android, iOS, macOS and Linux are coming soon!

[Platforms](#platforms) [Features](#features) [Formats](#formats) [Themes](#themes) [About](#about)

## Platforms

### Web app:

#### Current version (2022.07.31):

[Use version 2.0.1 online](https://app.ididit.today)

#### Beta version (2022.08.01):

[Use version 2.1.0-beta online](https://new.ididit.today)

#### Old version (2021.11.09):

[Use version 1.3.7 online](https://old.ididit.today)

### Windows:

#### Current version (2022.07.31):

[Download version 2.0.1 installer](https://ididit.today/IdiditSetup.exe)

#### Beta version (2022.08.01):

[Download version 2.1.0-beta installer](https://ididit.today/IdiditBetaSetup.exe)

### Android:

Coming soon! (Hopefully in 2022)

### iOS:

Coming soonish! (Hopefully in 2023)

### macOS:

Coming soonish! (Hopefully in 2023)

### Linux:

Coming later! (Hopefully in 2024)

## Features

### Take notes, manage tasks, track habits:

*   Define your goals by organizing your notes, tasks and habits in one place
*   Use categories and sub-categories to group similar goals
*   Keep track of what is important by assigning priority to any note, task or habit

### Responsive user interface:

*   Use search to quickly find any note, task or habit
*   Use filters to list only the notes, tasks and habits that meet the criteria
*   Sort your notes, tasks and habits by any property

### Habit tracking:

*   Set the desired interval for your habits (repeating tasks)
*   Compare the actual average interval with the desired interval
*   See when you last completed a repeating task and how that compares to the desired interval

## Formats

### Integration with Google services:

*   Import from Google Keep
*   Backup to Google Drive

### File import/export:

*   Markdown import/export
*   JSON import/export
*   YAML import/export
*   TSV (Tab Separated Values) import/export

## Themes

### There are 21 color themes:

*   Cerulean
*   Cosmo
*   Cyborg
*   Darkly
*   Flatly
*   Journal
*   Litera
*   Lumen
*   Lux
*   Materia
*   Minty
*   Pulse
*   Sandstone
*   Simplex
*   Sketchy
*   Slate
*   Solar
*   Spacelab
*   Superhero
*   United
*   Yeti

## About

### ididit! is:

*   Free
*   Open source: [GitHub](https://github.com/Jinjinov/Ididit)
*   Cross platform: Planned for Web, Windows, Android, iOS, macOS and Linux

### Made with latest technologies:

*   [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
*   [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
*   [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
*   [Blazorise](https://blazorise.com/)
*   [Bootstrap](https://getbootstrap.com/)
*   [Bootswatch](https://bootswatch.com/)
*   [CsvHelper](https://joshclose.github.io/CsvHelper/)
*   [DnetIndexedDb](https://github.com/amuste/DnetIndexedDb)
*   [Font Awesome](https://fontawesome.com/)
*   [Google Drive API](https://developers.google.com/api-client-library/dotnet)
*   [IndexedDB](https://www.w3.org/TR/IndexedDB/)
*   [Markdig](https://github.com/xoofx/markdig)
*   [MAUI](https://docs.microsoft.com/en-us/dotnet/maui/)
*   [WebAssembly](https://webassembly.org/)
*   [WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)
*   [YamlDotNet](https://aaubry.net/pages/yamldotnet.html)

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
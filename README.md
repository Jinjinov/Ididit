ididit!
=======

https://ididit.today/

The procrastination friendly habit tracker.  

Take notes, manage tasks, track habits.

* * *

ididit! is free, open source and works on Web, Windows, Android, iOS, macOS and Linux!

# If this issue is still open https://github.com/dotnet/maui/issues/12080 then install the Microsoft.WindowsAppRuntime.1.2 redist: https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads

[Why ididit!](#why) [Web app](#webapp) [Releases](#releases) [Features](#features) [Formats](#formats) [Themes](#themes) [About](#about)

## Why ididit! is better

### The procrastination friendly habit tracker

#### The problem:

1.  Many other habit trackers have a streak counter. When you break the streak, you have to start over.  
    This works ok, until an event out of your control unexpectedly happens and you break the streak.  
    When that happens, you get demotivated instead of motivated.
2.  Most repeating task reminders either ignore the task you didn't do or show you the same reminder every day until you do the task.  
    There is no difference between a task that repeats every 14 days and is 2 days overdue and a task that repeats daily and is 2 days overdue.  
    Clearly one is more urgent than the other.

#### The solution:

1.  Unlike many other habit trackers, ididit! has no streak counter.  
    That means that you are not punished for skipping one day and breaking a streak.  
    Instead, ididit! keeps track of the time elapsed since the last time you did the task.
2.  ididit! compares the time elapsed since the last time you did the task with the repeating task interval.  
    You can compare the tasks by how overdue they are and immediately see which task is the most urgent.

## Web app - use ididit! online

### Current version (2022.11.11)

[Use ididit! version 1.0.2 online](https://app.ididit.today)

### Old version (2021.11.09)

[Use ididit! version 0.3.7 online](https://old.ididit.today)

## Releases

[Get ididit! from Microsoft Store](https://apps.microsoft.com/store/detail/ididit/9P5L0K28XWM3)

[Get ididit! on Google Play](https://play.google.com/store/apps/details?id=com.jinjinov.ididit)

[Get ididit! on the App Store](https://apps.apple.com/us/app/ididit-habit-tracker/id1659289949)

### 1.0.4 (2022.12.31)

[Download ididit! for macOS](https://ididit.today/download/ididit!.app.zip) and run ididit!.app

### 1.0.2 (2022.11.11)

#### Windows:

[Download ididit! version 1.0.2](https://ididit.today/download/ididit.1.0.2.zip) and run setup.exe

#### Linux:

[Download ididit! for Linux](https://ididit.today/download/ididit.linux.1.0.2.zip) and run `./Ididit.WebView.Photino` from the terminal

### 1.0.1 (2022.07.31)

#### Windows:

[Download ididit! version 1.0.1](https://ididit.today/download/ididit.1.0.1.zip) and run setup.exe

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

*   [.NET](https://dotnet.microsoft.com/en-us/download/dotnet)
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
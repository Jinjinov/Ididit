# TODO:

- [ ] choose minimum task list for ididit
- [ ] real usage screenshots

- [ ] use Size in all controls

- [ ] separate import column, select line in textarea, search selected line, copy selected line
- [ ] organize habits / Google Keep notes

Screenshots dimensions should be: 1280x800 1440x900 2560x1600 2880x1800

- [ ] use phone back button to exit options / settings
- [ ] fix phone 2x app open - check if app is already running



better description intro sentence
    more friendly than a streak counter
        ididit! doesn't use a streak counter so you won't feel demotivated when you skip a habit once and have to start your streak from zero
        ididit! calculates the average interval with which you do your habits so whenever you skip a habit, the average interval gets a little longer

    better prioritization than reminders
        10 day, 4 days, 140%
        1 day, 4 days, 400%

one screenshot per feature

example markdown note
example task note

localization
    English
    German
    Slovenian
    Croatian
    Spanish
    Japanese
    Czech

debug import export on phones
Maui Google drive authentication



- [ ] Maui Google Drive authentication:
    - Use server back end: ASP.NET Core / PHP / Python / Node.js
    - https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication
    - Why use a server back end - IWebAuthenticator - Microsoft.Maui.Authentication
    - https://blog.jhonatanoliveira.dev/google-login-integration-with-net-maui
    - https://github.com/jhonatanfernando/maui-google-authentication
    - public class LoginResponse
    - public string AccessToken { get; set; }
    - https://github.com/jhonatanfernando/maui-google-authentication/issues/1
    - namespace Ididit.Wasm.Online;
    - internal class GoogleDriveBackup
    - https://github.com/Clancey/SimpleAuth

    - https://github.com/thaveeshakannangara/MauiWithFirebase
    - https://github.com/carlfranklin/MsalGoogleAuthInMaui
    - https://github.com/omegazero2310/GoogleMapMAUI
    - https://github.com/jhonatanfernando/maui-google-authentication
    - https://github.com/krimou2/.net-maui-login-with-google
    - https://github.com/madamireag/Bookspedia-PDM
    - Xamarin Google Drive:
    - https://github.com/Dineshbala1/GoogleDriveAPI-XamarinForms
    - https://github.com/stevenchang0529/XamarinGoogleDriveRest
    - https://github.com/Kimserey/GDrivePrototype
    - https://github.com/nguyenthanhliemfc/TestGoogleDriveForXamarinForms



- [ ] Linux software repository

    - https://stackoverflow.com/questions/69132782/publishing-net-core-application-in-linux
    - https://stackoverflow.com/questions/46843863/how-to-run-a-net-core-console-application-on-linux
    - https://stackoverflow.com/questions/57898073/is-it-possible-to-publish-a-stand-alone-c-sharp-application-to-linux

    dotnet publish -c release -r linux-x64

    In .NET Core 2.2 you can create a self-contained app (i.e. not requiring .NET Core runtime installed on host machine) with this command dotnet publish -r centos.7-x64 -c Release --self-contained. 
    It'll produce executable and a lot of dependencies.

    In .NET Core 3 you can compress all dependencies into a single file dotnet publish -r centos.7-x64 -c Release /p:PublishSingleFile=true. 
    You can also add flag /p:PublishTrimmed=true to reduce executable size by tree trimming.

    Follow the below steps to run your application:

    Publish your application as a self contained application:

    dotnet publish -c release -r ubuntu.16.04-x64 --self-contained
    Copy the publish folder to the Ubuntu machine

    Open the Ubuntu machine terminal (CLI) and go to the project directory

    Provide execute permissions:

    chmod 777 ./appname
    Execute the application

    ./appname

    - https://help.ubuntu.com/community/Repositories/Ubuntu
    - https://askubuntu.com/questions/16446/how-to-get-my-software-into-ubuntu
    - https://wiki.ubuntu.com/UbuntuDevelopment/NewPackages
    - https://wiki.ubuntu.com/Upstream
    - https://wiki.ubuntu.com/Debian/ForUbuntuDevelopers#Getting_new_software_in_Debian

    - https://launchpad.net/
    - https://help.launchpad.net/Packaging
    - https://help.launchpad.net/Packaging/PPA
    - https://packaging.ubuntu.com/html/
    - https://packaging.ubuntu.com/html/getting-set-up.html
    - https://packaging.ubuntu.com/html/packaging-new-software.html
    - https://help.launchpad.net/Packaging/PPA/BuildingASourcePackage
    - https://help.launchpad.net/Packaging/PPA/Uploading
    - https://help.launchpad.net/Packaging/SourceBuilds

    - https://askubuntu.com/questions/71510/how-do-i-create-a-ppa
    - https://askubuntu.com/questions/1415597/creating-a-ppa-for-my-app
    - https://shallowsky.com/blog/programming/packaging-launchpad-ppas.html
    - https://medium.com/@labruillere/how-to-create-a-ppa-and-manage-it-like-a-8055-293da8124165
    - https://blog.desdelinux.net/en/como-subir-paquetes-a-tu-ppa/

    - https://askubuntu.com/questions/28562/how-do-i-create-a-ppa-for-a-working-program
    - https://askubuntu.com/questions/161155/how-to-create-a-ppa-for-c-program

    - https://unix.stackexchange.com/questions/620672/how-can-i-publish-a-deb-package
    - https://blog.packagecloud.io/using-dh-make-to-prepare-debian-packages/
    - https://linuxhint.com/cmake-cpack-linux/
    - https://blog.knoldus.com/create-a-debian-package-using-dpkg-deb-tool/
    - https://blog.packagecloud.io/buildling-debian-packages-with-debuild/

    - https://github.com/quamotion/dotnet-packaging
    - https://stackoverflow.com/questions/46809219/net-core-2-0-application-published-as-deb-file
    - https://medium.com/bluekiri/packaging-a-net-core-service-for-ubuntu-4f8e9202d1e5

    - https://github.com/ygoe/DotnetMakeDeb
    - https://github.com/quamotion/dotnet-packaging

    - https://askubuntu.com/questions/171796/how-can-i-create-a-simple-debian-package-from-binary
    - https://askubuntu.com/questions/896252/how-do-i-create-a-deb-package-for-a-precompiled-files
    - https://askubuntu.com/questions/27715/create-a-deb-package-from-scripts-or-binaries
    - https://askubuntu.com/questions/395753/how-can-i-create-a-deb-from-binaries

- [ ] macOS
    - https://learn.microsoft.com/en-us/dotnet/maui/macos/cli?view=net-maui-7.0

-------------------------------------------------------------------------------

- [ ] release news on: 
    - facebook - https://www.facebook.com/
    - reddit - https://www.reddit.com/
    - twitter - https://twitter.com/
    - instagram - https://www.instagram.com/
    - linkedin - https://www.linkedin.com/
    - https://www.youtube.com/
    - https://stackoverflow.com/
    - https://github.com/
    - https://alternativeto.net/

- [ ] add Google Ads
    - https://ads.google.com/

- [ ] add Google Analytics
    - https://analytics.google.com/



- [ ] Bootstrap 5.1.3 ---> 5.2.0 - https://github.com/twbs/bootstrap/issues/36431
- [ ] Fontawesome 5.15.4 ---> 6.0.0 - https://www.jsdelivr.com/package/npm/@fortawesome/fontawesome-free?version=6.0.0



- [ ] GoogleDriveBackup - auto save backup - `class Repository` - on every `IDatabaseAccess` use - Benchmark / Stopwatch
- [ ] GoogleDriveBackup - auto restore backup - `class LoginComponent` - on `LogIn` complete - `AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;`

- [ ] load db before app start in wasm

- [ ] backup on: Dropbox / OneDrive / iCloud

- [ ] fix AddCategoryList(List<CategoryModel> categoryList)
    - in Repository and DatabaseAccess
    - first search for existing Entity / Model and merge / overwrite, then add
    - add class Map / Mapper with Entity <-> Model mapping

- [ ] composition over inheritance: class Note, class Task, class RepeatingTask

- [ ] task - "done times list" should load on demand - on Task done - on show Task details

- [ ] @if (Goal is not null) in GoalComponent.razor is still called after Delete Goal

## HIGH PRIORITY:

- [ ] toggle: see all Tasks / collapse to Goal titles

- [ ] don't add Category/Goal until (name is set) / (Save button is clicked) - no need to undo adding empty objects = easy discard

- [ ] mobile: show sub-categories in Goal list

- [ ] every line that StartsWith("- ") is a task detail - edit in another MemoEdit inside TaskComponent

- [ ] DirectoryBackup save Task Details

- [ ] task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
- [ ] solution = Task.Details - when, where, name, address, phone number, working hours, website, email
- [ ] possible to do the task:
    - anytime
    - free time
    - during work week open hours
    - during weekend
    - when opportunity arises

- [ ] Tizen: keyboard arrow down can change focus to next textarea

## MEDIUM PRIORITY:

- [ ] use Drag & Drop to move Subcategory into another Category
- [ ] use Drag & Drop to move Goal into another Category
- [ ] use Drag & Drop to reorder Goals
- [ ] use Drag & Drop to reorder Tasks

- [ ] mobile: use recursion to show Category/Subcategory in Goal header
    - https://stackoverflow.com/questions/57091756/can-i-write-a-function-in-blazor-that-dynamically-renders-elements-in-a-view

- [ ] UI - range DateEdit - `<DatePicker TValue="DateTime?" InputMode="DateInputMode.Date" SelectionMode="DateInputSelectionMode.Range" />`
- [ ] UI - range ratio filter - 2x Slider

## LOW PRIORITY:

- [ ] task priority (must, should, can) - importance / urgency - (scale 1-10) - (low / med / high)
- [ ] sort by priority (must, should, can) - importance / urgency

- [ ] weekly category goal (do X tasks from this category)
- [ ] statistics (did X tasks from this category)
- [ ] graphs (number of tasks over time)

- [ ] show "Task done times" in a Calendar/Scheduler, not a List

- [ ] help: product tour: customer onboarding: Pendo, UserPilot, HelpHero, Appcues

- [ ] fix "The given key '6' was not present in the dictionary" in DeleteTask() in OnTextChanged() - there are 2 tasks from one goal details line when typing too fast

- [ ] https://blazorise.com/docs/components/repeater
    - The repeater component is a helper component that repeats the child content for each element in a collection.
    - One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
    - Meaning you can do custom actions whenever a data-source changes.

- [X] Icon:
    - https://favicon.io/favicon-generator/
    - ¡!
    - Circle
    - Miltonian
    - Regular 400 Normal
    - 110
    - #EEEEEE
    - #333333
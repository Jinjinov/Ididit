# TODO:

- [ ] organize habits

- [ ] separate import column, select line in textarea, search selected line, copy selected line

- [ ] why should you use ididit - the procrastination friendly habit tracker - with screenshots

- [ ] favicon in linux - find out why this works in Photino sample

- [ ] catch all unhandled exceptions in macOS - "Ran out of trampolines of type 0" - too many refresh calls?

- [ ] November 8-10, 2022
    - [ ] Bootstrap 4.6 ---> 5.2 - no more jQuery
        - https://getbootstrap.com/docs/4.6/getting-started/introduction/
        - https://getbootstrap.com/docs/5.2/getting-started/introduction/
    - [ ] Blazorise 1.0 ---> 1.1
        - Blazorise.Bootstrap
        - Blazorise.Bootstrap5
    - [ ] .NET 6 ---> .NET 7



- [ ] Debug on the Android Emulator
    - https://docs.microsoft.com/en-us/dotnet/maui/android/emulator/debug-on-emulator

- [ ] Set up Android device for debugging
    - https://docs.microsoft.com/en-us/dotnet/maui/android/device/setup

- [ ] Publish a .NET MAUI app for Android
    - https://docs.microsoft.com/en-us/dotnet/maui/android/deployment/overview



- [ ] GoogleDriveBackup - auto save backup - `class Repository` - on every `IDatabaseAccess` use - Benchmark / Stopwatch
- [ ] GoogleDriveBackup - auto restore backup - `class LoginComponent` - on `LogIn` complete - `AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;`



- [ ] google store - $25
    - https://docs.microsoft.com/en-us/xamarin/android/deploy-test/publishing/publishing-to-google-play

- [ ] apple store - $99 annually
    - https://docs.microsoft.com/en-us/xamarin/ios/deploy-test/app-distribution/app-store-distribution/publishing-to-the-app-store

- [ ] linux software repository
    - https://help.ubuntu.com/community/Repositories/Ubuntu
    - https://askubuntu.com/questions/16446/how-to-get-my-software-into-ubuntu
    - https://wiki.ubuntu.com/UbuntuDevelopment/NewPackages
    - https://wiki.ubuntu.com/Upstream

-------------------------------------------------------------------------------

- [ ] add Google Analytics
    - https://analytics.google.com/

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



- [ ] load db before app start in wasm
- [ ] read Theme from IndexedDb and apply the stylesheet in MainComponent HeadContent

- [ ] backup on: Dropbox / OneDrive / iCloud

- [ ] fix AddCategoryList(List<CategoryModel> categoryList)
    - in Repository and DatabaseAccess
    - first search for existing Entity / Model and merge / overwrite, then add
    - add class Map / Mapper with Entity <-> Model mapping

- [ ] composition over inheritance: class Note, class Task, class RepeatingTask

- [ ] task - "done times list" should load on demand - on Task done - on show Task details

- [ ] @if (Goal is not null) in GoalComponent.razor is still called after Delete Goal

## HIGH PRIORITY:

- [ ] new settings
- [ ] load settings
- [ ] help

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
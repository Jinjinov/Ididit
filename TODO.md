# TODO:

- [ ] screenshots of examples

- [ ] interface GetUserDisplayName()

- [ ] GoogleDriveBackup - auto save backup
- [ ] GoogleDriveBackup - auto restore backup



- [ ] google store - $25
    - https://docs.microsoft.com/en-us/xamarin/android/deploy-test/publishing/publishing-to-google-play

- [ ] apple store - $99 annually
    - https://docs.microsoft.com/en-us/xamarin/ios/deploy-test/app-distribution/app-store-distribution/publishing-to-the-app-store

- [ ] windows store - $19
    - https://www.youtube.com/watch?v=N6jPl3tBNfg
    - https://stackoverflow.com/questions/63369727/not-seeing-any-alternative-option-for-publish-create-app-packages-in-my-wpf
    - https://docs.microsoft.com/en-us/windows/uwp/publish/opening-a-developer-account
    - https://docs.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net

- [ ] linux software repository
    - https://help.ubuntu.com/community/Repositories/Ubuntu
    - https://askubuntu.com/questions/16446/how-to-get-my-software-into-ubuntu

- [ ] add Google Analytics
- [ ] release news on: facebook, reddit, twitter, instagram, linkedin



- [ ] Bootstrap 4.6 ---> 5.2



- [ ] merge "toggle goal details edit" with "edit goal title" - rewrite FocusOut

- [ ] load db before app start in wasm
- [ ] read Theme from IndexedDb and apply the stylesheet in MainComponent HeadContent

- [ ] task - "done times list" should load on demand - on Task done - on show Task details

## HIGH PRIORITY:

- [ ] new settings
- [ ] load settings
- [ ] help

- [ ] UI - remove focused borders

- [ ] toggle: see all Tasks / collapse to Goal titles

- [ ] don't add Category/Goal until (name is set) / (Save button is clicked) - no need to undo adding empty objects = easy discard

- [ ] mobile: show sub-categories in Goal list

- [ ] every line that StartsWith("- ") is a task detail - edit in another MemoEdit inside TaskComponent

- [ ] task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
- [ ] solution = Task.Details - when, where, name, address, phone number, working hours, website, email
- [ ] possible to do the task:
    - anytime
    - free time
    - during work week open hours
    - during weekend
    - when opportunity arises

## MEDIUM PRIORITY:

- [ ] use Drag & Drop to move Subcategory into another Category
- [ ] use Drag & Drop to move Goal into another Category
- [ ] use Drag & Drop to reorder Goals
- [ ] use Drag & Drop to reorder Tasks

- [ ] mobile: use recursion to show Category/Subcategory in Goal header
    - https://stackoverflow.com/questions/57091756/can-i-write-a-function-in-blazor-that-dynamically-renders-elements-in-a-view

- [ ] UI - range DateEdit - `<DatePicker TValue="DateTime?" InputMode="DateInputMode.Date" SelectionMode="DateInputSelectionMode.Range" />`
- [ ] UI - range ratio filter - 2x Slider
- [ ] UI - range priority filter - 2x Slider

- [ ] desired task duration - set (i want to exercise for 15 min) / countdown timer + alarm
- [ ] average task duration - start / stop timer (how long does it take to wash the dishes)

- [ ] keyboard arrow down can change focus to next textarea

- [ ] backup on: Dropbox / OneDrive / iCloud

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
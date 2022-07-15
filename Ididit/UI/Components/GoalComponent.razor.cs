using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    [EditorRequired]
    public GoalModel Goal { get; set; } = null!;

    [Parameter]
    public GoalModel? SelectedGoal { get; set; }

    [Parameter]
    public EventCallback<GoalModel?> SelectedGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditGoalChanged { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    TaskModel? _selectedTask;

    Blazorise.MemoEdit? _memoEdit;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (SelectedGoal == Goal && _memoEdit != null)
        {
            await _memoEdit.Focus();
        }
    }

    async Task SelectGoal()
    {
        if (SelectedGoal != Goal)
            SelectedGoal = Goal;
        else
            SelectedGoal = null;

        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    class DoneLine
    {
        public string Line = null!;
        public bool IsDone;
    }

    class DoneTask
    {
        public TaskModel Task = null!;
        public bool IsDone;
    }

    async Task OnTextChanged(string text)
    {
        List<DoneTask> oldLines = Goal.TaskList.Select(task => new DoneTask { Task = task }).ToList();
        List<DoneLine> newLines = text.Split('\n').Select(line => new DoneLine { Line = line }).ToList();

        Goal.Details = text;
        await Repository.UpdateGoal(Goal.Id);

        // reordering will be done with drag & drop, don't check the order of tasks here

        while (newLines.Any(p => !p.IsDone) || oldLines.Any(p => !p.IsDone))
        {
            while (newLines.Any(p => !p.IsDone) && oldLines.Any(p => !p.IsDone))
            {
                var newLine = newLines.First(p => !p.IsDone);
                var oldLine = oldLines.First(p => !p.IsDone);

                if (newLine.Line == oldLine.Task.Name)
                {
                    newLine.IsDone = true;
                    oldLine.IsDone = true;
                }
                else
                {
                    break;
                }
            }

            int newLinesCount = newLines.Count(p => !p.IsDone);
            int oldLinesCount = oldLines.Count(p => !p.IsDone);

            if (oldLinesCount == newLinesCount && newLinesCount > 0) // changed
            {
                var newLine = newLines.First(p => !p.IsDone);
                var oldLine = oldLines.First(p => !p.IsDone);

                await UpdateTask(oldLine.Task, newLine.Line);

                newLine.IsDone = true;
                oldLine.IsDone = true;
            }
            else if (oldLinesCount < newLinesCount) // added
            {
                var newLine = newLines.First(p => !p.IsDone);
                int idx = oldLines.FindLastIndex(p => p.IsDone) + 1;

                await AddTaskAt(idx, newLine.Line);

                newLine.IsDone = true;
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                var oldLine = oldLines.First(p => !p.IsDone);

                await DeleteTask(oldLine.Task);

                oldLine.IsDone = true;
            }
        }
    }

    // TODO: read Theme from IndexedDb and apply the stylesheet in MainLayout HeadContent
    // TODO: UI - add options page: size & theme
    // TODO: UI - move backup from footer to options
    // TODO: UI - move Login from header to options

    // TODO: fix login auth - https://github.com/dotnet/aspnetcore/issues/42046
    // TODO: GoogleDriveBackup

    private async Task UpdateTask(TaskModel task, string line)
    {
        task.Name = line;
        await Repository.UpdateTask(task.Id);
    }

    private async Task AddTaskAt(int idx, string line)
    {
        (TaskModel task, TaskModel? changedTask) = Goal.CreateTaskAt(Repository.MaxTaskId + 1, idx);
        task.Name = line;

        if (changedTask is not null)
            await Repository.UpdateTask(changedTask.Id);

        await Repository.AddTask(task);
    }

    private async Task DeleteTask(TaskModel task)
    {
        TaskModel? changedTask = Goal.RemoveTask(task);

        if (changedTask is not null)
            await Repository.UpdateTask(changedTask.Id);

        await Repository.DeleteTask(task.Id);
    }

    // TODO: mobile: single column, minimized tree view

    // TODO: mobile: show sub-categories in Goal list

    // TODO: task - "times list" should load on demand - on Task done - on show Task details

    // TODO: mobile: use Breadcrumb to show Category/Subcategory in Goal header
    // https://stackoverflow.com/questions/57091756/can-i-write-a-function-in-blazor-that-dynamically-renders-elements-in-a-view

    //
    // HIGH PRIORITY:
    //

    // TODO: new settings
    // TODO: load settings
    // TODO: help + GitHub link
    // TODO: load examples
    // TODO: delete all

    // TODO: UI - DateEdit ---> DatePicker - range <DatePicker TValue="DateTime?" InputMode="DateInputMode.Date" SelectionMode="DateInputSelectionMode.Range" />
    // TODO: UI - 2x Slider - ratio filter
    // TODO: UI - 2x Slider - priority filter

    // TODO: every line that StartsWith("- ") is a task detail

    // TODO: task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
    // TODO: (class Solution) --> (class Task.Details) - when, where
    // name, address, phone number, working hours, website, email
    // possible to do the task:
    // - anytime
    // - free time
    // - during work week open hours
    // - during weekend
    // - when opportunity arises

    // TODO: remove focused borders

    // TODO: "save" (exit edit) on Enter key

    //
    // MEDIUM PRIORITY:
    //

    // TODO: user friendly "edit" "save" - remove Edit name buttons, remove Toggle buttons (click on Goal to toggle edit mode), start edit on click (except on URL link click)

    // TODO: don't add Category/Goal until (name is set) / (Save button is clicked) - no need to undo adding empty objects = easy discard

    // TODO: desired task duration - set (i want to exercise 15 min) / countdown timer + alarm
    // TODO: average task duration - start / stop timer (how long does it take to clean the floor)

    // TODO: see all - or collapse to titles

    // TODO: use Drag & Drop to move Subcategory into another Category
    // TODO: use Drag & Drop to move Goal into another Category
    // TODO: use Drag & Drop to reorder Goals
    // TODO: use Drag & Drop to reorder Tasks

    // TODO: arrow down can change focus to next textarea

    // TODO: backup - Dropbox / OneDrive / iCloud

    //
    // LOW PRIORITY:
    //

    // TODO: task priority (must, should, can) - importance / urgency - (scale 1-10) - (low / med / high)
    // TODO: sort by priority (must, should, can) - importance / urgency

    // TODO: weekly category goal (do X tasks from this category)
    // TODO: statistics (did X tasks from this category)
    // TODO: graphs (num of tasks over time)

    // TODO: show Times in a Calendar/Scheduler, not a List
}

// TODO: use blazor layouts?
// @inherits LayoutComponentBase
// @page "/users"
// @layout MainLayout
// @page "/admin"
// @layout AdminLayout

// TODO: use route navigation for help, options, settings?

// https://blazorise.com/docs/components/repeater
// The repeater component is a helper component that repeats the child content for each element in a collection.
// One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
// Meaning you can do custom actions whenever a data-source changes.
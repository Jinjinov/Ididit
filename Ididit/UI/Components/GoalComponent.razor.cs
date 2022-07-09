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
    IRepository _repository { get; set; } = null!;

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

    async Task OnTextChanged(string text)
    {
        List<(TaskModel task, bool done, int idx)> oldLines = Goal.TaskList.Select((task, i) => (task, false, i)).ToList();
        List<(string line, bool done, int idx)> newLines = text.Split('\n').Select((line, i) => (line, false, i)).ToList();

        Goal.Details = text;
        await _repository.UpdateGoal(Goal.Id);

        // reordering will be done with drag & drop, don't check the order of tasks here

        /*

        // TODO: https://stackoverflow.com/questions/4585939/comparing-strings-and-get-the-first-place-where-they-vary-from-eachother

        go char by char until first difference
        - add
        - update

        go char by char from back
        - delete

        /**/

        while (newLines.Any(p => !p.done) && oldLines.Any(p => !p.done))
        {
            while (newLines.Any(p => !p.done) && oldLines.Any(p => !p.done))
            {
                var newLine = newLines.First(p => !p.done);
                var oldLine = oldLines.First(p => !p.done);

                if (newLine.line == oldLine.task.Name)
                {
                    newLine.done = true;
                    oldLine.done = true;
                }
                else
                {
                    break;
                }
            }

            while (newLines.Any(p => !p.done) && oldLines.Any(p => !p.done))
            {
                var newLine = newLines.Last(p => !p.done);
                var oldLine = oldLines.Last(p => !p.done);

                if (newLine.line == oldLine.task.Name)
                {
                    newLine.done = true;
                    oldLine.done = true;
                }
                else
                {
                    break;
                }
            }

            int newLinesCount = newLines.Count(p => !p.done);
            int oldLinesCount = oldLines.Count(p => !p.done);

            bool anyNew = newLinesCount > 0;
            bool anyOld = oldLinesCount > 0;

            if (oldLinesCount == newLinesCount && anyNew) // changed
            {
                var newLine = newLines.First(p => !p.done);
                var oldLine = oldLines.First(p => !p.done);

                await UpdateTask(oldLine.task, newLine.line);

                newLine.done = true;
                oldLine.done = true;
            }
            else if (oldLinesCount < newLinesCount) // added
            {
                var newLine = newLines.First(p => !p.done);
                int idx = anyOld ? oldLines.First(p => !p.done).idx : 0;

                await AddTaskAt(idx, newLine.line);

                newLine.done = true;
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                var oldLine = oldLines.First(p => !p.done);

                await DeleteTask(oldLine.task);

                oldLine.done = true;
            }
        }

        // TODO: every line that StartsWith("- ") is a task detail

        // TODO: GoogleDriveBackup

        // TODO: tree view width
        // TODO: tree view toggle all

        // TODO: toggle: (only current Category Goals) / (Goals of all sub-categories, grouped by Category)

        // TODO: show only ASAP tasks, show only repeating tasks, show only notes

        // TODO: task priority (must, should, can) - importance / urgency - (scale 1-10) - (low / med / high)
        // TODO: group by (must, should, can) - importance / urgency

        // TODO: mobile: single column, minimized tree view
        // TODO: new import/export page
        // TODO: main menu
    }

    private async Task UpdateTask(TaskModel task, string line)
    {
        task.Name = line;
        await _repository.UpdateTask(task.Id);
    }

    private async Task AddTaskAt(int idx, string line)
    {
        (TaskModel task, TaskModel? changedTask) = Goal.CreateTaskAt(_repository.MaxTaskId + 1, idx);
        task.Name = line;

        if (changedTask is not null)
            await _repository.UpdateTask(changedTask.Id);

        await _repository.AddTask(task);
    }

    private async Task DeleteTask(TaskModel task)
    {
        TaskModel? changedTask = Goal.RemoveTask(task);

        if (changedTask is not null)
            await _repository.UpdateTask(changedTask.Id);

        await _repository.DeleteTask(task.Id);
    }

    // TODO: user friendly "edit" "save" - remove Edit name buttons, remove Toggle buttons (click on Goal to toggle edit mode), edit on click (except on URL link click)

    // TODO: remove focused borders

    // TODO: don't add Category / Goal until (name is set) / (Save button is clicked)

    // TODO: use Breadcrumb to show Category/Subcategory in Goal header

    // TODO: show sub-categories in Goal list

    // TODO: use Drag & Drop to move Subcategory into another Category
    // TODO: use Drag & Drop to move Goal into another Category
    // TODO: use Drag & Drop to reorder Goals
    // TODO: use Drag & Drop to reorder Tasks

    // TODO: move backup from MainLayout to a component

    // TODO: task - times list should load on demand - on Task done - on show Task details

    // https://blazorise.com/docs/components/repeater
    // The repeater component is a helper component that repeats the child content for each element in a collection.
    // One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
    // Meaning you can do custom actions whenever a data-source changes.

    // TODO: desired task duration - set (i want to exercise 15 min) / countdown timer + alarm
    // TODO: average task duration - start / stop timer (how long does it take to clean the floor)

    // TODO: weekly category goal (do X tasks from this category)
    // TODO: statistics (did X tasks from this category)
    // TODO: graphs (num of tasks over time)

    // TODO: backup - Dropbox / OneDrive / iCloud

    // TODO: task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
    // TODO: (class Solution) --> (class TaskDetails) - when, where
    // name, address, phone number, working hours, website, email
    // possible to do the task:
    // - anytime
    // - free time
    // - during work week open hours
    // - during weekend
    // - when opportunity arises

    // TODO: settings with small and large UI: https://bootstrapdemo.blazorise.com/tests/misc-forms

    // TODO: bootstrap themes
    // https://cdnjs.com/libraries/bootstrap/5.1.3
    // https://www.jsdelivr.com/package/npm/bootstrap
    // https://cdnjs.com/libraries/bootswatch/5.1.3
    // https://www.jsdelivr.com/package/npm/bootswatch?path=dist

    // TODO: loading intro - https://bootstrapdemo.blazorise.com/tests/spinkit

    // TODO: use blazor layouts?
    // @inherits LayoutComponentBase
    // @page "/users"
    // @layout MainLayout
    // @page "/admin"
    // @layout AdminLayout

    // TODO: use route navigation for help, options, settings?

    // TODO: options
    // TODO: help

    // TODO: see all - or collapse to titles

    // TODO: show Times in a Calendar/Scheduler, not a List

    // TODO: arrow down can change focus to next textarea
}

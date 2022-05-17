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
        List<string> oldLines = Goal.Details.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        List<string> newLines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        Goal.Details = text;
        await _repository.UpdateGoal(Goal.Id);

        // reordering will be done with drag & drop, don't check the order of tasks here

        List<int> indexOfNewLineInOldLines = newLines.Select(newLine => oldLines.IndexOf(newLine)).ToList();
        List<int> indexOfOldLineInNewLines = oldLines.Select(oldLine => newLines.IndexOf(oldLine)).ToList();

        for (int i = indexOfNewLineInOldLines.Count - 1; i >= 0; --i)
        {
            if (indexOfNewLineInOldLines[i] == -1) // newLines has a line that was not here before
            {
                if (oldLines.Count == newLines.Count) // changed
                {
                    TaskModel task = Goal.TaskList[i];

                    task.Name = newLines[i];

                    await _repository.UpdateTask(task.Id);
                }
                else // added
                {
                    TaskModel task = Goal.CreateTask(i);

                    task.Name = newLines[i];

                    // TODO: set task.PreviousId

                    await _repository.AddTask(task);
                }
            }
        }

        for (int i = indexOfOldLineInNewLines.Count - 1; i >= 0; --i)
        {
            if (indexOfOldLineInNewLines[i] == -1) // oldLines has a line that is not here now
            {
                if (oldLines.Count == newLines.Count) // changed
                {

                }
                else // deleted
                {
                    TaskModel task = Goal.TaskList[i];

                    Goal.TaskList.Remove(task);

                    // TODO: set task.PreviousId

                    await _repository.DeleteTask(task.Id);
                }
            }
        }

        // TODO: GoogleDriveBackup

        // TODO: mobile: single column, minimized tree view
        // TODO: new import/export page
        // TODO: main menu
    }

    // TODO: user friendly "edit" "save" - remove Edit buttons, remove Toggle buttons, edit on click (except on URL link click)

    // TODO: remove focused borders

    // TODO: don't add Category / Goal until (name is set) / (Save button is clicked)

    // TODO: use Breadcrumb to show Category/Subcategory in Goal header

    // TODO: show sub-categories in Goal list

    // TODO: toggle: (only current Category Goals) / (Goals of all sub-categories, grouped by Category)

    // TODO: use Drag & Drop to move Subcategory into another Category
    // TODO: use Drag & Drop to move Goal into another Category
    // TODO: use Drag & Drop to reorder Goals
    // TODO: use Drag & Drop to reorder Tasks

    // TODO: move backup from MainLayout to a component

    // TODO: import - # headers
    // TODO: import - ### headers
    // TODO: import - empty lines
    // TODO: import - notes = lines without "-"
    // TODO: import - tasks = lines with "-"
    // TODO: import - nested tasks = lines with "    -"

    // TODO: task - times list should load on demand - on Task done - on show Task details

    // https://blazorise.com/docs/components/repeater
    // The repeater component is a helper component that repeats the child content for each element in a collection.
    // One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
    // Meaning you can do custom actions whenever a data-source changes.

    // TODO: task priority (must, should, can) - importance / urgency - (scale 1-10) - (low / med / high)
    // TODO: group by (must, should, can) - importance / urgency
    // TODO: group statistics

    // TODO: desired task duration - set (i want to exercise 15 min) / countdown timer + alarm
    // TODO: average task duration - start / stop timer (how long does it take to clean the floor)

    // TODO: weekly category goal (do X tasks from this category)

    // TODO: statistics / graphs (did X tasks from this category) / (num of tasks over time)

    // TODO: backup - Dropbox / OneDrive / iCloud

    // TODO: task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
    // TODO: class Solution - when, where
    // name, address, phone number, working hours, website, email
    // possible to do the task:
    // - anytime
    // - free time
    // - during work week open hours
    // - during weekend
    // - when opportunity arises

    // TODO: settings - https://bootstrapdemo.blazorise.com/tests/misc-forms

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

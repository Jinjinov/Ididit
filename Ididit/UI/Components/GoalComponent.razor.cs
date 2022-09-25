using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
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
        if (SelectedGoal == Goal && EditGoal == null && _memoEdit != null)
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

    async Task OnFocusOut()
    {
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
        Goal.Details = text;
        await Repository.UpdateGoal(Goal.Id);

        if (!Goal.CreateTaskFromEachLine)
            return;

        List<DoneTask> oldLines = Goal.TaskList.Select(task => new DoneTask { Task = task }).ToList();
        List<DoneLine> newLines = text.Split('\n').Select(line => new DoneLine { Line = line }).ToList();

        // reordering will be done with drag & drop, don't check the order of tasks here

        while (newLines.Any(p => !p.IsDone) || oldLines.Any(p => !p.IsDone))
        {
            while (newLines.Any(p => !p.IsDone) && oldLines.Any(p => !p.IsDone))
            {
                DoneLine newLine = newLines.First(p => !p.IsDone);
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

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
                DoneLine newLine = newLines.First(p => !p.IsDone);
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

                await UpdateTask(oldLine.Task, newLine.Line);

                newLine.IsDone = true;
                oldLine.IsDone = true;
            }
            else if (oldLinesCount < newLinesCount) // added
            {
                DoneLine newLine = newLines.First(p => !p.IsDone);
                int idx = oldLines.FindLastIndex(p => p.IsDone) + 1;

                await AddTaskAt(idx, newLine.Line);

                newLine.IsDone = true;
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

                await DeleteTask(oldLine.Task);

                oldLine.IsDone = true;
            }
        }
    }

    private async Task UpdateTask(TaskModel task, string line)
    {
        task.Name = line;
        await Repository.UpdateTask(task.Id);
    }

    private async Task AddTaskAt(int idx, string line)
    {
        (TaskModel task, TaskModel? changedTask) = Goal.CreateTaskAt(Repository.NextTaskId, idx);
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
}

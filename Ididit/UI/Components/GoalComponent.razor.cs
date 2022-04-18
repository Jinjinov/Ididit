using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
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

    TaskModel? _selectedTask;

    async Task SelectGoal()
    {
        SelectedGoal = Goal;

        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task OnTextChanged(string text)
    {
        if (Goal.Details.Count(c => c.Equals('\n')) < text.Count(c => c.Equals('\n')))
        {
            string[] lines = Goal.Details.Split('\n');

            TaskModel task = Goal.CreateTask();

            task.Name = lines[^1];

            await _repository.AddTask(task);
        }
        else if (Goal.Details.Count(c => c.Equals('\n')) > text.Count(c => c.Equals('\n')) && Goal.TaskList.Any())
        {
            TaskModel task = Goal.TaskList.Last();

            Goal.TaskList.Remove(task);

            await _repository.DeleteTask(task.Id);
        }

        // TODO: edit time
        // TODO: delete time
        // TODO: task - times completed list should load on demand

        // TODO: task - set desired interval
        // TODO: task - calculate average interval
        // TODO: task - calculate (elapsed time / desired interval)

        // TODO: move backup from MainLayout to a component

        // TODO: import - # headers
        // TODO: import - ### headers
        // TODO: import - empty lines
        // TODO: import - notes = lines without "-"
        // TODO: import - tasks = lines with "-"
        // TODO: import - nested tasks = lines with "    -"

        // using Markdig;
        // readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();
        // public string MarkdownToHtml(string markdown) => Markdown.ToHtml(markdown, markdownPipeline);
        // @((MarkupString)Goal.NotesMarkdownHtml)

        // TODO: settings
        // TODO: themes
        // TODO: loading intro

        // TODO: drag & drop - reorder
        // TODO: drag & drop - move / change parent

        await _repository.UpdateGoalDetails(Goal.Id, text);
    }
}

using Blazorise.Localization;
using Ididit.Data;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Ididit.Data.Model.Models;

namespace Ididit.App;

internal class Examples : IExamples
{
    private readonly IRepository _repository;
    private readonly ITextLocalizer<Examples> _localizer;

    public Examples(IRepository repository, ITextLocalizer<Examples> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task LoadBasicExamples()
    {
        long nextGoalId = _repository.NextGoalId;

        GoalModel notesGoal = _repository.Category.CreateGoal(nextGoalId++, _localizer["Change these notes to tasks"]);
        GoalModel tasksGoal = _repository.Category.CreateGoal(nextGoalId++, _localizer["Tasks goal"]);
        GoalModel markdownGoal = _repository.Category.CreateGoal(nextGoalId++, _localizer["Markdown goal"]);

        notesGoal.Details =
            $"""
            {_localizer["The first icon changes notes to tasks"]}
            {_localizer["Each line becomes one task"]}
            {_localizer["Select a task to see its details"]}
            """;

        await _repository.AddGoal(notesGoal);

        long nextTaskId = _repository.NextTaskId;

        TaskModel note = tasksGoal.CreateTask(nextTaskId++, _localizer["Note"], TimeSpan.Zero, Priority.None, TaskKind.Note, null);
        TaskModel doneTask = tasksGoal.CreateTask(nextTaskId++, _localizer["High priority task"], TimeSpan.Zero, Priority.High, TaskKind.Task, null);
        TaskModel notDoneTask = tasksGoal.CreateTask(nextTaskId++, _localizer["Task"], TimeSpan.Zero, Priority.Medium, TaskKind.Task, TimeSpan.FromMinutes(30));
        TaskModel neverDoneTask = tasksGoal.CreateTask(nextTaskId++, _localizer["Low priority habit"], TimeSpan.FromDays(10), Priority.Low, TaskKind.RepeatingTask, null);
        TaskModel doneTwiceTask = tasksGoal.CreateTask(nextTaskId++, _localizer["Habit"], TimeSpan.FromDays(4), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(15));

        foreach (TaskModel task in tasksGoal.TaskList)
        {
            tasksGoal.Details += string.IsNullOrEmpty(tasksGoal.Details) ? task.Name : Environment.NewLine + task.Name;
        }

        tasksGoal.CreateTaskFromEachLine = true;

        await _repository.AddGoal(tasksGoal);

        await _repository.AddTask(note);
        await _repository.AddTask(doneTask);
        await _repository.AddTask(notDoneTask);
        await _repository.AddTask(neverDoneTask);
        await _repository.AddTask(doneTwiceTask);

        List<(DateTime time, long taskId)> times = new()
        {
            { doneTask.AddTime(DateTime.Now.AddDays(-2)) },
            { doneTwiceTask.AddTime(DateTime.Now.AddDays(-3)) },
            { doneTwiceTask.AddTime(DateTime.Now.AddDays(-1)) }
        };

        foreach ((DateTime time, long taskId) in times)
        {
            await _repository.AddTime(time, taskId);
            await _repository.UpdateTask(taskId);
        }

        markdownGoal.Details =
            """
            # Markdown
            ## Heading
                code
                block
            **bold**
            *italic*
            ***bold and italic***
            `code`
            [ididit!](https://ididit.today)
            - one item
            - another item
            ---
            1. first item
            2. second item
            """;

        await _repository.AddGoal(markdownGoal);
    }

    public async Task LoadExamples()
    {
        long nextCategoryId = _repository.NextCategoryId;

        List<CategoryModel> categories = new()
        {
            { _repository.Category.CreateCategory(nextCategoryId++, _localizer["Accomplishments"]) },
            { _repository.Category.CreateCategory(nextCategoryId++, _localizer["Health"]) },
            { _repository.Category.CreateCategory(nextCategoryId++, _localizer["Well-being"]) }
        };

        foreach (CategoryModel category in categories)
            await _repository.AddCategory(category);

        long nextGoalId = _repository.NextGoalId;

        List<GoalModel> goals = new()
        {
            { categories[0].CreateGoal(nextGoalId++, _localizer["Chores"]) },
            { categories[0].CreateGoal(nextGoalId++, _localizer["Hobbies"]) },
            { categories[0].CreateGoal(nextGoalId++, _localizer["Personal growth"]) },

            { categories[1].CreateGoal(nextGoalId++, _localizer["Appearance"]) },
            { categories[1].CreateGoal(nextGoalId++, _localizer["Food"]) },
            { categories[1].CreateGoal(nextGoalId++, _localizer["Sports"]) },

            { categories[2].CreateGoal(nextGoalId++, _localizer["Peace of mind"]) },
            { categories[2].CreateGoal(nextGoalId++, _localizer["Relationships"]) },
            { categories[2].CreateGoal(nextGoalId++, _localizer["Relaxation"]) }
        };

        long nextTaskId = _repository.NextTaskId;

        List<TaskModel> tasks = new()
        {
            { goals[0].CreateTask(nextTaskId++, _localizer["Clean dust"], TimeSpan.FromDays(7), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },
            { goals[0].CreateTask(nextTaskId++, _localizer["Clean the windows"], TimeSpan.FromDays(90), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(20)) },

            { goals[1].CreateTask(nextTaskId++, _localizer["Go salsa dancing"]) },
            { goals[1].CreateTask(nextTaskId++, _localizer["Play the piano"], TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },

            { goals[2].CreateTask(nextTaskId++, _localizer["Attend a workshop"]) },
            { goals[2].CreateTask(nextTaskId++, _localizer["Learn Spanish"], TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },

            { goals[3].CreateTask(nextTaskId++, _localizer["Get a haircut"], TimeSpan.FromDays(21), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },
            { goals[3].CreateTask(nextTaskId++, _localizer["Buy new clothes"]) },

            { goals[4].CreateTask(nextTaskId++, _localizer["Drink water"], TimeSpan.FromHours(8), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(1)) },
            { goals[4].CreateTask(nextTaskId++, _localizer["Eat fruit"], TimeSpan.FromHours(12), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(3)) },

            { goals[5].CreateTask(nextTaskId++, _localizer["Stretch & workout"], TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(5)) },
            { goals[5].CreateTask(nextTaskId++, _localizer["Go hiking"], TimeSpan.FromDays(7), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(90)) },

            { goals[6].CreateTask(nextTaskId++, _localizer["Take a walk"], TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },
            { goals[6].CreateTask(nextTaskId++, _localizer["Meditate"], TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(15)) },

            { goals[7].CreateTask(nextTaskId++, _localizer["Call parents"], TimeSpan.FromDays(7), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },
            { goals[7].CreateTask(nextTaskId++, _localizer["Do someone a favor"]) },

            { goals[8].CreateTask(nextTaskId++, _localizer["Read a book"], TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(60)) },
            { goals[8].CreateTask(nextTaskId++, _localizer["Get a massage"], TimeSpan.FromDays(28), Priority.Low, TaskKind.RepeatingTask, TimeSpan.FromMinutes(45)) }
        };

        foreach (GoalModel goal in goals)
        {
            foreach (TaskModel task in goal.TaskList)
            {
                goal.Details += string.IsNullOrEmpty(goal.Details) ? task.Name : Environment.NewLine + task.Name;
            }

            goal.CreateTaskFromEachLine = true;

            await _repository.AddGoal(goal);
        }

        foreach (TaskModel task in tasks)
            await _repository.AddTask(task);

        List<(DateTime time, long taskId)> times = new()
        {
            { tasks[0].AddTime(DateTime.Now.AddDays(-20)) },
            { tasks[0].AddTime(DateTime.Now.AddDays(-10)) },

            { tasks[3].AddTime(DateTime.Now.AddDays(-28)) },
            { tasks[3].AddTime(DateTime.Now.AddDays(-8)) },

            { tasks[6].AddTime(DateTime.Now.AddDays(-27)) },
            { tasks[6].AddTime(DateTime.Now.AddDays(-13)) },

            { tasks[11].AddTime(DateTime.Now.AddDays(-70)) },
            { tasks[11].AddTime(DateTime.Now.AddDays(-12)) },

            { tasks[17].AddTime(DateTime.Now.AddDays(-300)) }
        };

        foreach ((DateTime time, long taskId) in times)
        {
            await _repository.AddTime(time, taskId);
            await _repository.UpdateTask(taskId);
        }
    }
}

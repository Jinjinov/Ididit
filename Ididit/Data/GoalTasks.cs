using Ididit.Data.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.Data;

internal static class GoalTasks
{
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

    public static async Task UpdateGoalTasks(this IRepository repository, GoalModel goal)
    {
        List<DoneTask> oldLines = goal.TaskList.Select(task => new DoneTask { Task = task }).ToList();
        List<DoneLine> newLines = goal.Details.Split('\n').Select(line => new DoneLine { Line = line }).ToList();

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

                TaskModel task = await AddTaskAt(idx, newLine.Line);

                newLine.IsDone = true;

                oldLines.Insert(idx, new DoneTask() { Task = task, IsDone = true });
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

                await DeleteTask(oldLine.Task);

                oldLine.IsDone = true;
            }
        }

        async Task UpdateTask(TaskModel task, string line)
        {
            task.Name = line;
            await repository.UpdateTask(task.Id);
        }

        async Task<TaskModel> AddTaskAt(int idx, string line)
        {
            (TaskModel task, TaskModel? changedTask) = goal.CreateTaskAt(repository.NextTaskId, idx);
            task.Name = line;

            if (changedTask is not null)
                await repository.UpdateTask(changedTask.Id);

            await repository.AddTask(task);

            return task;
        }

        async Task DeleteTask(TaskModel task)
        {
            TaskModel? changedTask = goal.RemoveTask(task);

            if (changedTask is not null)
                await repository.UpdateTask(changedTask.Id);

            await repository.DeleteTask(task.Id);
        }
    }
}

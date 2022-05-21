using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class GoalModel
{
    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long CategoryId { get; set; }

    public long? PreviousId { get; set; }

    public string Name { get; set; } = string.Empty;
    internal string Details { get; set; } = string.Empty;

    [JsonIgnore]
    internal int Rows => Details.Count(c => c == '\n') + 1;

    public List<TaskModel> TaskList = new();

    public TaskModel CreateTask(long id)
    {
        TaskModel task = new()
        {
            Id = id,
            GoalId = Id,
            PreviousId = TaskList.Any() ? TaskList.Last().Id : null,
            Name = "Task " + TaskList.Count,
            CreatedAt = DateTime.Now
        };

        TaskList.Add(task);

        return task;
    }

    public (TaskModel newTask, TaskModel? changedTask) CreateTask(long id, int index)
    {
        TaskModel? changedTask = null;

        TaskModel task = new()
        {
            Id = id,
            GoalId = Id,
            PreviousId = TaskList.Any() ? TaskList.Last().Id : null,
            Name = "Task " + TaskList.Count,
            CreatedAt = DateTime.Now
        };

        if (index > 0)
            task.PreviousId = TaskList[index - 1].Id;
        else
            task.PreviousId = null;

        if (index < TaskList.Count)
        {
            changedTask = TaskList[index];
            changedTask.PreviousId = task.Id;
        }

        TaskList.Insert(index, task);

        return (task, changedTask);
    }

    public TaskModel? RemoveTask(TaskModel task)
    {
        TaskModel? changedTask = null;

        int index = TaskList.IndexOf(task);

        if (index < TaskList.Count - 1)
        {
            changedTask = TaskList[index + 1];

            if (index > 0)
                changedTask.PreviousId = TaskList[index - 1].Id;
            else
                changedTask.PreviousId = null;
        }

        TaskList.Remove(task);

        return changedTask;
    }

    public void OrderTasks()
    {
        List<TaskModel> taskList = new();
        long? previousId = null;

        while (TaskList.Any())
        {
            TaskModel task = TaskList.Single(t => t.PreviousId == previousId);
            previousId = task.Id;
            taskList.Add(task);
            TaskList.Remove(task);
        }

        TaskList = taskList;
    }
}

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

    public TaskModel CreateTask(int index = -1)
    {
        TaskModel task = new()
        {
            GoalId = Id,
            PreviousId = TaskList.Any() ? TaskList.Last().Id : null,
            Name = "Task " + TaskList.Count,
            CreatedAt = DateTime.Now
        };

        if (index == -1)
            TaskList.Add(task);
        else
            TaskList.Insert(index, task);

        return task;
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

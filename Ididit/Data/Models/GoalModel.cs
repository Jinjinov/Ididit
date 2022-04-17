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

    public string Name { get; set; } = string.Empty;
    internal string Details { get; set; } = string.Empty;

    [JsonIgnore]
    internal int Rows => Details.Count(c => c == '\n') + 1;

    public List<TaskModel> TaskList = new();

    public TaskModel CreateTask()
    {
        TaskModel task = new()
        {
            GoalId = Id,
            Name = "Task " + TaskList.Count,
            CreatedAt = DateTime.Now
        };

        TaskList.Add(task);

        return task;
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class TaskModel
{
    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long GoalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public long CreatedAt { get; set; }

    public long AverageInterval { get; set; }
    public long DesiredInterval { get; set; }

    public List<long> TimeList = new();

    public (long Ticks, long TaskId) AddTime()
    {
        long time = DateTime.Now.Ticks;

        TimeList.Add(time);

        return (time, Id);
    }
}

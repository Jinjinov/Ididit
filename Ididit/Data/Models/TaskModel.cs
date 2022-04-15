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

    public bool IsRepeating { get; set; }
    public DateTime Created { get; set; }

    public long AverageIntervalTicks { get => AverageInterval.Ticks; set => AverageInterval = new TimeSpan(value); }
    public long DesiredIntervalTicks { get => DesiredInterval.Ticks; set => DesiredInterval = new TimeSpan(value); }

    internal TimeSpan AverageInterval { get; set; }
    internal TimeSpan DesiredInterval { get; set; } = new TimeSpan(1, 0, 0, 0);

    public List<DateTime> TimeList = new();

    public (long Ticks, long TaskId) AddTime()
    {
        DateTime time = DateTime.Now;

        TimeList.Add(time);

        return (time.Ticks, Id);
    }
}

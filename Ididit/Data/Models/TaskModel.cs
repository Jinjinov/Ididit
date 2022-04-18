using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class TaskModel
{
    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long GoalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public long AverageInterval { get; set; }
    public long DesiredInterval { get; set; } = 864000000000;

    [JsonIgnore]
    internal TimeSpan AverageTime { get => new(AverageInterval); set => AverageInterval = value.Ticks; }
    [JsonIgnore]
    internal TimeSpan DesiredTime { get => new(DesiredInterval); set => DesiredInterval = value.Ticks; }
    [JsonIgnore]
    internal TimeSpan ElapsedTime => TimeList.Any() ? DateTime.Now - TimeList.Last() : DateTime.Now - CreatedAt;
    [JsonIgnore]
    internal bool IsElapsedOverAverage => TimeList.Any() && (ElapsedTime > AverageTime);
    [JsonIgnore]
    internal double ElapsedToAverageRatio => TimeList.Any() ? ElapsedTime / AverageTime * 100.0 : 100.0;
    [JsonIgnore]
    internal bool IsElapsedOverDesired => ElapsedTime > DesiredTime;
    [JsonIgnore]
    internal double ElapsedToDesiredRatio => ElapsedTime / DesiredTime * 100.0;

    public List<DateTime> TimeList = new();

    public (DateTime Time, long TaskId) AddTime()
    {
        DateTime time = DateTime.Now;

        TimeList.Add(time);

        if (TimeList.Count == 1)
            AverageTime = TimeList.First() - CreatedAt;
        else
            AverageTime = TimeSpan.FromMilliseconds(TimeList.Zip(TimeList.Skip(1), (x, y) => (y - x).TotalMilliseconds).Average());

        return (time, Id);
    }

    public void RemoveTime(DateTime time)
    {
        TimeList.Remove(time);

        if (TimeList.Count == 0)
            AverageTime = TimeSpan.Zero;
        else if (TimeList.Count == 1)
            AverageTime = TimeList.First() - CreatedAt;
        else
            AverageTime = TimeSpan.FromMilliseconds(TimeList.Zip(TimeList.Skip(1), (x, y) => (y - x).TotalMilliseconds).Average());
    }
}

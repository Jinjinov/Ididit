using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class TaskModel
{
    static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long GoalId { get; set; }

    public long? PreviousId { get; set; }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;

            if (!_name.StartsWith("- "))
                MarkdownHtml = Markdown.ToHtml(_name, _markdownPipeline).Replace("<p>", "<div>").Replace("</p>", "</div>");
            else
                MarkdownHtml = null;
        }
    }

    public string Details { get; set; } = string.Empty;

    [JsonIgnore]
    internal string? MarkdownHtml { get; set; }

    public DateTime CreatedAt { get; set; }

    public long AverageInterval { get; set; }
    public long DesiredInterval { get; set; }

    public Priority Priority { get; set; }

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

    public (DateTime Time, long TaskId) AddTime(DateTime time)
    {
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

    public void UpdateTime(DateTime oldTime, DateTime newTime)
    {
        TimeList.Remove(oldTime);

        TimeList.Add(newTime);

        if (TimeList.Count == 1)
            AverageTime = TimeList.First() - CreatedAt;
        else
            AverageTime = TimeSpan.FromMilliseconds(TimeList.Zip(TimeList.Skip(1), (x, y) => (y - x).TotalMilliseconds).Average());
    }
}

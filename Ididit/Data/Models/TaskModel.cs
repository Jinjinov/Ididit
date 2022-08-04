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
    internal long Id { get; init; }
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

            if (!string.IsNullOrEmpty(_name) && char.IsLetter(_name.First()))
                MarkdownHtml = null;
            else
                MarkdownHtml = Markdown.ToHtml(_name, _markdownPipeline).Replace("<p>", "<div>").Replace("</p>", "</div>");
        }
    }

    private string _details = string.Empty;
    public string Details
    {
        get => _details;
        set
        {
            _details = value;

            DetailsMarkdownHtml = Markdown.ToHtml(_details, _markdownPipeline).Replace("<p>", "<div>").Replace("</p>", "</div>");
        }
    }

    [JsonIgnore]
    internal string? MarkdownHtml { get; set; }

    [JsonIgnore]
    internal string DetailsMarkdownHtml { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastTimeDoneAt { get; set; }

    [JsonIgnore]
    internal bool IsCompleted => !IsRepeating && IsDoneAtLeastOnce;

    [JsonIgnore]
    internal bool IsDoneAtLeastOnce => LastTimeDoneAt != null;

    public Priority Priority { get; set; }

    public long AverageInterval { get; set; }
    public long? DesiredInterval { get; set; }

    [JsonIgnore]
    internal bool IsRepeating => DesiredInterval > 0;

    [JsonIgnore]
    internal bool IsTask => DesiredInterval != null;

    [JsonIgnore]
    internal TaskKind TaskKind => DesiredInterval switch
    {
        null => TaskKind.Note,
        0 => TaskKind.Task,
        _ => TaskKind.RepeatingTask
    };

    [JsonIgnore]
    internal TimeSpan AverageTime { get => new(AverageInterval); set => AverageInterval = value.Ticks; }
    [JsonIgnore]
    internal TimeSpan DesiredTime { get => new(DesiredInterval ?? 0); set => DesiredInterval = value.Ticks; }

    [JsonIgnore]
    internal TimeSpan ElapsedTime => LastTimeDoneAt.HasValue ? DateTime.Now - LastTimeDoneAt.Value : DateTime.Now - CreatedAt;

    [JsonIgnore]
    internal bool IsElapsedOverAverage => IsDoneAtLeastOnce && (ElapsedTime > AverageTime);
    [JsonIgnore]
    internal double ElapsedToAverageRatio => IsDoneAtLeastOnce ? ElapsedTime / AverageTime * 100.0 : 0.0;
    [JsonIgnore]
    internal bool IsElapsedOverDesired => ElapsedTime > DesiredTime;
    [JsonIgnore]
    internal double ElapsedToDesiredRatio => IsRepeating ? ElapsedTime / DesiredTime * 100.0 : 0.0;
    [JsonIgnore]
    internal double AverageToDesiredRatio => IsRepeating ? AverageTime / DesiredTime * 100.0 : 0.0;

    public List<DateTime> TimeList = new();

    public (DateTime Time, long TaskId) AddTime(DateTime time)
    {
        TimeList.Add(time);

        OnTimeListChanged();

        return (time, Id);
    }

    public void RemoveTime(DateTime time)
    {
        TimeList.Remove(time);

        if (TimeList.Count == 0)
        {
            AverageTime = TimeSpan.Zero;

            LastTimeDoneAt = null;
        }
        else
        {
            OnTimeListChanged();
        }
    }

    public void UpdateTime(DateTime oldTime, DateTime newTime)
    {
        TimeList.Remove(oldTime);

        TimeList.Add(newTime);

        OnTimeListChanged();
    }

    private void OnTimeListChanged()
    {
        LastTimeDoneAt = TimeList.Last();

        if (TimeList.Count == 1)
            AverageTime = TimeList.First() - CreatedAt;
        else
            AverageTime = TimeSpan.FromMilliseconds(TimeList.Zip(TimeList.Skip(1), (x, y) => (y - x).TotalMilliseconds).Average());
    }
}

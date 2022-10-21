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
                MarkdownHtml = Markdown.ToHtml(_name, _markdownPipeline);
        }
    }

    private string _detailsText = string.Empty;
    public string DetailsText
    {
        get => _detailsText;
        set
        {
            _detailsText = value;

            DetailsMarkdownHtml = Markdown.ToHtml(_detailsText, _markdownPipeline);
        }
    }

    [JsonIgnore]
    internal string? MarkdownHtml { get; set; }

    [JsonIgnore]
    internal string DetailsMarkdownHtml { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public TaskKind TaskKind { get; set; }

    [JsonIgnore]
    internal bool IsTask => TaskKind != TaskKind.Note;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastTimeDoneAt { get; set; }

    [JsonIgnore]
    internal bool IsDoneAtLeastOnce => LastTimeDoneAt != null;

    [JsonIgnore]
    internal TimeSpan ElapsedTime => LastTimeDoneAt.HasValue ? DateTime.Now - LastTimeDoneAt.Value : DateTime.Now - CreatedAt;

    public TimeSpan AverageInterval { get; set; }
    public TimeSpan DesiredInterval { get; set; }

    [JsonIgnore]
    internal bool IsRepeating => DesiredInterval.Ticks > 0;

    [JsonIgnore]
    internal bool IsCompletedTask => IsTask && !IsRepeating && IsDoneAtLeastOnce;

    [JsonIgnore]
    internal bool IsElapsedOverAverage => IsDoneAtLeastOnce && (ElapsedTime > AverageInterval);
    [JsonIgnore]
    internal double ElapsedToAverageRatio => IsDoneAtLeastOnce ? ElapsedTime / AverageInterval * 100.0 : 0.0;
    [JsonIgnore]
    internal bool IsElapsedOverDesired => ElapsedTime > DesiredInterval;
    [JsonIgnore]
    internal double ElapsedToDesiredRatio => IsRepeating ? ElapsedTime / DesiredInterval * 100.0 : 0.0;
    [JsonIgnore]
    internal double AverageToDesiredRatio => IsRepeating ? AverageInterval / DesiredInterval * 100.0 : 0.0;

    public TimeSpan? AverageDuration { get; set; }
    public TimeSpan? DesiredDuration { get; set; }
    public int DurationTimedCount { get; set; }

    public List<DateTime> TimeList = new();

    public DetailsModel? Details { get; set; }

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
            AverageInterval = TimeSpan.Zero;

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
            AverageInterval = TimeList.First() - CreatedAt;
        else
            AverageInterval = TimeSpan.FromMilliseconds(TimeList.Zip(TimeList.Skip(1), (x, y) => (y - x).TotalMilliseconds).Average());
    }

    public void AddDetail(string detail)
    {
        Details ??= new();

        if (detail.StartsWith("- Date: "))
        {
            if (DateTime.TryParse(detail.Replace("- Date: ", string.Empty), out DateTime date))
            {
                Details.Date = date;
            }
        }
        else if (detail.StartsWith("- Address: "))
        {
            Details.Address = detail.Replace("- Address: ", string.Empty);
        }
        else if (detail.StartsWith("- Phone: "))
        {
            Details.Phone = detail.Replace("- Phone: ", string.Empty);
        }
        else if (detail.StartsWith("- Email: "))
        {
            if (Uri.TryCreate(detail.Replace("- Email: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                Details.Email = uri;
            }
        }
        else if (detail.StartsWith("- Website: "))
        {
            if (Uri.TryCreate(detail.Replace("- Website: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                Details.Website = uri;
            }
        }
        else if (detail.StartsWith("- Open: "))
        {
            string open = detail.Replace("- Open: ", string.Empty);

            string[] fromTill = open.Split('-');

            if (fromTill.Length == 2)
            {
                if (TimeSpan.TryParse(fromTill[0].Trim(), out TimeSpan from))
                {
                    Details.OpenFrom = from;
                }

                if (TimeSpan.TryParse(fromTill[1].Trim(), out TimeSpan till))
                {
                    Details.OpenTill = till;
                }
            }
        }
    }
}

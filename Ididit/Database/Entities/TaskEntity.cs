using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class TaskEntity
{
    [IndexDbKey]
    public long Id { get; init; }

    [IndexDbIndex]
    public long GoalId { get; set; }

    [IndexDbIndex]
    public long? PreviousId { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;

    [IndexDbIndex]
    public string DetailsText { get; set; } = string.Empty;

    [IndexDbIndex]
    public DateTime CreatedAt { get; set; }

    [IndexDbIndex]
    public DateTime? LastTimeDoneAt { get; set; }

    [IndexDbIndex]
    public TimeSpan AverageInterval { get; set; }

    [IndexDbIndex]
    public TimeSpan DesiredInterval { get; set; }

    [IndexDbIndex]
    public TimeSpan? AverageDuration { get; set; }

    [IndexDbIndex]
    public TimeSpan? DesiredDuration { get; set; }

    [IndexDbIndex]
    public int DurationTimedCount { get; set; }

    [IndexDbIndex]
    public Priority Priority { get; set; }

    [IndexDbIndex]
    public TaskKind TaskKind { get; set; }

    [IndexDbIndex]
    public DetailsEntity? Details { get; set; }
}

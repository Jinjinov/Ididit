using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class TaskEntity
{
    [IndexDbKey]
    public long Id { get; set; }

    [IndexDbIndex]
    public long GoalId { get; set; }

    [IndexDbIndex]
    public long? PreviousId { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;

    [IndexDbIndex]
    public DateTime CreatedAt { get; set; }

    [IndexDbIndex]
    public long AverageInterval { get; set; }

    [IndexDbIndex]
    public long DesiredInterval { get; set; }

    [IndexDbIndex]
    public Priority Priority { get; set; }
}

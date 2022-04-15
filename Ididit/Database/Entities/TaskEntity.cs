using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class TaskEntity
{
    [IndexDbKey(AutoIncrement = true)]
    public long Id { get; set; }

    [IndexDbIndex]
    public long GoalId { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;

    [IndexDbIndex]
    public bool IsRepeating { get; set; }

    [IndexDbIndex]
    public DateTime Created { get; set; }

    [IndexDbIndex]
    public long AverageIntervalTicks { get; set; }

    [IndexDbIndex]
    public long DesiredIntervalTicks { get; set; }
}

using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class TimeEntity
{
    [IndexDbKey]
    public DateTime Time { get; set; }

    [IndexDbIndex]
    public long TaskId { get; init; }
}

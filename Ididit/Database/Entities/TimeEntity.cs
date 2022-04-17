using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class TimeEntity
{
    [IndexDbKey(AutoIncrement = false)]
    public long Id { get => Time.Ticks; set => Time = new DateTime(value); }

    public DateTime Time { get; set; }

    [IndexDbIndex]
    public long TaskId { get; set; }
}

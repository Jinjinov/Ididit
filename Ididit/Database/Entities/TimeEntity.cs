using DnetIndexedDb;

namespace Ididit.Database.Entities;

internal class TimeEntity
{
    [IndexDbKey(AutoIncrement = false)]
    public long Ticks { get; set; }

    [IndexDbIndex]
    public long TaskId { get; set; }
}

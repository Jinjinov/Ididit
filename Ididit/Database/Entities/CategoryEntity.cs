using DnetIndexedDb;

namespace Ididit.Database.Entities;

internal class CategoryEntity
{
    [IndexDbKey]
    public long Id { get; init; }

    [IndexDbIndex]
    public long? CategoryId { get; set; }

    [IndexDbIndex]
    public long? PreviousId { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;
}

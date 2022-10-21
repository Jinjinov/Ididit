using DnetIndexedDb;
using System;

namespace Ididit.Database.Entities;

internal class DetailsEntity
{
    [IndexDbIndex]
    public DateTime? Date { get; set; }

    [IndexDbIndex]
    public string? Address { get; set; }

    [IndexDbIndex]
    public string? Phone { get; set; }

    [IndexDbIndex]
    public Uri? Email { get; set; }

    [IndexDbIndex]
    public Uri? Website { get; set; }

    [IndexDbIndex]
    public TimeSpan? OpenFrom { get; set; }

    [IndexDbIndex]
    public TimeSpan? OpenTill { get; set; }
}

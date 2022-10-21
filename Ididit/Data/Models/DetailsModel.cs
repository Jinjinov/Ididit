using System;

namespace Ididit.Data.Models;

public class DetailsModel
{
    public DateTime? Date { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public Uri? Email { get; set; }
    public Uri? Website { get; set; }
    public TimeSpan? OpenFrom { get; set; }
    public TimeSpan? OpenTill { get; set; }
}

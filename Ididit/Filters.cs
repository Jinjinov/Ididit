using System;

namespace Ididit;

public class Filters
{
    public string SearchFilter { get; set; } = string.Empty;

    public DateTime? DateFilter { get; set; }

    public Priority? PriorityFilter { get; set; }

    public Sort Sort { get; set; }

    public long ElapsedToDesiredRatioMin { get; set; }

    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    public bool ShowOnlyRepeating { get; set; }

    public bool ShowOnlyAsap { get; set; }

    public bool AlsoShowCompletedAsap { get; set; }
}

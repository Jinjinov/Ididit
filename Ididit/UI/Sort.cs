using System.ComponentModel;

namespace Ididit.UI;

public enum Sort
{
    [Description("None")]
    None,

    [Description("Name")]
    Name,

    [Description("Priority")]
    Priority,

    [Description("Elapsed time")]
    ElapsedTime,

    [Description("Elapsed time to Average time ratio")]
    ElapsedToAverageRatio,

    [Description("Elapsed time to Desired time ratio")]
    ElapsedToDesiredRatio,

    [Description("Average time to Desired time ratio")]
    AverageToDesiredRatio
}

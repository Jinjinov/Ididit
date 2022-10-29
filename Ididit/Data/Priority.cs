using System.ComponentModel;

namespace Ididit.Data;

public enum Priority
{
    [Description("no")]
    None,
    [Description("very low")]
    VeryLow,
    [Description("low")]
    Low,
    [Description("medium")]
    Medium,
    [Description("high")]
    High,
    [Description("very high")]
    VeryHigh
}

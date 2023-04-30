using System.ComponentModel;

namespace Ididit.Data;

public enum TaskKind
{
    [Description("Note")]
    Note,

    [Description("Task")] // ASAP, DateTime, Opportunity
    Task,

    [Description("Repeating task")]
    RepeatingTask
}

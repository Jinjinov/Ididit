using System.ComponentModel;

namespace Ididit.Data;

public enum TaskKind
{
    [Description("Note")]
    Note,

    [Description("Task")]
    Task,

    [Description("Repeating task")]
    RepeatingTask
}

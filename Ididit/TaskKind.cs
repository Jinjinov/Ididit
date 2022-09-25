using System.ComponentModel;

namespace Ididit;

public enum TaskKind
{
    [Description("note")]
    Note,
    [Description("task")]
    Task,
    [Description("repeating task")]
    RepeatingTask
}

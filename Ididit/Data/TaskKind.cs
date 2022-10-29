using System.ComponentModel;

namespace Ididit.Data;

public enum TaskKind
{
    [Description("note")]
    Note,
    [Description("task")]
    Task,
    [Description("repeating task")]
    RepeatingTask
}

using Blazorise;
using System.Collections.Generic;

namespace Ididit.App;

internal class Theme
{
    readonly Dictionary<string, string> ButtonSizeClassDict = new()
        {
            { "small", "btn-sm" },
            { "medium", "" },
            { "large", "btn-lg" }
        };

    public string ButtonSizeClass => ButtonSizeClassDict["medium"];

    public Size Size { get; set; } = Size.Default;
}

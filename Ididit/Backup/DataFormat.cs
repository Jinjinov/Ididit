using System.ComponentModel;

namespace Ididit.Backup;

public enum DataFormat
{
    [Description("Directory")]
    Directory,

    [Description("Json")]
    Json,

    [Description("Markdown")]
    Markdown,

    [Description("Tsv")]
    Tsv,

    [Description("Yaml")]
    Yaml,

    [Description("Google Drive")]
    GoogleDrive
}

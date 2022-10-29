using System.Threading.Tasks;

namespace Ididit.Backup;

public interface IDataExport
{
    bool UnsavedChanges { get; }

    DataFormat DataFormat { get; }

    Task ExportData();
}

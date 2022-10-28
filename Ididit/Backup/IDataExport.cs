using System.Threading.Tasks;

namespace Ididit.Backup;

public interface IDataExport
{
    bool UnsavedChanges { get; }

    Task ExportData();
}

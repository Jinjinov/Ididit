using System.Threading.Tasks;

namespace Ididit;

public interface IDataExport
{
    bool UnsavedChanges { get; }

    Task ExportData();
}

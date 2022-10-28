using System.Threading.Tasks;

namespace Ididit;

public interface IBackup
{
    bool UnsavedChanges { get; }

    Task ExportData();
}

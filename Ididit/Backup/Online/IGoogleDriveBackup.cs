using System.Threading.Tasks;

namespace Ididit.Backup.Online;

public interface IGoogleDriveBackup : IDataExport
{
    bool IsGoogleDriveAvailable { get; }

    Task ImportData();
}

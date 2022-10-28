using System.Threading.Tasks;

namespace Ididit.Online;

public interface IGoogleDriveBackup : IBackup
{
    Task ImportData();
}

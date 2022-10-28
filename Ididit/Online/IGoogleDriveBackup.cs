using Ididit.Data;
using System.Threading.Tasks;

namespace Ididit.Online;

public interface IGoogleDriveBackup
{
    Task ImportData();
    Task ExportData(IDataModel data);
}

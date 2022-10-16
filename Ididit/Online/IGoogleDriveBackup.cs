using Ididit.Data;
using System.Threading.Tasks;

namespace Ididit.Online;

public interface IGoogleDriveBackup
{
    Task<DataModel> ImportData();
    Task ExportData(IDataModel data);
}

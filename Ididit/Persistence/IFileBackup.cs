using System.IO;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal interface IFileBackup : IBackup
{
    Task ImportData(Stream stream);
}

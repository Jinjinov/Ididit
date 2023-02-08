using System.IO;
using System.Threading.Tasks;

namespace Ididit.Backup;

public interface IFileImport : IFileExtension
{
    Task ImportData(Stream stream);
}

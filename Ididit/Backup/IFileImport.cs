using System.IO;
using System.Threading.Tasks;

namespace Ididit.Backup;

public interface IFileImport
{
    string FileExtension { get; }

    Task ImportData(Stream stream);
}

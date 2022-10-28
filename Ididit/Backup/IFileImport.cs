using System.IO;
using System.Threading.Tasks;

namespace Ididit.Backup;

internal interface IFileImport
{
    string FileExtension { get; }

    Task ImportData(Stream stream);
}

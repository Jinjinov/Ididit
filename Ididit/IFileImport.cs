using System.IO;
using System.Threading.Tasks;

namespace Ididit;

internal interface IFileImport
{
    string FileExtension { get; }

    Task ImportData(Stream stream);
}

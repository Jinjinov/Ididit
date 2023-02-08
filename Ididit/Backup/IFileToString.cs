using System.IO;
using System.Threading.Tasks;

namespace Ididit.Backup;

internal interface IFileToString : IFileExtension
{
    Task<string> GetString(Stream stream);
}

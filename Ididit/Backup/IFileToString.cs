using System.IO;
using System.Threading.Tasks;

namespace Ididit.Backup;

internal interface IFileToString
{
    Task<string> GetString(Stream stream);
}

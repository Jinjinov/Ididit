using System.Threading.Tasks;

namespace Ididit.App;

public interface IUserDisplayName
{
    Task<string> GetUserDisplayName();
}

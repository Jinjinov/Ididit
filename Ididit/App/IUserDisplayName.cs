using System.Threading.Tasks;

namespace Ididit.App;

public interface IUserDisplayName
{
    string DisplayName { get; set; }

    Task<string> GetUserDisplayName();
}

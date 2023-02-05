using Google.Apis.Auth.OAuth2;

namespace Ididit.WebView.Online;

internal class GoogleDriveClientSecretsBase
{
    public virtual ClientSecrets ClientSecrets { get; } = new();
}

internal partial class GoogleDriveClientSecrets : GoogleDriveClientSecretsBase
{
}

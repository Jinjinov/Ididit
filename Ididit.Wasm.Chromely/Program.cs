using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Chromely.Core.Network;
using Chromely.NativeHosts;

IChromelyConfiguration config = DefaultConfiguration.CreateForRuntimePlatform();
config.StartUrl = "http://app/index.html";
config.UrlSchemes.Add(new UrlScheme(DefaultSchemeName.LOCALREQUEST, "http", "app", string.Empty, UrlSchemeType.LocalResource, false));
config.WindowOptions.Title = "ididit!";
config.WindowOptions.RelativePathToIconFile = "favicon.ico";
config.WindowOptions.Size = new WindowSize(1680, 1050);

if (config.CustomSettings is not null)
{
    string cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
    config.CustomSettings["CachePath"] = cachePath;
    config.CustomSettings["PersistSessionCookies"] = cachePath;
    config.CustomSettings["PersistUserPreferences"] = cachePath;
}

ThreadApt.STA();

AppBuilder
    .Create(args)
    .UseConfig<DefaultConfiguration>(config)
    .UseWindow<Window>()
    .UseApp<ChromelyBasicApp>()
    .Build()
    .Run();
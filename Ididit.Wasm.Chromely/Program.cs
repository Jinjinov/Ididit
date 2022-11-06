using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Chromely.Core.Network;
using Chromely.NativeHosts;

var config = DefaultConfiguration.CreateForRuntimePlatform();
config.WindowOptions.Title = "ididit!";
config.UrlSchemes.Add(new UrlScheme(DefaultSchemeName.LOCALREQUEST, "http", "app", string.Empty, UrlSchemeType.LocalResource, false));
config.StartUrl = "http://app/index.html";

ThreadApt.STA();

AppBuilder
    .Create(args)
    .UseConfig<DefaultConfiguration>(config)
    .UseWindow<Window>()
    .UseApp<ChromelyBasicApp>()
    .Build()
    .Run();
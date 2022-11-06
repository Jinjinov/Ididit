using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Ididit.WebView.Chromely;
using System.Reflection;

var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "ididit!";
var firstProcess = ServerAppUtil.IsMainProcess(args);
var port = ServerAppUtil.AvailablePort;

if (firstProcess)
{
    if (port != -1)
    {
        // start the kestrel server in a background thread
        AppDomain.CurrentDomain.ProcessExit += ServerAppUtil.ProcessExit;
        ServerAppUtil.BlazorTaskTokenSource = new CancellationTokenSource();
        ServerAppUtil.BlazorTask = new Task(() =>
        {
            BlazorAppBuilder.Create(args, port)
                .Build()
                .Run();

        }, ServerAppUtil.BlazorTaskTokenSource.Token, TaskCreationOptions.LongRunning);
        ServerAppUtil.BlazorTask.Start();

        // wait till its up
        while (ServerAppUtil.IsPortAvailable(port))
        {
            Thread.Sleep(1);
        }
    }

    // Save port for later use by chromely processes
    ServerAppUtil.SavePort(appName, port);
}
else
{
    // fetch port number
    port = ServerAppUtil.GetSavedPort(appName);
}

if (port != -1)
{
    // start up chromely
    var core = typeof(IChromelyConfiguration).Assembly;
    var config = DefaultConfiguration.CreateForRuntimePlatform();
    config.WindowOptions.Title = "ididit!";
    config.StartUrl = $"https://127.0.0.1:{port}";
    config.DebuggingMode = true;
    config.WindowOptions.RelativePathToIconFile = "favicon.ico";

    try
    {
        var builder = AppBuilder.Create(args);
        builder = builder.UseConfig<DefaultConfiguration>(config);
        builder = builder.UseApp<ChromelyBasicApp>();
        builder = builder.Build();
        builder.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}
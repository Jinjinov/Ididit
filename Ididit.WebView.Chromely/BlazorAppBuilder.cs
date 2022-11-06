namespace Ididit.WebView.Chromely;

public sealed class BlazorAppBuilder
{
    private readonly IHostBuilder _hostBuilder;
    private IHost? _host;

    private BlazorAppBuilder(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public static BlazorAppBuilder Create(string[] args, int port)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseUrls(new[] { $"https://127.0.0.1:{port}" });
            });

        var appBuilder = new BlazorAppBuilder(hostBuilder);

        return appBuilder;
    }
      
    public BlazorAppBuilder Build()
    {
        _host =_hostBuilder.Build();

        return this;
    }

    public void Run()
    {
        _host?.Run();
    }
}
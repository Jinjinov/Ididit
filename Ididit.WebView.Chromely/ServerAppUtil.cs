using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.NetworkInformation;

namespace Ididit.WebView.Chromely;

public static class ServerAppUtil
{
    private const int DefaultPort = 5001;
    private const int StartScan = 5050;
    private const int EndScan = 6000;
    private const string ArgumentType = "--type";

    public static Task? BlazorTask;
    public static CancellationTokenSource? BlazorTaskTokenSource;

    public static int AvailablePort
    {
        get
        {
            for (int i = StartScan; i < EndScan; i++)
            {
                if (IsPortAvailable(i))
                {
                    return i;
                }
            }

            return DefaultPort;
        }
    }

    public static bool IsMainProcess(IEnumerable<string> args)
    {
        if (args == null || !args.Any())
        {
            return true;
        }

        if (!HasArgument(args, ArgumentType))
        {
            return true;
        }

        return false;
    }

    public static bool IsPortAvailable(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

        foreach (IPEndPoint endpoint in tcpConnInfoArray)
        {
            if (endpoint.Port == port)
            {
                return false;
            }
        }

        return true;
    }

    public static void SavePort(string appName, int port)
    {
        // used to pass the port number to chromely child processes
        MemoryMappedFile mmf = MemoryMappedFile.CreateNew(appName, 4);
        MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
        accessor.Write(0, (int)port);
    }

    public static int GetSavedPort(string appName)
    {
        MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(appName, 4);
        MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
        return accessor.ReadInt32(0);
    }

    public static void ProcessExit(object? sender, EventArgs e)
    {
        // Clean up kestrel process if not taken down by OS. This can
        // occur when the app is closed from WindowController (frameless).
        Task.Run(() =>
        {
            if (BlazorTaskTokenSource != null)
            {
                WaitHandle.WaitAny(new[] { BlazorTaskTokenSource.Token.WaitHandle });
            }

            BlazorTask?.Dispose();
        });
        BlazorTaskTokenSource?.Cancel();
    }

    private static bool HasArgument(IEnumerable<string> args, string arg)
    {
        return args.Any(a => a.StartsWith(arg));
    }
}
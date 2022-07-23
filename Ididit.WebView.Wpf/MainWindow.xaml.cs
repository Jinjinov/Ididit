using Ididit.App;
using Ididit.Persistence;
using Ididit.WebView.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Ididit.WebView.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddWpfBlazorWebView();
#if DEBUG
        serviceCollection.AddBlazorWebViewDeveloperTools();
#endif
        serviceCollection.AddServices();
        serviceCollection.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        Resources.Add("services", serviceProvider);

        InitializeComponent();

        serviceProvider.UseServices();
    }
}

// Workaround for compiler error "error MC3050: Cannot find the type 'local:Main'"
// It seems that, although WPF's design-time build can see Razor components, its runtime build cannot.
public partial class Main { }

using Ididit.App;
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
        serviceCollection.AddBlazorWebView();
        serviceCollection.AddServices();

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        Resources.Add("services", serviceProvider);

        InitializeComponent();

        serviceProvider.UseServices();
    }
}

// Workaround for compiler error "error MC3050: Cannot find the type 'local:Main'"
// It seems that, although WPF's design-time build can see Razor components, its runtime build cannot.
public partial class Main { }

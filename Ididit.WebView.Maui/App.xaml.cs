namespace Ididit.WebView.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Title = "ididit!";

        window.X = 0;
        window.Y = 0;

        window.Width = 1680;
        window.Height = 1050;

        return window;
    }
}

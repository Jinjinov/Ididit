using Application = Microsoft.Maui.Controls.Application;

namespace Ididit.Maui
{
    public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}
	}
}

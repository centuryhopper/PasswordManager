using PasswordManagerMobileApp.MVVM;

namespace PasswordManagerMobileApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(Passwords), typeof(Passwords));
		Routing.RegisterRoute(nameof(Settings), typeof(Settings));
		Routing.RegisterRoute(nameof(Profile), typeof(Profile));
		Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
	}
}

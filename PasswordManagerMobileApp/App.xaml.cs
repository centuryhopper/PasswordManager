
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.MVVM;

namespace PasswordManagerMobileApp;

public partial class App : Application
{
	/*
	To create a new xaml page with dotnet cli:
		dotnet new maui-page-xaml -n [name of your xaml]
	
	*/
	public App(LoginViewModel vm)
	{
		InitializeComponent();

		MainPage = new AppShell();


		// start in login page if user isn't logged in
		// if (Preferences.Get(JwtConfig.JWT_TOKEN_NAME, null) is null)
		// {
		// 	MainPage = new LoginPage(vm);
		// }
		// else
		// {
		// }

	}
}

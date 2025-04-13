
using PasswordManagerMobileApp.ViewModels;
using PasswordManagerMobileApp.Views;

namespace PasswordManagerMobileApp;

public partial class App : Application
{
	/*
	To create a new xaml page with dotnet cli:
		dotnet new maui-page-xaml -n [name of your xaml]
	
	*/
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}

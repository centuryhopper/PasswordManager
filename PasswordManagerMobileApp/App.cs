
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.MVVM;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp;

public partial class App : Application
{
	/*
	To create a new xaml page with dotnet cli:
		dotnet new maui-page-xaml -n [name of your xaml]
	
	*/
	public App(IAccountService accountService)
	{
		// start in login page if user isn't logged in
		MainPage = new AppShell();

        // Safe: Application.Current is now initialized (because this is Application)
        // Resources.MergedDictionaries.Add(ThemeResources.Create());

        // UserAppTheme = AppTheme.Dark;
        // RequestedThemeChanged += OnRequestedThemeChanged;

        Task.Run(async () =>
        {
            if (await accountService.IsAuthenticatedAsync())
            {
                // Navigate to the LoadingPage (or home)
                await Shell.Current.GoToAsync($"//{nameof(LoadingPage)}");
            }
            else
            {
                // Default state is login
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        });

	}

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        Console.WriteLine($"Theme changed to: {e.RequestedTheme}");
    }

	// protected override void OnResume()
    // {
    //     Preferences.Default.Set("lastVisitedPage", nameof(PasswordsPage));
    // }

    // protected override void OnSleep()
    // {
    //     string lastPage = Preferences.Default.Get("lastVisitedPage", "");
	// 	Shell.Current.GoToAsync($"//{lastPage}");
    // }
}

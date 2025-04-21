using Microsoft.Extensions.Logging;
using PasswordManagerMobileApp.Services;
using PasswordManagerMobileApp.MVVM;
using CommunityToolkit.Maui;

namespace PasswordManagerMobileApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


		// Dependency injection
		builder.Services.AddSingleton<HttpClient>();

		// C# interface services
		builder.Services.AddScoped<IAccountService, AccountService>();
		builder.Services.AddScoped<IPasswordManagerService, PasswordManagerService>();

		// markups and viewmodels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<PasswordsGridVM>();
		builder.Services.AddTransient<App>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoadingPage>();
		builder.Services.AddTransient<LogoutPage>();
		builder.Services.AddTransient<PasswordsPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ProfilePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

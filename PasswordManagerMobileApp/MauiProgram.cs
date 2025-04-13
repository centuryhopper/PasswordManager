using Microsoft.Extensions.Logging;
using PasswordManagerMobileApp.Services;
using PasswordManagerMobileApp.MVVM;

namespace PasswordManagerMobileApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Dependency injection
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddScoped<IAccountService, AccountService>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoadingPage>();
		builder.Services.AddTransient<LogoutPage>();
		builder.Services.AddTransient<Passwords>();
		builder.Services.AddTransient<Settings>();
		builder.Services.AddTransient<Profile>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

using CommunityToolkit.Maui.Markup;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;

public partial class LogoutPage : ContentPage
{
	private readonly IAccountService accountService;
	public LogoutPage(IAccountService accountService)
	{
		this.accountService = accountService;

		Title = "Log Out";

		var layout = new VerticalStackLayout
		{
			Children = {
				new ActivityIndicator {
					IsRunning = true,
				}.Center(),
				new Label {
					Text = "Logging out...",
					TextColor = Colors.Black
				}.Center(),
			},
		};

		Content = layout;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		await accountService.LogoutAsync();
		await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
	}
}
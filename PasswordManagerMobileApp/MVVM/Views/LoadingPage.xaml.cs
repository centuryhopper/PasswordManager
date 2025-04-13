using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.Views;

public partial class LoadingPage : ContentPage
{
    private readonly IAccountService accountService;

    public LoadingPage(IAccountService accountService)
	{
		InitializeComponent();
        this.accountService = accountService;
    }

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		if (await accountService.IsAuthenticatedAsync())
		{
			await Shell.Current.GoToAsync($"//{nameof(Passwords)}");
		}
		else
		{
			await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
		}
	}

}
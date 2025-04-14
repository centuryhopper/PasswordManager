using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;

public partial class Passwords : ContentPage
{
    private readonly IPasswordManagerService passwordManagerService;
    private readonly PasswordsGridVM vm;

    public Passwords(IPasswordManagerService passwordManagerService, PasswordsGridVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
        this.passwordManagerService = passwordManagerService;
        this.vm = vm;
    }

	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializeAsync();
    }
}
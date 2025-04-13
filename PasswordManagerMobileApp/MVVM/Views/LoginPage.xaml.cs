using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;
using PasswordManagerMobileApp.ViewModels;

namespace PasswordManagerMobileApp.Views;

public partial class LoginPage : ContentPage
{    
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }

}


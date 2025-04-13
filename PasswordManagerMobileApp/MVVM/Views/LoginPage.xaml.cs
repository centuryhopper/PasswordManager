using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

}


namespace PasswordManagerMobileApp.MVVM;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;

        Title = "Login Page";
        BackgroundColor = Colors.Lavender;

        Content = BuildContent(vm);
    }
}

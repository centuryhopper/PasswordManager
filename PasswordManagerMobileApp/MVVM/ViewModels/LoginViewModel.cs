using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;
using CommunityToolkit.Maui.Alerts;

namespace PasswordManagerMobileApp.MVVM;
public partial class LoginViewModel : ObservableObject
{
    private readonly IAccountService _accountService;

    public LoginViewModel(IAccountService accountService)
    {
        _accountService = accountService;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool rememberMe;

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private bool isErrorVisible;

    public IAsyncRelayCommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            IsErrorVisible = true;
            return;
        }

        try
        {
            var loginDTO = new LoginDTO
            {
                Email = Email,
                Password = Password,
                RememberMe = RememberMe
            };

            var result = await _accountService.LoginAsync(loginDTO);

            if (result.Flag)
            {
                Email = string.Empty;
                Password = string.Empty;

                // Optional: Show token in a toast
                // var toast = Toast.Make(result.Token, ToastDuration.Long, 14);
                // await toast.Show();

                await Shell.Current.GoToAsync($"//{nameof(LoadingPage)}");
            }
            else
            {
                ErrorMessage = "Invalid credentials.";
                IsErrorVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Something went wrong: {ex.Message}";
            IsErrorVisible = true;
        }
    }

    partial void OnErrorMessageChanged(string value)
    {
        IsErrorVisible = !string.IsNullOrWhiteSpace(value);
    }
}

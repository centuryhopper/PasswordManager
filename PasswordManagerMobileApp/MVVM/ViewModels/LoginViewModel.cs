using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;
using Microsoft.Maui.Controls;

namespace PasswordManagerMobileApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAccountService _accountService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                IsErrorVisible = !string.IsNullOrWhiteSpace(value);
            }
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set
            {
                _isErrorVisible = value;
                OnPropertyChanged();
            }
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
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
                    await SecureStorage.SetAsync(JwtConfig.JWT_TOKEN_NAME, result.Token);
                    await Shell.Current.GoToAsync("//Passwords");
                }
                else
                {
                    ErrorMessage = "Invalid credentials.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Something went wrong: {ex.Message}";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;

        Title = "Login Page";
        BackgroundColor = Colors.Lavender;

        var layout = new VerticalStackLayout
        {
            Spacing = 25,
            Padding = 20
        };

        layout.Children.Add(
            new Image
            {
                Source = "password_manager_flaticon.png",
                HeightRequest = 150,
                WidthRequest = 200
            }
            .CenterHorizontal()
        );

        layout.Children.Add(
            new Entry
            {
                Keyboard = Keyboard.Email
            }
            .Margin(0, 10)
            .Placeholder("Enter a valid email such as abc@gmail.com")
            .Bind(Entry.TextProperty, nameof(vm.Email))
        );

        layout.Children.Add(
            new Entry
            {
                IsPassword = true
            }
            .Placeholder("Enter a secure password")
            .Bind(Entry.TextProperty, nameof(vm.Password))
        );

        layout.Children.Add(
            new HorizontalStackLayout
            {
                Children =
                {
                    new CheckBox()
                        .Bind(CheckBox.IsCheckedProperty, nameof(vm.RememberMe)),

                    new Label()
                        .Text("Remember Me")
                        .CenterVertical()
                }
            }
        );

        layout.Children.Add(
            new Button
            {
                BackgroundColor = Colors.LightBlue,
                TextColor = Colors.Black,
                FontSize = 20
            }
            .Text("Login")
            .Bind(Button.CommandProperty, nameof(vm.LoginCommand))
        );

        layout.Children.Add(
            new Label
            {
                TextColor = Colors.Red,
                FontSize = 18,
                HorizontalTextAlignment = TextAlignment.Center
            }
            .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
            .Bind(Label.IsVisibleProperty, nameof(vm.IsErrorVisible))
        );

        var registerLabel = new Label
        {
            TextColor = Colors.Blue,
            TextDecorations = TextDecorations.Underline,
            Text = "New User? Create Account",
            HorizontalOptions = LayoutOptions.Center
        };

        registerLabel.GestureRecognizers.Add(
            new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    // Navigation logic or ViewModel Command here
                    var toast = Toast.Make("register button clicked!", ToastDuration.Long, 14);
                    await toast.Show();
                })
            }
        );

        layout.Children.Add(registerLabel);

        Content = new ScrollView
        {
            Content = layout
        };
    }
}

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;

namespace PasswordManagerMobileApp.MVVM;

public partial class LoginPage
{
    private View BuildContent(LoginViewModel vm)
    {
        var layout = new VerticalStackLayout
        {
            Spacing = 25,
            Padding = 20
        };

        layout.Add(
            new Image
                {
                    Source = "password_manager_flaticon.png",
                    HeightRequest = 150,
                    WidthRequest = 200
                }
                .CenterHorizontal()
        );

        layout.Add(
            new Entry
                {
                    Keyboard = Keyboard.Email,
                    TextColor = Colors.Black,
                }
                .Margin(0, 10)
                .Placeholder("Enter a valid email such as abc@gmail.com", textColor: Colors.Grey)
                .Bind(Entry.TextProperty, nameof(vm.Email))
        );

        bool isPasswordHidden = true;

        var passwordEntry = new Entry
        {
            IsPassword = true,
            TextColor = Colors.Black,
        }
        .Placeholder("Enter a secure password", textColor: Colors.Grey)
        .Bind(Entry.TextProperty, nameof(vm.Password));

        var eyeButton = new ImageButton
        {
            Source = "eye_closed.png", // default hidden icon
            BackgroundColor = Colors.Transparent,
            HeightRequest = 24,
            WidthRequest = 24
        }
        // right-align inside the container
        .End();

        eyeButton.Clicked += (_, _) =>
        {
            isPasswordHidden = !isPasswordHidden;
            passwordEntry.IsPassword = isPasswordHidden;
            eyeButton.Source = isPasswordHidden ? "eye_closed.png" : "eye_open.png";
        };

        layout.Add(
            new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    passwordEntry.Column(0),
                    eyeButton.Column(1)
                },
            }
        );

        layout.Add(
            new HorizontalStackLayout
            {
                Children =
                {
                    new CheckBox()
                        .Bind(CheckBox.IsCheckedProperty, nameof(vm.RememberMe)),

                    new Label()
                        .Text("Remember Me", textColor: Colors.Black)
                        .CenterVertical()
                }
            }
        );

        layout.Add(
            new Button
                {
                    BackgroundColor = Colors.LightBlue,
                    TextColor = Colors.Black,
                    FontSize = 20
                }
                .Text("Login")
                .Bind(Button.CommandProperty, nameof(vm.LoginCommand))
        );

        layout.Add(
            new Label
                {
                    TextColor = Colors.Red,
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center
                }
                .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                .Bind(IsVisibleProperty, nameof(vm.IsErrorVisible))
        );

        var registerLabel = new Label
        {
            TextColor = Colors.Blue,
            TextDecorations = TextDecorations.Underline,
            Text = "New User? Create Account",
        }.CenterHorizontal();

        registerLabel.GestureRecognizers.Add(
            new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var toast = Toast.Make("register button clicked!", ToastDuration.Long, 14);
                    await toast.Show();
                })
            }
        );

        layout.Add(registerLabel);

        return new ScrollView { Content = layout };
    }
}

using Microsoft.Maui.Controls;
using PasswordManagerMobileApp.MVVM;

namespace PasswordManagerMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        // Setting up Shell properties
        FlyoutBehavior = FlyoutBehavior.Disabled;
        Title = "Password Manager";

        // Add the LoginPage as the first screen before navigation to other pages
        Items.Add(new ShellContent
        {
            Title = "Login",
            Route = nameof(LoginPage), // Register the LoginPage route
            ContentTemplate = new DataTemplate(typeof(LoginPage)),
            FlyoutItemIsVisible = false,
        });

        Items.Add(new ShellContent
        {
            Title = "Loading",
            Route = nameof(LoadingPage), // Register the LoadingPage route
            ContentTemplate = new DataTemplate(typeof(LoadingPage)),
            FlyoutItemIsVisible = false,
        });

        // Create the bottom TabBar and the ShellContent for each route
        var mainTabBar = new TabBar
        {
            Title = "Main Navigation",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems
        };

        // Adding Tabs
        mainTabBar.Items.Add(new Tab
        {
            Title = "Passwords",
            Icon = "lock.png",
            Items =
            {
                new ShellContent
                {
                    Title = "Passwords",
                    Route =  nameof(PasswordsPage),
                    ContentTemplate = new DataTemplate(typeof(PasswordsPage))
                }
            }
        });

        mainTabBar.Items.Add(new Tab
        {
            Title = "Settings",
            Icon = "cog.png",
            Items =
            {
                new ShellContent
                {
                    Title = "Settings",
                    Route = nameof(SettingsPage),
                    ContentTemplate = new DataTemplate(typeof(SettingsPage))
                }
            }
        });

        mainTabBar.Items.Add(new Tab
        {
            Title = "Profile",
            Icon = "person.png",
            Items =
            {
                new ShellContent
                {
                    Title = "Profile",
                    Route = nameof(ProfilePage),
                    ContentTemplate = new DataTemplate(typeof(ProfilePage))
                }
            }
        });

        mainTabBar.Items.Add(new Tab
        {
            Title = "Log out",
            Icon = "logout.png",
            Items =
            {
                new ShellContent
                {
                    Title = "Logout",
                    Route = nameof(LogoutPage),
                    ContentTemplate = new DataTemplate(typeof(LogoutPage))
                }
            }
        });

        // Add the main TabBar to the AppShell
        Items.Add(mainTabBar);

        // Register Routes
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(PasswordsPage), typeof(PasswordsPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
    }
}

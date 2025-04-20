using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using PasswordManagerMobileApp.Services;
using PasswordManagerMobileApp.Models;


namespace PasswordManagerMobileApp.MVVM;

public partial class PasswordsPage : ContentPage
{
    private readonly PasswordsGridVM vm;

    public PasswordsPage(PasswordsGridVM vm)
	{
        this.vm = vm;
        BindingContext = vm;

        Title = "Passwords";

        Content = BuildContent(vm);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializeAsync();
    }
}

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

        try
        {
            Content = BuildContent(vm);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"🔥 BuildContent crashed: {ex.Message}\n{ex.StackTrace}");
        }
    }
}

using CommunityToolkit.Maui.Markup;

namespace PasswordManagerMobileApp.MVVM;


public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		Title = "Settings Page";
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			Children = {
				new Label {
					Text = "This is the settings page",
					TextColor = Colors.Black
				}.Center()
			},
		};

		Content = layout;
	}
}
using CommunityToolkit.Maui.Markup;

namespace PasswordManagerMobileApp.MVVM;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		Title = "Profile Page";
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			Children = {
				new Label {
					Text = "This is the profile page",
					TextColor = Colors.Black
				}.Center()
			},
		};

		Content = layout;
	}
}
using PasswordManagerMobileApp.Services;
using CommunityToolkit.Maui.Markup;


namespace PasswordManagerMobileApp.MVVM;

public partial class LoadingPage : ContentPage
{
	private View BuildContent()
	{
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			Children = {
				new ActivityIndicator {
					IsRunning = true,
				}.Center(),
				new Label {
					Text = "Checking authentication state"
				}.Center(),
			},
		};

		return layout;
	}

}
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
		};

		layout.Add(
			new ActivityIndicator {
				IsRunning = true,
			}
		);

		layout.Add(
			new Label {
				Text = "Checking authentication state"
			}
		);

		return layout;
	}

}
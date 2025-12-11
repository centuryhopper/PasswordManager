
using CommunityToolkit.Maui.Markup;

public partial class SettingsPage : ContentPage
{
	private View BuildContent()
	{
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			Children = {
				new Label {
					Text = "SettingsPage"
				}.Center(),
			},
		};

		return layout;
	}
}

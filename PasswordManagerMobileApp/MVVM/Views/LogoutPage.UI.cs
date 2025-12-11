
using CommunityToolkit.Maui.Markup;

public partial class LogoutPage : ContentPage
{
	private View BuildContent()
	{
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			Children = {
				new Label {
					Text = "LogoutPage"
				}.Center(),
			},
		};

		return layout;
	}
}


using CommunityToolkit.Maui.Markup;

public partial class ProfilePage : ContentPage
{
	private View BuildContent()
	{
		var layout = new VerticalStackLayout
		{
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			Children = {
				new Label {
					Text = "ProfilePage"
				}.Center(),
			},
		};

		return layout;
	}
}

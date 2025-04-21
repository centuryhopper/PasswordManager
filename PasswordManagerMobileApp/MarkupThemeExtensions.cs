public static class MarkupThemeExtensions
{
    public static T TextColorTheme<T>(this T view, string key) where T : Label
    {
        view.SetDynamicResource(Label.TextColorProperty, key);
        return view;
    }

    public static T BackgroundColorTheme<T>(this T view, string key) where T : View
    {
        view.SetDynamicResource(VisualElement.BackgroundColorProperty, key);
        return view;
    }
}

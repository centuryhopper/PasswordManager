using CommunityToolkit.Maui;

public static class ThemeResources
{
    public static ResourceDictionary Create()
    {
        var dict = new ResourceDictionary();

        // Base colors
        dict.Add("BackgroundLight", Color.FromArgb("#FFFFFF"));
        dict.Add("BackgroundDark", Color.FromArgb("#121212"));
        dict.Add("TextLight", Color.FromArgb("#000000"));
        dict.Add("TextDark", Color.FromArgb("#FFFFFF"));

        // AppTheme bindings
        dict.Add("BackgroundColor", new AppThemeColor
        {
            Light = (Color)dict["BackgroundLight"],
            Dark = (Color)dict["BackgroundDark"]
        });

        dict.Add("TextColor", new AppThemeColor
        {
            Light = (Color)dict["TextLight"],
            Dark = (Color)dict["TextDark"]
        });

        return dict;
    }
}

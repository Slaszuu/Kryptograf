using System.Windows;
using Kryptograf.Enums;

namespace Kryptograf.Helpers;

public static class ThemeHelper
{
    private static readonly Dictionary<Theme, string> ThemePaths = new()
    {
        { Theme.Light, "Themes/LightTheme.xaml" },
        { Theme.Dark, "Themes/DarkTheme.xaml" }
    };

    public static void ApplyTheme(Theme theme)
    {
        if (ThemePaths.TryGetValue(theme, out var themePath) is false)
        {
            throw new ArgumentException($"Theres no path to {theme} theme.");
        }

        var dict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };

        var appResources = Application.Current.Resources;

        for (var i = appResources.MergedDictionaries.Count - 1; i >= 0; i--)
        {
            appResources.MergedDictionaries.RemoveAt(i);
        }

        appResources.MergedDictionaries.Add(dict);
    }
}

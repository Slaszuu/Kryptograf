using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kryptograf.Converters;

public class NegativeBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility && (Visibility)value == Visibility.Visible;
    }
}

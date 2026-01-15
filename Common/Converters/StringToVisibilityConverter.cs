using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeDen.Common.Converters;

/// <summary>
/// 文字列を Visibility に変換するコンバーター
/// 文字列が空でなければ Visible
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

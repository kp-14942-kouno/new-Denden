using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DenDen.Common.Converters;

/// <summary>
/// FieldType の値と ConverterParameter が一致する場合に Visible を返すコンバーター
/// </summary>
public class FieldTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string fieldType && parameter is string expected)
        {
            return string.Equals(fieldType, expected, StringComparison.OrdinalIgnoreCase)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

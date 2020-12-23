using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using YiSA.Markup.Common;

namespace YiSA.Markup.Converters
{
    /// <summary>
    /// Boolから任意の型にデータを変換します。
    /// </summary>
    public class BoolToAnyConverter : IValueConverter
    {
        public object True { get; set; } = DependencyProperty.UnsetValue;
        public object False { get; set; } = DependencyProperty.UnsetValue;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? True : False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value,True) ? Boxes.True : Boxes.False;
        }
    }
}
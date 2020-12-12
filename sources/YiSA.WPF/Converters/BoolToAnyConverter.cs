using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using YiSA.WPF.Common;

namespace YiSA.WPF.Converters
{
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
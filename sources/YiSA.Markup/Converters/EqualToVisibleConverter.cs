﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YiSA.Markup.Converters
{
    public class EqualToVisibleConverter : IValueConverter
    {
        public object? Value { get; set; }

        public Visibility True { get; set; } = Visibility.Visible;
        public Visibility False { get; set; } = Visibility.Collapsed;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(Value,value))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
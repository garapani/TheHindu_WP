using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheHindu.Converters
{
    public sealed class IsStringNotNullConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value != null)
            {
                var temp = (string)value;
                return string.IsNullOrEmpty(temp) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class IsStringNullOrEmptyToVisibleConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value == null) return Visibility.Visible;
            var temp = (string)value;
            return string.IsNullOrEmpty(temp) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheHindu.Converters
{
    public class NullOrEmptyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = value as string;
            return string.IsNullOrEmpty(temp) ? Visibility.Visible : Visibility.Collapsed;
            //    Visibility visibility;

            //    value = value != null;

            //    if (parameter != null)
            //    {
            //        bool isReversed = Boolean.Parse((string)parameter);

            //        visibility = (bool)value ? isReversed ? Visibility.Collapsed : Visibility.Visible : isReversed ? Visibility.Visible : Visibility.Collapsed;
            //    }
            //    else
            //    {
            //        visibility = (bool)value ? Visibility.Visible : Visibility.Collapsed;
            //    }

            //    return visibility;
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
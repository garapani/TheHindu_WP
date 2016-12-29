using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Converters
{
    public class NullOrEmptyVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = value as string;
            if (string.IsNullOrEmpty(temp))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
            //Visibility visibility;

            //value = value != null;

            //if (parameter != null)
            //{
            //    bool isReversed = Boolean.Parse((string)parameter);

            //    visibility = (bool)value ? isReversed ? Visibility.Collapsed : Visibility.Visible : isReversed ? Visibility.Visible : Visibility.Collapsed;
            //}
            //else
            //{
            //    visibility = (bool)value ? Visibility.Visible : Visibility.Collapsed;
            //}

            //return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}
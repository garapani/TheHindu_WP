using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DotNetApp.Toolkit.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if (value != null)
            {
                if (parameter != null)
                {
                    bool isReversed = Boolean.Parse((string)parameter);

                    visibility = (bool)value ? isReversed ? Visibility.Collapsed : Visibility.Visible : isReversed ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    visibility = (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
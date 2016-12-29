using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheHindu.Converters
{
    public class IsoToLocalTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DateTime.Now;
            DateTime dateTime = (DateTime)value;
            return dateTime.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}
using System;
using System.Windows.Data;

namespace TheHindu.Converters
{
    public class FontSizePercantageToWinPhoneConverter : IValueConverter
    {
        private const double Ratio = 1.5625;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var d = (double)value;
                return d * Ratio;
            }
            catch { return value; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var d = (double)value;
                return d / Ratio;
            }
            catch { return value; }
        }
    }
}
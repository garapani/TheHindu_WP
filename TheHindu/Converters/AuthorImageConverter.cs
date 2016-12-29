using System;
using System.Globalization;
using System.Windows.Data;

namespace TheHindu.Converters
{
    public class AuthorImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string image = string.Empty;

            switch ((string)value)
            {
                case "Joe Fedewa":
                    image = "/Assets/Views/Main/HubJoe.png";
                    break;

                case "Ashley King":
                    image = "/Assets/Views/Main/HubAshley.png";
                    break;

                case "Mark Jackson":
                    image = "/Assets/Views/Main/HubMark.png";
                    break;

                case "Rob Jackson":
                    image = "/Assets/Views/Main/HubRob.png";
                    break;
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}
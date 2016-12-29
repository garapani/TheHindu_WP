using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using TheHindu.Helper;

namespace TheHindu.Converters
{
    public class TextPlainConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            var plainText = String.Empty;
            try
            {
                if (value != null)
                {
                    var text = value.ToString();
                    if (text.Length > 0)
                    {
                        plainText = Utility.DecodeHtml(text);
                        if (parameter != null)
                        {
                            var maxLength = 0;
                            Int32.TryParse(parameter.ToString(), out maxLength);
                            if (maxLength > 0)
                            {
                                plainText = plainText.Truncate(maxLength);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("App.xaml.cs:" + exception);
                }
            }
            return plainText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
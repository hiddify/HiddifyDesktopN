using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using v2rayN.Mode;

namespace v2rayN.Converters
{
    public class LocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProxyModeEnum)
                return ((ProxyModeEnum)value).ToLocalizedDescriptionString();
            string resourceName = value.ToString();
            return Application.Current.FindResource(resourceName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

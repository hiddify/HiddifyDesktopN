using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace v2rayN.Converters
{
    public class SizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };


        private static bool IsNumber(object value)
        {
            return value is sbyte || value is byte ||
                   value is short || value is ushort ||
                   value is int || value is uint ||
                   value is long || value is ulong ||
                   value is float || value is double ||
                   value is decimal;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !IsNumber(value))
            {
                return null;
            }

            long fileSizeInBytes = System.Convert.ToInt64(value);
            if (fileSizeInBytes == 0)
            {
                return "0 B";
            }

            var sizeIndex = (int)Math.Floor(Math.Log(fileSizeInBytes, 1024));
            var size = fileSizeInBytes / Math.Pow(1024, sizeIndex);
            var sizeSuffix = SizeSuffixes[sizeIndex];
            var formattedSize = string.Format("{0:n1}", size);

            return $"{formattedSize} {sizeSuffix}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
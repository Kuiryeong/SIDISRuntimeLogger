using System;
using System.Globalization;
using System.Windows.Data;

namespace SIDISRuntimeLogger.Converters
{
    internal class BooleanReversConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

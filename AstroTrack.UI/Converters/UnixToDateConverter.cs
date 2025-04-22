using System;
using System.Globalization;
using System.Windows.Data;
using AstroTrack.Common;

namespace AstroTrack.UI.Converters
{
    /// <summary>
    /// Converts a Unix timestamp (long) to a DateTime for display in XAML.
    /// </summary>
    public class UnixToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long timestamp)
            {
                return DateTimeHelper.FromUnixTimestamp(timestamp);
            }
            if (value is int intVal)
            {
                return DateTimeHelper.FromUnixTimestamp(intVal);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return DateTimeHelper.ToUnixTimestamp(dt);
            }
            return Binding.DoNothing;
        }
    }
}

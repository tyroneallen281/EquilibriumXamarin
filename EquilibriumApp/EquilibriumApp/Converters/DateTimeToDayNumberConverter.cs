using System;
using System.Globalization;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Converters
{
    public class DateTimeToDayNumberConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime?)value;
            return val?.ToString("dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

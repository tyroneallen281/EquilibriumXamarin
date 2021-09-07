using System;
using System.Globalization;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Converters
{
    public class LabelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = new Color();

            var isReminder = (bool)value;

            if (isReminder)
            {
                color = (Color)Application.Current.Resources["PrimaryColor"];
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
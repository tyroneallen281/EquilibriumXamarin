using System;
using System.Globalization;
using EquilibriumApp.Mobile.Enums;
using EquilibriumApp.Mobile.Models.ViewModels;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Converters
{
    public class IsActiveToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorType = (EColorModelType) parameter;
            var isActive = (bool) value;
           
            var colormodel = isActive
                ? new ColorModel()
                {
                    BackgroundColor = Color.FromHex("#FF1DA1F3"),
                    IconColor = Color.White,
                    TextColor = Color.White
                }
                : new ColorModel()
                {
                    BackgroundColor = Color.FromHex("#FFF1F2F6"),
                    IconColor = Color.FromHex("#FFA6AAB6"),
                    TextColor = Color.Default
                };

            switch (colorType)
            {
                case EColorModelType.BackColor:
                    return colormodel.BackgroundColor;
                case EColorModelType.TextColor:
                    return colormodel.TextColor;
                case EColorModelType.IconColor:
                    return colormodel.IconColor;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

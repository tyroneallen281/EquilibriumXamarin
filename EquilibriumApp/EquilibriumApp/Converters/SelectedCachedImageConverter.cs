using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Converters
{
    public class SelectedCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transformation = new List<ITransformation>();
            var isReminder = (bool)value;

            if (isReminder)
            {
                transformation.Add(new TintTransformation() { EnableSolidColor = true, HexColor = "#4069B1" });
            }
            return transformation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
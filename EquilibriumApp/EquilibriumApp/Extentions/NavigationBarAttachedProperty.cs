﻿using EquilibriumApp.Views.Home;
using Xamarin.Forms;

namespace EquilibriumApp.Extentions
{
    public static class NavigationBarAttachedProperty
    {
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.CreateAttached(
                "TextColor", 
                typeof(Color), 
                typeof(NavigationBarAttachedProperty), 
                Color.Default);

        public static Color GetTextColor(BindableObject view)
        {
            return (Color)view.GetValue(TextColorProperty);
        }

        public static void SetTextColor(BindableObject view, Color value)
        {
            view.SetValue(TextColorProperty, value);

            var page = view as Page;
            var parent = page?.Parent as CustomNavigationPage;
            parent?.ApplyNavigationTextColor(page);
        }
    }
}

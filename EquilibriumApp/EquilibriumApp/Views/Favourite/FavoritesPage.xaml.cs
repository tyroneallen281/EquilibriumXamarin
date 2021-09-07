using EquilibriumApp.DI;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Mobile.ViewModels.Favourite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.Favourite
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
            this.BindingContext = Locator.Instance.Resolve(typeof(FavoritesViewModel));
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
            }
            catch (Exception e)
            {
            }
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            

             //await Navigation.PushAsync(new ViewClassDetailsPage(e.Item as ClassEvent).WithinNavigationPage());
        }

       
    }
}
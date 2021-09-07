using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Mobile.Views.CSPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RX.Api.Client;
using EquilibriumApp.DI;

namespace EquilibriumApp.Mobile.Views.Class
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchClassPage : ContentPage
    {
        public SearchClassPage()
        {
            InitializeComponent();
            this.BindingContext = Locator.Instance.Resolve(typeof(SearchClassesViewModel));
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

        private void ItemsView_OnSelectedItemChanged(object sender, EventArgs e)
        {
            var context = BindingContext as SearchClassesViewModel;
            foreach (DateModel dateModel in ItemsView.ItemsSource)
            {
                dateModel.ShowColor = false;
            }
            var model = ItemsView.SelectedItem as DateModel;
            if (model != null)
                model.ShowColor = true;
            context.LoadEventsCommand.Execute(null);
        }
    }
}
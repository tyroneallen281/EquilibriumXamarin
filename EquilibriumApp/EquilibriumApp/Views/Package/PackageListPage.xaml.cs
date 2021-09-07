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

namespace EquilibriumApp.Mobile.Views.Package
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PackageListPage : ContentPage
    {
        public PackageListPage()
        {
            InitializeComponent();
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
    }
}
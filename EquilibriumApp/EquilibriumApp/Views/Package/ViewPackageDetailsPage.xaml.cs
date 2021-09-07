using FFImageLoading.Forms;
using System;
using System.Linq;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.Package
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewPackageDetailsPage : ContentPage
    {
        public ViewPackageDetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

        }
    }
}
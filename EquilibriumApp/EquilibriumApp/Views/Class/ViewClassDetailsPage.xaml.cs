using FFImageLoading.Forms;
using System;
using System.Linq;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.Class
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewClassDetailsPage : ContentPage
    {
        public ViewClassDetailsPage()
        {
            InitializeComponent();
            if (Navigation.NavigationStack.Count() < 2)
            {
                this.ToolbarItems.Add(new ToolbarItem
                {
                    IconImageSource = "close.png",
                    Command = new Command(async () =>
                              {
                                  await this.Navigation.PopModalAsync();
                              })
                });
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

        }
    }
}
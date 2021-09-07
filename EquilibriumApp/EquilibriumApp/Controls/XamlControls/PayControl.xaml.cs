using Acr.UserDialogs;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.ViewModels.Favourite;
using EquilibriumApp.Mobile.ViewModels.Payment;
using EquilibriumApp.Mobile.Views.CSPages;
using EquilibriumApp.Models.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Controls.XamlControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayControl : BaseContentPage
    {
        public string StartUrl { get; set; }
         public PayControl()
        {
            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Close",
                Command = new Command(() =>
                {
                    Navigation.PopModalAsync();
                })
            });
            Title = $"Payment";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PayWebView.Navigating += PayWebViewOnNavigating;
            PayWebView.Navigated += PayWebViewOnNavigated;
        }
               
        protected override void OnDisappearing()
        {
            PayWebView.Navigated -= PayWebViewOnNavigated;
            PayWebView.Navigating -= PayWebViewOnNavigating;
        }

        private void PayWebViewOnNavigated(object sender, WebNavigatedEventArgs webNavigatingEventArgs)
        {
            LoadingControl.Visibility = false;
            LoadingControl.IsVisible = false;
        }

        private void PayWebViewOnNavigating(object sender, WebNavigatingEventArgs webNavigatingEventArgs)
        {
            if (webNavigatingEventArgs != null)
            {
                var context = BindingContext as PayViewModel;
                if (!string.IsNullOrEmpty(webNavigatingEventArgs.Url))
                {
                   
                    if (webNavigatingEventArgs.Url.ToLower().Contains("paymentaccept"))
                    {
                        Thread.Sleep(5000);
                        //AnalyticsService.TrackEvent("purchase", new Dictionary<object, object> { { "EventId", Event.EventId }, { "EventName", Event.Name }});
                        MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                        //Task.Run(() => UserDialogs.Instance.AlertAsync("Payment Successful."));
                        Navigation.PopAsync();
                        context.PaymentResultCommand.Execute(null);
                    }

                    if (webNavigatingEventArgs.Url.ToLower().Contains("paymentdecline") || webNavigatingEventArgs.Url.ToLower().Contains("paymentcomplete"))
                    {
                        Thread.Sleep(5000);
                        //AnalyticsService.TrackEvent("purchase_declined", new Dictionary<object, object> { { "EventId", Event.EventId }, { "EventName", Event.Name } });
                        Task.Run(() => UserDialogs.Instance.AlertAsync("Payment Failed, Please Try Again"));
                        Navigation.PopAsync();

                    }

                }

            }
        }
       
    }
}
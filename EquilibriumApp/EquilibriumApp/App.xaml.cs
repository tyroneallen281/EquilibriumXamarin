using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RX.Api.Client;
using System.Net.Http;
using EquilibriumApp.Mobile.Views.Class;
using EquilibriumApp.DI;
using EquilibriumApp.Services.Navigation;
using EquilibriumApp.ViewModels;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;

namespace EquilibriumApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SetCultureToUSEnglish();
            BuildDependencies();
            InitNavigation();


        }
        private Task InitNavigation()
        {
            var navigationService = Locator.Instance.Resolve<INavigationService>();

            return navigationService.NavigateToAsync<MainViewModel>();
        }
        public void BuildDependencies()
        {
            Locator.Instance.Build();
        }

        public static Page CreatePage(Page page)
        {
            var navigationPage = new NavigationPage(page);

            NavigationPage.SetHasNavigationBar(page, false);
            return navigationPage;
        }

        protected override void OnStart()
        {
             // Handle when your app starts
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
               
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Received");
                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                
            };
            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Dismissed");
            };
        }
        private void SetCultureToUSEnglish()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.CurrencySymbol = "R";
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

        }
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

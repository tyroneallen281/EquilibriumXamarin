using EquilibriumApp.Mobile.Models.AppModels;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquilibriumApp.Helpers
{
    public static class Settings
    {


        private const string AuthenticationAccessKey = "authaccess_key";
        private static readonly string AuthenticationAccessDefault = string.Empty;
        public static string AuthenticationAccess
        {
            get
            {
                return Xamarin.Essentials.Preferences.Get(AuthenticationAccessKey, AuthenticationAccessDefault);
            }
            set
            {
                Xamarin.Essentials.Preferences.Set(AuthenticationAccessKey, value);
            }
        }


        private const string AccountIdKey = "authaccess_accountid";
        private static readonly string AccountIdDefault = string.Empty;
        public static string AccountId
        {
            get
            {
                return Xamarin.Essentials.Preferences.Get(AccountIdKey, AccountIdDefault);
            }
            set
            {
                Xamarin.Essentials.Preferences.Set(AccountIdKey, value);
            }
        }
    }
}

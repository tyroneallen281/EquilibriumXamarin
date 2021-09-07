using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.AppModels;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using Plugin.FirebasePushNotification;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquilibriumApp.Services
{
    public static class AuthenticationService
    {
        public static bool ActiveAccount()
        {
            return !string.IsNullOrEmpty(Settings.AuthenticationAccess);
        }

        public static void Logout()
        {
            Settings.AuthenticationAccess = string.Empty;
            ClientBaseFactory.SetBearerToken(string.Empty);
            MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
            MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
            CrossFirebasePushNotification.Current.UnsubscribeAll();
        }

        public static void SetAuthTokenData(string authData)
        {
            Settings.AuthenticationAccess = authData;
            var authObject = string.IsNullOrEmpty(authData) ? new AuthResponse() : Settings.AuthenticationAccess.Deserialize<AuthResponse>();
            ClientBaseFactory.SetBearerToken(authObject?.AccessToken);
        }

        public static async Task<AccountUserModel> GetUserInformation()
        {
            var jsonResult = await HttpService.GetAsync(AppSettings.AuthUrl, $"api/Accounts/User", null,
                ClientBaseFactory.BearerToken);
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var user = jsonResult.Deserialize<AccountUserModel>();
                Settings.AccountId = user.Id;
                CrossFirebasePushNotification.Current.Subscribe(user.Id);
                return user;
            }
            return null;
        }

        public static async Task<bool> PostUserInformation(AccountUserModel user)
        {
            var jsonResult = await HttpService.PostAsync(AppSettings.AuthUrl, "api/accounts/update", user, null,
                              null,
                             ClientBaseFactory.BearerToken);

            return jsonResult.IsSuccess;
        }

    }
}

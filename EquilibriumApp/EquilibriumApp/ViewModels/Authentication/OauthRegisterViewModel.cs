using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using Microsoft.AppCenter.Analytics;
using Plugin.Connectivity;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Authentication
{
    public class OauthRegisterViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private readonly bool _isModal;
       
        private string _emailForgotPassswordErrorText = "Email is required";
        private LoginModel _loginModel;
        private RegisterModel _registerModel;
        private readonly IUserClient _userClient;

        public OauthRegisterViewModel(IUserClient userClient)
        {
            _userClient = userClient;
        }

        public override Task InitializeAsync(object navigationData)
        {
            RegisterModel = (RegisterModel)navigationData;
            return base.InitializeAsync(navigationData);
        }

        public RegisterModel RegisterModel
        {
            get => _registerModel;
            set => SetProperty(ref _registerModel, value);
        }

        public string EmailForgotPasswordErrorText
        {
            get => _emailForgotPassswordErrorText;
            set => SetProperty(ref _emailForgotPassswordErrorText, value);
        }

     
      
         public ICommand RegisterCommand => new Command(Register);
       
        public async void Register()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await UserDialogs.Instance.AlertAsync("No Internet connection, Please connect to the Internet and try again");
                IsBusy = false;
                return;
            }
            RegisterModel.DateOfBirth = DateTime.Now.AddYears(-12);

            if (string.IsNullOrEmpty(RegisterModel.Email.Trim()) || string.IsNullOrEmpty(RegisterModel.FirstName) || string.IsNullOrEmpty(RegisterModel.LastName) || string.IsNullOrEmpty(RegisterModel.PhoneNumber) ||  RegisterModel.DateOfBirth == null)
            {
                UserDialogs.Instance.Toast("Please fill in all required fields", TimeSpan.FromSeconds(5));
                return;
            }
            if (DateTime.Now.AddYears(-10) < RegisterModel.DateOfBirth)
            {
                UserDialogs.Instance.Toast("Please check your selected date of birth.", TimeSpan.FromSeconds(5));
                return;
            }

            RegisterModel.PhoneNumber = RegisterModel.PhoneNumber.Trim();
            if (!ValidationHelper.ValidatePhoneNumber(RegisterModel.PhoneNumber))
            {
                UserDialogs.Instance.Toast("Phone number is invalid, please remove the leading 0 and ensure full number entered.", TimeSpan.FromSeconds(5));
                return;
            }

            RegisterModel.Email = RegisterModel.Email.Trim();
            if (!ValidationHelper.ValidateEmail(RegisterModel.Email))
            {
                UserDialogs.Instance.Toast("Email is invalid", TimeSpan.FromSeconds(5));
                return;
            }

            IsBusy = true;
            RegisterModel.Username = RegisterModel.PhoneNumber;
          
            var registerResult = await HttpService.PostAsync(AppSettings.AuthUrl, "api/accounts/register", RegisterModel);
            if (registerResult.IsSuccess)
            {
                await UserDialogs.Instance.AlertAsync("You have successfully signed up.", "Success");
                var signin = new Dictionary<string, string>
                    {
                        {"username", RegisterModel.ExternalAccountId},
                        {"password", RegisterModel.AccessToken},
                        {"client_id", AppSettings.ClientIdFacebook},
                        {"grant_type", "password"}
                    };

                var signinResponse = await HttpService.PostAsync(AppSettings.AuthUrl, "token", string.Empty, signin);
                if (signinResponse != null)
                {
                    AuthenticationService.SetAuthTokenData(signinResponse.Data);
                    try
                    {
                        await _userClient.AssignUserToMemberAccountsAsync();
                    }
                    catch (Exception)
                    {

                    }

                    MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                    MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
                    await _navigationService.NavigateBackModalAsync<LoginRegisterViewModel>(true);
                    IsBusy = false;
                }
                else
                {
                    Analytics.TrackEvent("oauth_reg_failed_logn", new Dictionary<string, string> { { "signinResponse_message", signinResponse.ErrorMessage }, { "UserId", RegisterModel.ExternalAccountId } });
                    UserDialogs.Instance.Toast("An error occurred logging in.", TimeSpan.FromSeconds(5));
                }


            }
            else
            {
                Analytics.TrackEvent("oauth_reg_failed", new Dictionary<string, string> { { "signinResponse_message", registerResult.ErrorMessage }, { "UserId", RegisterModel.ExternalAccountId } });
                UserDialogs.Instance.Toast("An error occurred logging in.", TimeSpan.FromSeconds(5));
            }
            
            IsBusy = false;
        }

       
    }
}
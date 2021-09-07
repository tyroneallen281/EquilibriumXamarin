using Acr.UserDialogs;
using EquilibriumApp.DI;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mappers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.User;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Authentication
{
    public class LoginRegisterViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private readonly bool _isModal;
        private bool _signIn;
        private bool _signUp;
        private bool _forgetPassword;
        private string _forgoPasswordCellNumber;
        private readonly IUserClient _userClient;

        public LoginRegisterViewModel(IUserClient userClient)
        {
            _userClient = userClient;
            SetSignUp();
            LoginModel = new LoginModel();
            RegisterModel = new RegisterModel();
        }


        public string ForgoPasswordCellNumber
        {
            get => _forgoPasswordCellNumber;
            set => SetProperty(ref _forgoPasswordCellNumber, value);
        }

        public void SetSignIn()
        {
            SignIn = true;
            SignUp = false;
            ForgetPassword = false;
        }

        public void SetSignUp()
        {
            SignIn = false;
            SignUp = true;
            ForgetPassword = false;
        }

        public void SetForgotPassword()
        {
            SignIn = false;
            SignUp = false;
            ForgetPassword = true;
        }

        private string _emailForgotPassswordErrorText = "Email is required";
        private LoginModel _loginModel;
        private RegisterModel _registerModel;

        public LoginModel LoginModel
        {
            get => _loginModel;
            set => SetProperty(ref _loginModel, value);
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

        public bool SignIn
        {
            get => _signIn;
            set => SetProperty(ref _signIn, value);
        }

        public bool ForgetPassword
        {
            get => _forgetPassword;
            set => SetProperty(ref _forgetPassword, value);
        }

        public bool SignUp
        {
            get => _signUp;
            set => SetProperty(ref _signUp, value);
        }
      
        public ICommand LoginWithFacebookCommand => new Command(FacebookLogin);
        public ICommand SignInPageCommand => new Command(SetSignIn);
        public ICommand ForgotPasswordPageCommand => new Command(SetForgotPassword);
        public ICommand SignUpPageCommand => new Command(SetSignUp);
        public ICommand LoginCommand => new Command(Login);
        public ICommand RegisterCommand => new Command(Register);
        public ICommand ForgotPasswordCommand => new Command(ForgotPassword);

        public async void ForgotPassword()
        {
            if (string.IsNullOrEmpty(ForgoPasswordCellNumber))
            {
                await UserDialogs.Instance.AlertAsync("Please enter a cellphone number");
                return;
            }
           
            IsBusy = true;
            try
            {
                var apiJsonValue = await _userClient.SendOtpSmsAsync(ForgoPasswordCellNumber.Trim());
                await UserDialogs.Instance.AlertAsync(
                        $"An SMS has been sent to you, please complete the reset.");
                await _navigationService.NavigateToAsync<ResetPasswordViewModel>();
                SetSignIn();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                UserDialogs.Instance.Toast("An Error Occured. Could not find user.", TimeSpan.FromSeconds(5));
            }
         

            IsBusy = false;
        }

        public async void Register()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await UserDialogs.Instance.AlertAsync("No Internet connection, Please connect to the Internet and try again");
                    IsBusy = false;
                    return;
                }

                if (string.IsNullOrEmpty(RegisterModel.Email.Trim()) || string.IsNullOrEmpty(RegisterModel.FirstName) || string.IsNullOrEmpty(RegisterModel.LastName) || string.IsNullOrEmpty(RegisterModel.PhoneNumber) || string.IsNullOrEmpty(RegisterModel.Password) || RegisterModel.DateOfBirth == null)
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

                if (!ValidationHelper.ValidatePassword(RegisterModel.Password))
                {
                    UserDialogs.Instance.Toast("Invalid Password, minimun 6 characters, Number, Upper and Lower Case Letters", TimeSpan.FromSeconds(5));
                    return;
                }
                IsBusy = true;
                RegisterModel.Username = RegisterModel.PhoneNumber;
                RegisterModel.ConfirmPassword = RegisterModel.Password;
                var registerResult = await HttpService.PostAsync(AppSettings.AuthUrl, "api/accounts/register", RegisterModel);
                if (registerResult.IsSuccess)
                {
                    await UserDialogs.Instance.AlertAsync("You have successfully signed up.", "Success");
                    var signin = new Dictionary<string, string>
                    {
                        {"username", RegisterModel.Username},
                        {"password", RegisterModel.Password},
                        {"client_id", AppSettings.ClientIdLogin},
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
                        catch (Exception e)
                        {
                            Crashes.TrackError(e);
                        }

                        MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                        MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
                        await _navigationService.NavigateBackModalAsync<LoginRegisterViewModel>(true);
                       
                        IsBusy = false;
                    }
                    else
                    {
                        Analytics.TrackEvent("registration_failed_login", new Dictionary<string, string> { { "signinResponse_message", signinResponse.ErrorMessage }, { "UserId", RegisterModel.PhoneNumber } });
                        UserDialogs.Instance.Toast("An error occurred logging in.", TimeSpan.FromSeconds(5));
                    }


                }
                else
                {
                    Analytics.TrackEvent("registration_failed", new Dictionary<string, string> { { "signinResponse_message", registerResult.ErrorMessage }, { "UserId", RegisterModel.PhoneNumber } });
                    UserDialogs.Instance.Toast("An error occurred in registration.", TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                UserDialogs.Instance.Toast("An error occurred registration.", TimeSpan.FromSeconds(5));
            }

            IsBusy = false;
        }

        public async void FacebookLogin()
        {
            var authResult = await WebAuthenticator.AuthenticateAsync(
            new Uri($"https://m.facebook.com/dialog/oauth?client_id={AppSettings.FacebookClientId}&redirect_uri=https://app.baobabtech.co.za/RX.Pay/Auth/Callback"),
            new Uri("equilibriumapp://"));

            var accessToken = authResult?.AccessToken;
            LoadingText = "Signing user in...";
            IsBusy = true;
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpResponse = await httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,picture,name,first_name,last_name,email,birthday,gender&access_token={accessToken}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                     throw new AuthenticationException();
                }

                string data = await httpResponse.Content.ReadAsStringAsync();
                var facebookModel = data.Deserialize<FacebookModel>();
               
                var signin = new Dictionary<string, string>
                {
                    {"username", facebookModel.id},
                    {"password", accessToken},
                    {"client_id", AppSettings.ClientIdFacebook},
                    {"grant_type", "password"}
                };

                var signinResponse = await HttpService.PostAsync(AppSettings.AuthUrl, "token", string.Empty, signin);
                if (signinResponse.IsSuccess)
                {
                    AuthenticationService.SetAuthTokenData(signinResponse.Data);
                    
                    MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                    MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
                    await _navigationService.NavigateBackModalAsync<LoginRegisterViewModel>(true);
                }
                else
                {
                    facebookModel.AccessToken = accessToken;
                    await _navigationService.NavigateToAsync<OauthRegisterViewModel>(facebookModel.MapToRegister());
                }

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast("An error occurred logging in via facebook, Please try again.", TimeSpan.FromSeconds(5));
            }
            IsBusy = false;
        }

        public async void Login()
        {
            try
            {
                AuthenticationService.Logout();
                if (string.IsNullOrEmpty(LoginModel.PhoneNumber) || string.IsNullOrEmpty(LoginModel.Password))
                {
                    UserDialogs.Instance.Toast("Your username and password cannot be empty", TimeSpan.FromSeconds(5));
                    return;
                }
                LoginModel.PhoneNumber = LoginModel.PhoneNumber.Trim();
                if (!ValidationHelper.ValidatePhoneNumber(LoginModel.PhoneNumber))
                {
                    UserDialogs.Instance.Toast("Phone number is invalid, please remove the leading 0 and ensure full number entered.", TimeSpan.FromSeconds(5));
                    return;
                }
                if (!CrossConnectivity.Current.IsConnected)
                {
                    UserDialogs.Instance.Toast("No internet connection", TimeSpan.FromSeconds(5));
                    return;
                }
                LoadingText = "Signing user in...";
                IsBusy = true;
                var signin = new Dictionary<string, string>
                    {
                        {"username", LoginModel.PhoneNumber.Trim()},
                        {"password", LoginModel.Password},
                        {"client_id", AppSettings.ClientIdLogin},
                        {"grant_type", "password"}
                    };

                var signinResponse = await HttpService.PostAsync(AppSettings.AuthUrl, "token", string.Empty, signin);
                if (signinResponse.IsSuccess)
                {
                    AuthenticationService.SetAuthTokenData(signinResponse.Data);
                    MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                    MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
                    await _navigationService.NavigateBackModalAsync<LoginRegisterViewModel>(true);
                    IsBusy = false;
                }
                else
                {
                    Analytics.TrackEvent("login_failed", new Dictionary<string, string> { { "signinResponse_message", signinResponse.ErrorMessage }, { "UserId", LoginModel.PhoneNumber } });
                    UserDialogs.Instance.Toast("Your cell phone number and password combination might be incorrect, please try again", TimeSpan.FromSeconds(5));
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            IsBusy = false;
        }

        public List<string> Genders
        {
            get
            {
                return Enum.GetNames(typeof(Gender)).Select(b => b).ToList();
            }
        }
    }
}
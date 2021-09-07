using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Plugin.Share;
using Plugin.Share.Abstractions;
using RX.Api.Client;
using RX.Mobile.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.User
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        private readonly IUserClient _userClient;
        private PasswordResetModel _password;

        public PasswordResetModel Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        public ResetPasswordViewModel(IUserClient userClient)
        {
            _userClient = userClient;
            Password = new PasswordResetModel();
        }

        private ICommand _resetPasswordCommand;

        public ICommand ResetPasswordCommand
        {
            get
            {
                return _resetPasswordCommand ?? (_resetPasswordCommand = new Command(async () =>
                {
                    try
                    {
                        if (!CrossConnectivity.Current.IsConnected)
                        {
                            UserDialogs.Instance.Toast("No internet connection", TimeSpan.FromSeconds(5));
                            return;
                        }
                        if (string.IsNullOrEmpty(Password.Otp) || string.IsNullOrEmpty(Password.Password))
                        {
                            UserDialogs.Instance.Toast("Please enter required fields.", TimeSpan.FromSeconds(5));
                            return;
                        }
                        if (!ValidationHelper.ValidatePassword(Password.Password))
                        {
                            UserDialogs.Instance.Toast("Invalid password, minimun 6 characters, number, upper and lower case letters", TimeSpan.FromSeconds(5));
                            return;
                        }
                        IsBusy = true;


                        var jsonResult = await _userClient.ResetPasswordAsync(Password);
                        await UserDialogs.Instance.AlertAsync($"Password Reset.");
                        await _navigationService.NavigateBackModalAsync<ResetPasswordViewModel>(true);
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                        UserDialogs.Instance.Toast("Reset Failed", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }
    }
}
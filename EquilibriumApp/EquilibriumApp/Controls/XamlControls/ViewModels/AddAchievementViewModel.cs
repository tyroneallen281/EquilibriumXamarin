using Acr.UserDialogs;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Interfaces;
using EquilibriumApp.Models.ViewModels;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Controls.XamlControls.ViewModels
{
    public class AddAchievementViewModel : BaseViewModel
    {
        private readonly IUserAchievementClient _userAchievementClient;

        
        public AddAchievementViewModel(IUserAchievementClient userAchievementClient)
        {
            _userAchievementClient = userAchievementClient;
        }

        private UserAchievementModel userAchievementModel;

        public UserAchievementModel UserAchievementModel
        {
            get { return userAchievementModel; }
            set { SetProperty(ref userAchievementModel, value); }
        }

        private string packageResultString;

        public string PackageResultString
        {
            get { return packageResultString; }
            set { SetProperty(ref packageResultString, value); }
        }
        public override Task InitializeAsync(object navigationData)
        {
            UserAchievementModel = new UserAchievementModel();
            return base.InitializeAsync(navigationData);
        }

        private ICommand _addAchievementCommand;

        public ICommand AddAchievementCommand
        {
            get
            {
                return _addAchievementCommand ?? (_addAchievementCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        UserAchievementModel.EntryDate = DateTime.Now;
                        var userAchievements = await _userAchievementClient.AddAchievementAsync(UserAchievementModel);
                        UserDialogs.Instance.Toast("Achievement Saved", TimeSpan.FromSeconds(5));
                        MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                        await _navigationService.NavigateBackModalAsync<BookingResultViewModel>(true);
                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error saving achievement, please  try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }

    }
}
using Acr.UserDialogs;
using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using Plugin.FirebasePushNotification;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.User
{
    public class MeViewModel : BaseViewModel
    {
        private readonly IUserClient _userClient;
        private readonly IClassBookingClient _classBookingClient;
        private readonly IMemberPackageClient _memberPackageClient;
        private readonly IMemberPackagePeriodClient _memberPackagePeriodClient;
        private readonly IUserAchievementClient _userAchievementClient;
        
        public MeViewModel(IUserClient userClient, IClassBookingClient classBookingClient, IMemberPackageClient memberPackageClient, IMemberPackagePeriodClient memberPackagePeriodClient, IUserAchievementClient userAchievementClient)
        {
            _userClient = userClient;
            _classBookingClient = classBookingClient;
            _memberPackageClient = memberPackageClient;
            _memberPackagePeriodClient = memberPackagePeriodClient;
            _userAchievementClient = userAchievementClient;
            BookedClasses = new ObservableCollection<ClassBookingViewModel>();
            RefreshCommand.Execute(null);
            SetBookingView();
            MessagingCenter.Subscribe<Messages>(this, "update_bookings", model =>
            {
                RefreshCommand.Execute(null);
            });

        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }
        private UserModel _user;

        public UserModel User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private bool _showNoBookings;

        public bool ShowNoBookings
        {
            get { return _showNoBookings; }
            set { SetProperty(ref _showNoBookings, value); }
        }

        private bool _showNoPackages;

        public bool ShowNoPackages
        {
            get { return _showNoPackages; }
            set { SetProperty(ref _showNoPackages, value); }
        }

        private bool _showBooking;
        public bool ShowBooking
        {
            get => _showBooking;
            set => SetProperty(ref _showBooking, value);
        }
        private bool _showPackage;

        public bool ShowPackage
        {
            get => _showPackage;
            set => SetProperty(ref _showPackage, value);
        }
        private bool _showAchievement;
        public bool ShowAchievement
        {
            get => _showAchievement;
            set => SetProperty(ref _showAchievement, value);
        }
        private bool _loggedIn;

        public bool LoggedIn
        {
            get => _loggedIn;
            set => SetProperty(ref _loggedIn, value);
        }

        private ObservableCollection<ClassBookingViewModel> _bookedClasses;

        public ObservableCollection<ClassBookingViewModel> BookedClasses
        {
            get { return _bookedClasses; }
            set { SetProperty(ref _bookedClasses, value); }
        }

        private ObservableCollection<MemberPackagePeriodModel> _memberPackages;

        public ObservableCollection<MemberPackagePeriodModel> MemberPackages
        {
            get { return _memberPackages; }
            set { SetProperty(ref _memberPackages, value); }
        }


        private ObservableCollection<UserAchievementModel> _userAchievements;

        public ObservableCollection<UserAchievementModel> UserAchievements
        {
            get { return _userAchievements; }
            set { SetProperty(ref _userAchievements, value); }
        }

        private ICommand _profileCommand;

        public ICommand ProfileCommand
        {
            get
            {
                return _profileCommand ?? (_profileCommand = new Command(async () =>
                {
                    await _navigationService.NavigateToPopupAsync<ProfileViewModel>(true);
                }));
            }
        }

        public void SetBookingView()
        {
            ShowBooking = true;
            ShowPackage = false;
            ShowAchievement = false;
        }
        public void SetPackageView()
        {
            ShowBooking = false;
            ShowPackage = true;
            ShowAchievement = false;
        }
        public void SetAchievementView()
        {
            ShowBooking = false;
            ShowPackage = false;
            ShowAchievement = true;
        }
        public ICommand SetBookingViewCommand => new Command(SetBookingView);
        public ICommand SetPackageViewCommand => new Command(SetPackageView);
        public ICommand SetAchievementViewCommand => new Command(SetAchievementView);

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                    LoggedIn = AuthenticationService.ActiveAccount();
                    if (AuthenticationService.ActiveAccount())
                    {
                        LoadBookingsCommand.Execute(null);
                        LoadPackagesCommand.Execute(null);
                        LoadAchievementsCommand.Execute(null);
                        RefreshUserCommand.Execute(null);
                    }
                  
                }));
            }
        }

        private ICommand _refreshUserCommand;

        public ICommand RefreshUserCommand
        {
            get
            {
                return _refreshUserCommand ?? (_refreshUserCommand = new Command(async () =>
                {
                    IsBusy = true;
                    try
                    {
                        User = await _userClient.GetUserDataAsync();
                        if (string.IsNullOrWhiteSpace(User.ProfileUrl))
                        {
                            if (User.Gender == Gender.Male)
                            {
                                User.ProfileUrl = "https://lmsblob.blob.core.windows.net/profile-images/person-male.png";
                            }
                            else if (User.Gender == Gender.Female)
                            {
                                User.ProfileUrl = "https://lmsblob.blob.core.windows.net/profile-images/person-female.png";
                            }
                        }
                        
                        Settings.AccountId = User.Id.ToString();
                        CrossFirebasePushNotification.Current.Subscribe(User.Id.ToString());
                    }
                    catch (Exception ex)
                    {

                        
                    }
                   
                    IsBusy = false;
                }));
            }
        }


        private ICommand _loadBookingsCommand;

        public ICommand LoadBookingsCommand
        {
            get
            {
                return _loadBookingsCommand ?? (_loadBookingsCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        var location = await LocationHelper.GetLocationAsync();
                        var bookingResults = await _classBookingClient.GetUserBookingsAsync( DateTime.Today,null,null, location.Longitude, location.Latitude);
                        var bookingsViewModel = new List<ClassBookingViewModel>();
                        foreach (var item in bookingResults)
                        {
                            bookingsViewModel.Add(new ClassBookingViewModel(item));
                        }
                        BookedClasses = new ObservableCollection<ClassBookingViewModel>(bookingsViewModel.OrderBy(_ => _.Model.ClassEvent.Start));
                        foreach (var booking in BookedClasses)
                        {
                            booking.Model.ClassEvent.UserBookedClassId = booking.Model.ClassBookingId;
                            booking.Model.ClassEvent.UserBookedClass = true;
                        }
                        ShowNoBookings = !BookedClasses.Any();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error loading classes please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;

                  
                }));
            }
        }

        private ICommand _loadAchievementsCommand;

        public ICommand LoadAchievementsCommand
        {
            get
            {
                return _loadAchievementsCommand ?? (_loadAchievementsCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        var userAchievements = await _userAchievementClient.GetUserAchievementsAsync(null);
                        UserAchievements = new ObservableCollection<UserAchievementModel>(userAchievements.OrderBy(_ => _.EntryDate));
                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error loading achievements, please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;


                }));
            }
        }

        private ICommand _loadPackagesCommand;

        public ICommand LoadPackagesCommand
        {
            get
            {
                return _loadPackagesCommand ?? (_loadPackagesCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        var packageResults = await _memberPackagePeriodClient.GetActivePaidPackagesForAccountAsync(null);
                        MemberPackages = new ObservableCollection<MemberPackagePeriodModel>(packageResults);
                        ShowNoPackages = !MemberPackages.Any();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error loading classes please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;


                }));
            }
        }


        private ICommand _classItemSelectedCommand;

        public ICommand ClassItemSelectedCommand
        {
            get
            {
                return _classItemSelectedCommand ?? (_classItemSelectedCommand = new Command(async (selectedItem) =>
                {
                    var classBooking = selectedItem as ClassBookingViewModel;

                    await _navigationService.NavigateToPopupAsync<ViewClassDetailsViewModel>(classBooking.Model.ClassEvent,true);
                }));
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new Command(async (selectedItem) =>
                {
                    await _navigationService.NavigateToPopupAsync<LoginRegisterViewModel>(true);
                }));
            }
        }

        private ICommand _addAchievementCommand;

        public ICommand AddAchievementCommand
        {
            get
            {
                return _addAchievementCommand ?? (_addAchievementCommand = new Command(async (selectedItem) =>
                {
                    await _navigationService.NavigateToPopupAsync<AddAchievementViewModel>(null, true);
                }));
            }
        }
    }
}
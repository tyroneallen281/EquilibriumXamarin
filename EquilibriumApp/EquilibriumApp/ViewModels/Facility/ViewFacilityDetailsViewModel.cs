using Acr.UserDialogs;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Services;
using Plugin.Share;
using Plugin.Share.Abstractions;
using RX.Api.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Events
{
    public class ViewFacilityDetailsViewModel : BaseViewModel
    {
        private double _scroll;
        private bool _eventDetails;
        private readonly IFacilityClient _facilityClient;
        private readonly IMemberPackageClient _memberPackageClient;

        public ViewFacilityDetailsViewModel(IFacilityClient facilityClient, IMemberPackageClient memberPackageClient)
        {
            _facilityClient = facilityClient;
            _memberPackageClient = memberPackageClient;

        }
       
        public bool EventDetails
        {
            get => _eventDetails;
            set => SetProperty(ref _eventDetails, value);
        }

        public double Scroll
        {
            get { return _scroll; }
            set { SetProperty(ref _scroll, value); }
        }

        public void SetEventDetails()
        {
            EventDetails = true;
        }

        public void SetLeaderboard()
        {
            EventDetails = false;
        }

        public ICommand EventDetailsPageCommand => new Command(SetEventDetails);
        public ICommand LeaderboardPageCommand => new Command(SetLeaderboard);
        private FacilityViewModel facility;

        public FacilityViewModel Facility
        {
            get { return facility; }
            set { SetProperty(ref facility, value); }
        }


        private MemberPackageModel memberPackage;

        public MemberPackageModel MemberPackage
        {
            get { return memberPackage; }
            set { SetProperty(ref memberPackage, value); }
        }

       
        public override Task InitializeAsync(object navigationData)
        {
            PageTitle = "Facility Details";
            var facilityId = navigationData;
            Task.Run(async () =>
            {
                IsBusy = true;
                try
                {
                    var  facilityModel = await _facilityClient.Get2Async((int)facilityId);
                    Facility = new FacilityViewModel(facilityModel);
                    MessagingCenter.Send(this, "MoveToRegion");
                }
                catch (System.Exception)
                {
                    UserDialogs.Instance.Toast("An Error Occured.", TimeSpan.FromSeconds(5));
                }
                IsBusy = false;
            });
            
            return base.InitializeAsync(navigationData);
        }

        private ICommand _classScheduleCommand;

        public ICommand ClassScheduleCommand
        {
            get
            {
                return _classScheduleCommand ?? (_classScheduleCommand = new Command(async () =>
                {
                    await _navigationService.NavigateToAsync<ClassScheduleViewModel>(Facility.Model);
                }));
            }
        }


        private ICommand _packageListCommand;

        public ICommand PackageListCommand
        {
            get
            {
                return _packageListCommand ?? (_packageListCommand = new Command(async () =>
                {
                    await _navigationService.NavigateToAsync<PackageListViewModel>(Facility.Model);
                }));
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                   
                }));
            }
        }

        private ICommand _websiteCommand;

        public ICommand WebsiteCommand
        {
            get
            {
                return _websiteCommand ?? (_websiteCommand = new Command(async () =>
                {
                    await Launcher.OpenAsync(new Uri(Facility.Model.Website));
                }));
            }
        } 

        private ICommand _callCommand;

        public ICommand CallCommand
        {
            get
            {
                return _callCommand ?? (_callCommand = new Command(async () =>
                {
                   
                    await Launcher.OpenAsync(new Uri($"tel:{Facility.Model.Contact}"));
                }));
            }
        }


        private ICommand _directionsCommand;

        public ICommand DirectionsCommand
        {
            get
            {
                return _directionsCommand ?? (_directionsCommand = new Command(async () =>
                {
                    var location = new Location(Facility.Model.Latitude, Facility.Model.Longitude);
                    var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                    await Map.OpenAsync(location, options);
                }));
            }
        }

        private ICommand _shareCommand;

        public ICommand ShareCommand
        {
            get
            {
                return _shareCommand ?? (_shareCommand = new Command(() =>
                {
                    CrossShare.Current.Share(new ShareMessage()
                    {
                        Text = $"Join me at {Facility.Model.Name}.",
                        Title = Facility.Model.Name,
                        Url = "https://bit.ly/3dBG7ZR",
                    }, new ShareOptions { ChooserTitle = Facility.Model.Name });
                }));
            }
        }


    }
}
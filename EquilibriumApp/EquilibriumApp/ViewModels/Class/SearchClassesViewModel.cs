using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services.Navigation;
using Plugin.FirebasePushNotification;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Events
{
    public class SearchClassesViewModel : ViewModelBase
    {
        private readonly ICalendarClient _calendarClient;
        private readonly IFacilityClient _facilityClient;
        
        public SearchClassesViewModel(ICalendarClient calendarClient, IFacilityClient facilityClient)
        {
            Radius = 30;
            _calendarClient = calendarClient;
            _facilityClient = facilityClient;
            Classes = new ObservableCollection<ClassEventModel>();
            Dates = new ObservableCollection<DateModel>();
            SetClassesView();
            var date = DateTime.Today;

            for (int i = 0; i < 31; i++)
            {
                Dates.Add(new DateModel(date));
                date = date.AddDays(1);
            }

            SelectedDate = Dates.FirstOrDefault();
            MessagingCenter.Subscribe<Messages>(this, "update_bookings", model =>
            {
                RefreshCommand.Execute(null);
            });
            RefreshCommand.Execute(null);
            CrossFirebasePushNotification.Current.Subscribe("equilibrium_push");
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        private readonly INavigation _navigation;

        private ObservableCollection<DateModel> _dates;

        public ObservableCollection<DateModel> Dates
        {
            get { return _dates; }
            set { SetProperty(ref _dates, value); }
        }

        private DateModel _selectedDate;
        public DateModel SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }


        private bool _showNoFacilities;
        public bool ShowNoFacilities
        {
            get { return _showNoFacilities; }
            set { SetProperty(ref _showNoFacilities, value); }
        }

        private bool _showNoEvents;

        public bool ShowNoEvents
        {
            get { return _showNoEvents; }
            set { SetProperty(ref _showNoEvents, value); }
        }

        private ObservableCollection<ClassEventModel> _classes;

        public ObservableCollection<ClassEventModel> Classes
        {
            get { return _classes; }
            set { SetProperty(ref _classes, value); }
        }

        private ObservableCollection<FacilityModel> _facilities;

        public ObservableCollection<FacilityModel> Facilities
        {
            get { return _facilities; }
            set { SetProperty(ref _facilities, value); }
        }

        private string _seachQuery;

        public string SearchQuery
        {
            get { return _seachQuery; }
            set {
                SetProperty(ref _seachQuery, value);
                RefreshCommand.Execute(null);
            }
        }

        private string _radiusText;

        public string RadiusText
        {
            get { return _radiusText; }
            set { SetProperty(ref _radiusText, value); }
        }

        private double _radius;

        public double Radius
        {
            get { return _radius; }
            set
            {
                SetProperty(ref _radius, value);
                _radius = Math.Round(_radius);
                RadiusText = _radius <= 80 ? $"{_radius} KM" : "ALL";
                RefreshCommand.Execute(null);
            }
        }
       
        private ICommand _profileCommand;

        public ICommand ProfileCommand
        {
            get
            {
                return _profileCommand ?? (_profileCommand = new Command(() =>
                {
                    //if (Helper.NoActiveAccount)
                    //{
                    //    _navigation.PushModalAsync(Helper.CreatePage(new LoginRegisterPage(true, null)));
                    //    return;
                    //}
                    //_navigation.PushModalAsync(Helper.CreatePage(new ProfilePage(true)));
                }));
            }
        }

        private bool _showClasses;
        public bool ShowClasses
        {
            get => _showClasses;
            set => SetProperty(ref _showClasses, value);
        }
        private bool _showFacility;

        public bool ShowFacility
        {
            get => _showFacility;
            set => SetProperty(ref _showFacility, value);
        }

        

        public void SetClassesView()
        {
            ShowClasses = true;
            ShowFacility = false;
        }
        public void SetFacilityView()
        {
            ShowClasses = false;
            ShowFacility = true;
        }

        public ICommand SetClassesViewCommand => new Command(SetClassesView);
        public ICommand SetFacilityViewCommand => new Command(SetFacilityView);

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                    LoadEventsCommand.Execute(null);
                    LoadFacilitiesCommand.Execute(null);
                }));
            }
        }

        private ICommand _loadEventsCommand;

        public ICommand LoadEventsCommand
        {
            get
            {
                return _loadEventsCommand ?? (_loadEventsCommand = new Command(async () =>
                {
                    try
                    {
                        if (_calendarClient == null)
                        {
                            return;
                        }

                        IsBusy = true;
                        Classes = new ObservableCollection<ClassEventModel>();
                        ShowNoEvents = false;
                        var location = await LocationHelper.GetLocationAsync();
                        var selectedDate = SelectedDate;
                        if (selectedDate == null || selectedDate.Date == DateTime.Today)
                        {
                            selectedDate = new DateModel(DateTime.Now);
                        }

                        var classesResult = await _calendarClient.GetAppClassEventsAsync(selectedDate.Date, selectedDate.Date.AddDays(1), SearchQuery, location?.Longitude, location?.Latitude, Convert.ToInt32(Radius), null,null);
                        Classes = new ObservableCollection<ClassEventModel>(classesResult.OrderBy(_ => _.Start));
                        ShowNoEvents = !Classes.Any();
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        //UserDialogs.Instance.Toast("Error loading classes please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }

        private ICommand _loadFacilitiesCommand;

        public ICommand LoadFacilitiesCommand
        {
            get
            {
                return _loadFacilitiesCommand ?? (_loadFacilitiesCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        ShowNoFacilities = false;
                        var location = await LocationHelper.GetLocationAsync();
                        var facilityResults = await _facilityClient.GetAppFacilitiesAsync(SearchQuery, location?.Longitude, location?.Latitude, Convert.ToInt32(Radius));
                        Facilities = new ObservableCollection<FacilityModel>(facilityResults);
                        ShowNoFacilities = !Facilities.Any();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

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
                    await _navigationService.NavigateToPopupAsync<ViewClassDetailsViewModel>(selectedItem, true);
                }));
            }
        }


        private ICommand _facilityItemSelectedCommand;

        public ICommand FacilityItemSelectedCommand
        {
            get
            {
                return _facilityItemSelectedCommand ?? (_facilityItemSelectedCommand = new Command(async (selectedItem) =>
                {
                    var faciltiy = selectedItem as FacilityModel;
                    await _navigationService.NavigateToPopupAsync<ViewFacilityDetailsViewModel>(faciltiy.FacilityId, true);
                }));
            }
        }
    }
}
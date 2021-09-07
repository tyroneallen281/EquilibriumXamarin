using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services.Navigation;
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
    public class ClassScheduleViewModel : ViewModelBase
    {
        private readonly ICalendarClient _calendarClient;
        private readonly IFacilityClient _facilityClient;
        
        public ClassScheduleViewModel(ICalendarClient calendarClient, IFacilityClient facilityClient)
        {
            _calendarClient = calendarClient;
            _facilityClient = facilityClient;
            Classes = new ObservableCollection<ClassEventModel>();
            Dates = new ObservableCollection<DateModel>();
            var date = DateTime.Today;

            for (int i = 0; i < 31; i++)
            {
                Dates.Add(new DateModel(date));
                date = date.AddDays(1);
            }

            SelectedDate = Dates.FirstOrDefault();
        }

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is FacilityModel)
            {
                Facility = (FacilityModel)navigationData;
                PageTitle = $"{Facility.Name} - Schedule";
            }
            if (navigationData is ClassModel)
            {
                Class = (ClassModel)navigationData;
                PageTitle = $"{Class.Name} - Class Schedule";
            }
            

            RefreshCommand.Execute(null);
            MessagingCenter.Subscribe<Messages>(this, "update_bookings", model =>
            {
                RefreshCommand.Execute(null);
            });
            return base.InitializeAsync(navigationData);
        }

       
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

        private FacilityModel _facility;
        public FacilityModel Facility
        {
            get { return _facility; }
            set { SetProperty(ref _facility, value); }
        }
        private ClassModel _class;
        public ClassModel Class
        {
            get { return _class; }
            set { SetProperty(ref _class, value); }
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

        private string _seachQuery;

        public string SearchQuery
        {
            get { return _seachQuery; }
            set {
                SetProperty(ref _seachQuery, value);
                RefreshCommand.Execute(null);
            }
        }



        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                    LoadEventsCommand.Execute(null);
                
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
                    if (Facility == null && Class == null)
                    {
                        return;
                    }
                    try
                    {
                        IsBusy = true;
                        ShowNoEvents = false;
                        var location = await LocationHelper.GetLocationAsync();
                        var selectedDate = SelectedDate;
                        if (selectedDate == null || selectedDate.Date == DateTime.Today)
                        {
                            selectedDate = new DateModel(DateTime.Now);
                        }

                        var classesResult = await _calendarClient.GetAppClassEventsAsync(selectedDate.Date, selectedDate.Date.AddDays(1), SearchQuery, location?.Longitude, location?.Latitude, 100, Facility?.FacilityId, Class?.ClassId);
                        Classes = new ObservableCollection<ClassEventModel>(classesResult.OrderBy(_ => _.Start));
                        ShowNoEvents = !Classes.Any();

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
                    await _navigationService.NavigateToAsync<ViewClassDetailsViewModel>(selectedItem);
                }));
            }
        }
    }
}
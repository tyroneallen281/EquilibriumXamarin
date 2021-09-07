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
    public class BookingResultViewModel : BaseViewModel
    {
        private readonly IMemberPackagePeriodClient _memberPackagePeriodClient;

        private ClassBookingModel classBooking { get; set; }

        public BookingResultViewModel(IMemberPackagePeriodClient memberPackagePeriodClient)
        {
            _memberPackagePeriodClient = memberPackagePeriodClient;
        }

        private MemberPackagePeriodModel package;

        public MemberPackagePeriodModel Package
        {
            get { return package; }
            set { SetProperty(ref package, value); }
        }

        private string packageResultString;

        public string PackageResultString
        {
            get { return packageResultString; }
            set { SetProperty(ref packageResultString, value); }
        }
        public override Task InitializeAsync(object navigationData)
        {
            classBooking = navigationData as ClassBookingModel;
            Task.Run(async () =>
            {   
                if (classBooking.MemberPackagePeriodId != null)
                {
                    Package = await _memberPackagePeriodClient.GetByIdAsync(classBooking.MemberPackagePeriodId);
                    PackageResultString = $"{Package.ClassLeft} Classes left";
                }
            });
            return base.InitializeAsync(navigationData);
        }

        private ICommand _addCalendarCommand;

        public ICommand AddCalendarCommand
        {
            get
            {
                return _addCalendarCommand ?? (_addCalendarCommand = new Command(async () =>
                {
                    bool result = await DependencyService.Get<ICalendar>().AddClassToCalender(classBooking.ClassEvent, 30, App.Current.Resources["PrimaryColor"] is Color ? (Color)App.Current.Resources["PrimaryColor"] : new Color());
                    if (result)
                    {
                        UserDialogs.Instance.Toast("Added to your calendar", TimeSpan.FromSeconds(5));
                    }
                }));
            }
        }

        private ICommand _continueCommand;

        public ICommand ContinueCommand
        {
            get
            {
                return _continueCommand ?? (_continueCommand = new Command(async () =>
                {
                    MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                    await _navigationService.NavigateBackModalAsync<BookingResultViewModel>(true);
                }));
            }
        }
    }
}
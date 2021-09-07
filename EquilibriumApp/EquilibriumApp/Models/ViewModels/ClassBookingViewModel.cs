using Acr.UserDialogs;
using EquilibriumApp.DI;
using EquilibriumApp.Mobile.Extentions;
using RX.Api.Client;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class ClassBookingViewModel : BaseViewModel
    {
        public ClassBookingViewModel(ClassBookingModel model)
        {
            Model = model;
        }
        private ClassBookingModel @class;

        public ClassBookingModel Model
        {
            get { return @class; }
            set { SetProperty(ref @class, value); }
        }

        private bool canCheckIn;
        public bool CanCheckIn
        {
            get
            {
                if (Model.CheckedIn)
                {
                    canCheckIn =  false;
                    return canCheckIn;
                }

                var checkInTime = Model.ClassEvent.Start.AddMinutes(-30);
                if (checkInTime < DateTimeOffset.Now && Model.ClassEvent.End > DateTimeOffset.Now && (Model.FacilityDistance?.IsClose ?? false))
                {
                    canCheckIn = true;
                }
                else
                {
                    canCheckIn = false;
                }

                return canCheckIn;
            }
            set { SetProperty(ref canCheckIn, value); }
        }
      
        private ICommand _loadCheckinCommand;

        public ICommand LoadCheckinCommand
        {
            get
            {
                return _loadCheckinCommand ?? (_loadCheckinCommand = new Command(async () =>
                {
                    try
                    {
                        var _bookingClient = (IClassBookingClient)Locator.Instance.Resolve(typeof(IClassBookingClient));
                        var result = await _bookingClient.PutCheckInClassBookingAsync(Model.ClassBookingId);
                        if (result.Result)
                        {
                            CanCheckIn = false;
                            UserDialogs.Instance.Toast("Checked In, Thank You.", TimeSpan.FromSeconds(5));
                        }
                        else{
                            UserDialogs.Instance.Toast("Error checking in, Please try again.", TimeSpan.FromSeconds(5));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error checking in, Please try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;


                }));
            }
        }

    }
}
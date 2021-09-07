using Acr.UserDialogs;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using Microsoft.AppCenter.Crashes;
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
    public class ViewClassDetailsViewModel : BaseViewModel
    {
        private double _scroll;
        private bool _eventDetails;
        private readonly IClassBookingClient _classBookingClient;
        private readonly ICalendarClient _calendarClient;
        private readonly IMemberPackageClient _memberPackageClient;
        private readonly IMemberPackagePeriodClient _memberPackagePeriodClient;
      
        public ViewClassDetailsViewModel(IClassBookingClient classBookingClient, IMemberPackageClient memberPackageClient, IMemberPackagePeriodClient memberPackagePeriodClient, ICalendarClient calendarClient)
        {
            _classBookingClient = classBookingClient;
            _calendarClient = calendarClient;
            _memberPackageClient = memberPackageClient;
            _memberPackagePeriodClient = memberPackagePeriodClient;
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
        private ClassEventViewModel @class;

        public ClassEventViewModel Class
        {
            get { return @class; }
            set { SetProperty(ref @class, value); }
        }

       
        private MemberPackagePeriodModel memberPackagePeriod;

        public MemberPackagePeriodModel MemberPackagePeriod
        {
            get { return memberPackagePeriod; }
            set { SetProperty(ref memberPackagePeriod, value); }
        }

       
        public override Task InitializeAsync(object navigationData)
        {
            PageTitle = "Class Details";
            var classEvent = navigationData as ClassEventModel;
            Class = new ClassEventViewModel(classEvent);
            MessagingCenter.Subscribe<Messages>(this, "update_bookings", model =>
            {
                RefreshCommand.Execute(null);
            });

            return base.InitializeAsync(navigationData);
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
                        Text = $"Join me at {Class.Model.FacilityName} for {Class.Model.ClassName} at {Class.Model.StartDateString}.",
                        Title = Class.Model.ClassName,
                        Url = "https://bit.ly/3dBG7ZR",
                    }, new ShareOptions { ChooserTitle = Class.Model.ClassName });
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
                    try
                    {
                        var classResult = await _calendarClient.GetAppClassEventAsync(Class.Model.CalendarEventId);
                        if (classResult != null)
                        {
                            Class = new ClassEventViewModel(classResult);
                        }
                    }
                    catch (Exception)
                    {

                    }
                   
                }));
            }
        }

        private ICommand _bookCommand;

        public ICommand BookCommand
        {
            get
            {
                return _bookCommand ?? (_bookCommand = new Command(async () =>
                {
                    IsBusy = true;
                    try
                    {
                        if (!Class.HasPackage && Class.ShowCallFacility)
                        {
                            await Launcher.OpenAsync(new Uri($"tel:{Class.Model.FacilityContactNumber}"));
                            return;
                        }

                        if (!AuthenticationService.ActiveAccount())
                        {
                            await _navigationService.NavigateToPopupAsync<LoginRegisterViewModel>(true);
                        }
                        else
                        {
                            var bookClassModel = new ClassBookingRequestModel()
                            {
                                ClassEventId = Class.Model.CalendarEventId,
                                MemberPackageId = Class.Model.MemberPackagePeriodId
                            };

                            var result = await _classBookingClient.PostClassBookingAsync(bookClassModel);
                           
                            if (result == null || !result.Result)
                            {
                                UserDialogs.Instance.Toast(result.ResultMessage ?? "Booking failed, please try again..", TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                await PaymentService.ProcessPaymentBooking(_navigationService, result.Data);
                            }
                           
                           
                        }
                    }
                    catch (System.Exception e)
                    {
                        Crashes.TrackError(e);
                        UserDialogs.Instance.Toast("An Error Occured.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }

        private ICommand _cancelBookingCommand;

        public ICommand CacelBookingCommand
        {
            get
            {
                return _cancelBookingCommand ?? (_cancelBookingCommand = new Command(async () =>
                {
                    var dlg =
                            await UserDialogs.Instance.ConfirmAsync(
                                "Are you sure you want to cancel?", "Cancel Booking", "YES", "NO");
                    if (dlg)
                    {
                        IsBusy = true;
                        try
                        {

                            var result = await _classBookingClient.PutCancelClassBookingAsync(Class.Model.UserBookedClassId);

                            if (result == null || !result.Result)
                            {
                                UserDialogs.Instance.Toast(result.ResultMessage ?? "Cancel failed, please try again.", TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                                UserDialogs.Instance.Toast("Booking cancelled, hope to see you next time.", TimeSpan.FromSeconds(5));
                            }



                        }
                        catch (System.Exception e)
                        {
                            Crashes.TrackError(e);
                            UserDialogs.Instance.Toast("An Error Occured.", TimeSpan.FromSeconds(5));
                        }
                    }
                   
                    IsBusy = false;
                }));
            }
        }

        private ICommand _viewFacilityCommand;

        public ICommand ViewFacilityCommand
        {
            get
            {
                return _viewFacilityCommand ?? (_viewFacilityCommand = new Command(async () =>
                {
                    await _navigationService.NavigateToAsync<ViewFacilityDetailsViewModel>(Class.Model.FacilityId);
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
                    var _class = Class.Model;
                    await Launcher.OpenAsync(new Uri($"tel:{_class.FacilityContactNumber}"));
                }));
            }
        }

    }
}
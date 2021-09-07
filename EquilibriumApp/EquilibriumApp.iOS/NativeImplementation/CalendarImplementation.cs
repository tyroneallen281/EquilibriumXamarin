using EquilibriumApp.Mobile.Interfaces;
using EquilibriumApp.Mobile.iOS;
using EventKit;
using Foundation;
using RX.Api.Client;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.iOS.NativeImplementation;

[assembly: Dependency(typeof(CalendarImplementation))]

namespace EquilibriumApp.iOS.NativeImplementation
{
    public class CalendarImplementation : ICalendar
    {
        protected CreateEventEditViewDelegate EventControllerDelegate;

        // protected EKEventStore eventStore;
        public CalendarImplementation()
        {
            // eventStore = new EKEventStore();
        }

        public async Task<bool> AddClassToCalender(ClassEventModel _class, double reminder = 30, Color? color = null)
        {
            try
            {
                var granted = await AppStore.Current.EventStore.RequestAccessAsync(EKEntityType.Event);//, (bool granted, NSError e) =>

                if (granted.Item1)
                {
                    EKEvent newEvent = EKEvent.FromStore(AppStore.Current.EventStore);
                    newEvent.AddAlarm(EKAlarm.FromDate(DateTimeToNsDate(_class.Start.DateTime)));
                    newEvent.StartDate = DateTimeToNsDate(_class.Start.DateTime);
                    newEvent.EndDate = DateTimeToNsDate(_class.End.DateTime);
                    newEvent.Title = _class.Name;
                    newEvent.Location = _class.FacilityName ?? "";
                    newEvent.Notes = _class.ClassDescription.ScrubHtml();
                    newEvent.Calendar = AppStore.Current.EventStore.DefaultCalendarForNewEvents;
                    NSError e;
                    var eventController =
                        new EventKitUI.EKEventEditViewController { EventStore = AppStore.Current.EventStore };

                    EventControllerDelegate = new CreateEventEditViewDelegate(eventController);
                    eventController.EditViewDelegate = EventControllerDelegate;
                    eventController.Event = newEvent;
                   // NSError e;
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        var window = UIApplication.SharedApplication.KeyWindow;
                        var vc = window.RootViewController;
                        while (vc.PresentedViewController != null)
                        {
                            vc = vc.PresentedViewController;
                        }
                        eventController.NavigationBar.BarTintColor = color.Value.ToUIColor();
                        eventController.NavigationBar.TintColor = UIColor.White;
                        eventController.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };

                        vc.PresentViewController(eventController, true, null);
                    });

                    return true;
                }
                new UIAlertView("Access Denied", "User Denied Access to Calendar Data", null, "ok", null).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                new UIAlertView("Error", e.Message, null, "ok", null).Show();
                return false;
            }

            return false;
        }

        public DateTime NsDateToDateTime(NSDate date)
        {
            // NSDate has a wider range than DateTime, so clip
            // the converted date to DateTime.Min|MaxValue.
            double secs = date.SecondsSinceReferenceDate;
            if (secs < -63113904000)
                return DateTime.MinValue;
            if (secs > 252423993599)
                return DateTime.MaxValue;
            return (DateTime)date;
        }

        public NSDate DateTimeToNsDate(DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);// or DateTimeKind.Utc, this depends on each app */)
            return (NSDate)date;
        }
    }
}
using Android.Content;
using Android.Provider;
using RX.Api.Client;
using EquilibriumApp.Mobile.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Activity = Android.App.Activity;
using TimeZone = Java.Util.TimeZone;
using EquilibriumApp.Mobile.Interfaces;
using EquilibriumApp.Droid.NativeImplementation;

[assembly: Dependency(typeof(CalendarImplementation))]

namespace EquilibriumApp.Droid.NativeImplementation
{
    public class CalendarImplementation : ICalendar
    {
        private long GetDateTimeMs(DateTime date)
        {
            var c = Java.Util.Calendar.GetInstance(TimeZone.Default);
            
            c.Set(Java.Util.CalendarField.DayOfMonth, date.Day);
            c.Set(Java.Util.CalendarField.HourOfDay, date.Hour);
            c.Set(Java.Util.CalendarField.Minute, date.Minute);
            c.Set(Java.Util.CalendarField.Month, date.Month -1);
            c.Set(Java.Util.CalendarField.Year, date.Year);

            return c.TimeInMillis;
        }

        public async Task<bool> AddClassToCalender(ClassEventModel _class, double reminder = 30, Color? color = null)
        {
            var intent = new Intent(Intent.ActionInsert);

            intent.PutExtra(CalendarContract.Events.InterfaceConsts.CalendarId, _class.CalendarEventId);
            intent.PutExtra(CalendarContract.Events.InterfaceConsts.Title, _class.ClassName);
            intent.PutExtra(CalendarContract.Events.InterfaceConsts.Description, _class.ClassDescription.ScrubHtml());
            intent.PutExtra(CalendarContract.Events.InterfaceConsts.Id, _class.CalendarEventId);
            intent.PutExtra(CalendarContract.Events.InterfaceConsts.EventLocation, _class?.FacilityName); //TODO
            

            intent.PutExtra(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMs(_class.Start.DateTime));
            intent.PutExtra(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMs(_class.End.DateTime));
            intent.PutExtra(CalendarContract.ExtraEventBeginTime, GetDateTimeMs((_class.Start.DateTime)));
            intent.PutExtra(CalendarContract.ExtraEventEndTime, GetDateTimeMs((_class.End.DateTime)));
            if (reminder> 0)
            {
                intent.PutExtra(CalendarContract.Reminders.InterfaceConsts.Minutes, reminder);
                intent.PutExtra(CalendarContract.Reminders.InterfaceConsts.CalendarId, _class.CalendarEventId);
                intent.PutExtra(CalendarContract.Reminders.InterfaceConsts.EventId, _class.CalendarEventId);

            }

            
            intent.SetData(CalendarContract.Events.ContentUri);
            ((Activity)Forms.Context).StartActivity(intent);
            
            return true;
        }

    }
}
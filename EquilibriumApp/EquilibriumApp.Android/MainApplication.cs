using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;

namespace EquilibriumApp.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            SetupPush();

        }

        public void SetupPush()
        {
            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = PackageName;

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = AppSettings.PushChannelName;
            }

            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif

            FirebasePushNotificationManager.IconResource = Resource.Drawable.pushlogo;

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                var message = string.Empty;
                if (p.Data.ContainsKey("body"))
                {
                    message = p.Data["body"]?.ToString();
                }
                if (p.Data.ContainsKey("message"))
                {
                    message = p.Data["message"]?.ToString();
                }
                if (string.IsNullOrEmpty(message))
                {
                    return;
                }
                var title = p.Data["title"]?.ToString();
               
                using (var notificationManager = NotificationManager.FromContext(BaseContext))
                {
                    Notification notification;
                    if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
                    {
                        notification = new Notification.Builder(BaseContext)
                                                             .SetContentTitle(title)
                                                             .SetContentText(message)
                                                             .SetAutoCancel(true)
                                                             .SetPriority(1)
                                                             .SetSmallIcon(Resource.Drawable.pushlogo)
                                                             .SetDefaults(NotificationDefaults.All)
                                                             .Build();
                    }
                    else
                    {
                        const string channelName = AppSettings.PushChannelName;

                        NotificationChannel channel;
                        channel = notificationManager.GetNotificationChannel(PackageName);
                        if (channel == null)
                        {
                            channel = new NotificationChannel(PackageName, channelName, NotificationImportance.High);
                            channel.EnableVibration(true);
                            channel.EnableLights(true);
                            channel.LockscreenVisibility = NotificationVisibility.Public;
                            notificationManager.CreateNotificationChannel(channel);
                        }
                        channel?.Dispose();

                        notification = new NotificationCompat.Builder(Context, PackageName)
                                                             .SetChannelId(PackageName)
                                                             .SetContentTitle(title)
                                                             .SetContentText(message)
                                                             .SetAutoCancel(true)
                                                             .SetSmallIcon(Resource.Drawable.pushlogo)
                                                             .Build();
                    }
                    notificationManager.Notify(1331, notification);
                    notification.Dispose();
                }
            };
        }
    }
}
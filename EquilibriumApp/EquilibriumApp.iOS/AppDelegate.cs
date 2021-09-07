using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using System.Linq;
using CachedImageRenderer = FFImageLoading.Forms.Platform.CachedImageRenderer;
using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;
using Xamarin;
using UserNotifications;
using FFImageLoading;
using Plugin.FirebasePushNotification;

namespace EquilibriumApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppCenter.Start("997abdef-5e15-4a29-aa48-9d8e81db3486",
                   typeof(Analytics), typeof(Crashes));
            try
            {
                global::Xamarin.Forms.Forms.Init();
                CachedImageRenderer.Init();
                ImageCircleRenderer.Init();
                FormsMaps.Init();

                Rg.Plugins.Popup.Popup.Init();
                FirebasePushNotificationManager.Initialize(options, true);
                LoadApplication(new App());
                //PermissionCheck();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
            return base.FinishedLaunching(app, options);
        }

        public void PermissionCheck(bool unregister = false)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                    });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            // base.ReceiveMemoryWarning(application);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Xamarin.Essentials.Platform.OpenUrl(app, url, options))
                return true;

            return base.OpenUrl(app, url, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {

            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            ProcessNotification(userInfo, true);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }
        private void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            NSDictionary aps = options.ObjectForKey(new NSString("notification")) as NSDictionary;

            string title = string.Empty;
            if (aps.ContainsKey(new NSString("title")))
                title = (aps[new NSString("title")] as NSString).ToString();

            string body = string.Empty;
            if (aps.ContainsKey(new NSString("body")))
                body = (aps[new NSString("body")] as NSString).ToString();

            //Manually show an alert
            if (!string.IsNullOrEmpty(body))
            {
                UIAlertView avAlert = new UIAlertView(title, body, null, "OK", null);
                avAlert.Show();
            }

        }
    }
}

using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.AppModels;
using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EquilibriumApp.Helpers
{
    public static class GeneralHelper
    {


        public static async Task<bool> RequestPermission(Permission permission)
        {
            try
            {
                var result = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission);
                if (result)
                {
                    return true;
                }
                var status = await CrossPermissions.Current.RequestPermissionAsync<CalendarPermission>();
                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                return false;
             }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

    }
}

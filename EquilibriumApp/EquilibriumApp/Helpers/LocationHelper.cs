using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EquilibriumApp.Helpers
{
    public static class LocationHelper
    {
        public static async Task<Location> GetLocationAsync()
        {
            try
            {

                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    return location;
                }
            
                return new Location();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                UserDialogs.Instance.Toast("GPS not Supported.", TimeSpan.FromSeconds(5));
            }
            catch (FeatureNotEnabledException fneEx)
            {
                UserDialogs.Instance.Toast("GPS not Enabled.", TimeSpan.FromSeconds(5));
            }
            catch (PermissionException pEx)
            {
                UserDialogs.Instance.Toast("GPS Permisson not granted.", TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast("GPS not Enabled.", TimeSpan.FromSeconds(5));
            }
            return new Location();
        }


        public static async Task<Placemark> GetGeocodingAsync(Location location)
        {
            if (location == null)
            {
                return null;
            }
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

            var placemark = placemarks?.FirstOrDefault();
            if (placemark == null)
            {
                return null;
            }
            return placemark;
        }
    }
}

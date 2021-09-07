using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquilibriumApp.Helpers
{
    public static class SettingsHelper
    {

        public static AuthResponse GetAuthTokenData
        {
            get
            {
                var settings = Settings.AuthenticationAccess;
                return string.IsNullOrEmpty(settings) ? new AuthResponse() : Settings.AuthenticationAccess.Deserialize<AuthResponse>();
            }
        }
    
    }
}

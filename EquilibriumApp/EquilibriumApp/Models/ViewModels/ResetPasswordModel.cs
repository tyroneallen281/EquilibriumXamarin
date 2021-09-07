using EquilibriumApp.Mobile.Extentions;
using System;

namespace RX.Mobile.Models.ViewModels
{
    public class ResetPasswordModel : BaseViewModel
    {
        public string OTP { get; set; }
        public string Password { get; set; }
    }
}
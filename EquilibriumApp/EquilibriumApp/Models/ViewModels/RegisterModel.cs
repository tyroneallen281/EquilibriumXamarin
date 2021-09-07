using EquilibriumApp.Mobile.Extentions;
using System;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class RegisterModel : BaseViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TenantId => "RXITME";
        public int? Gender { get; set; }
        public string ProfileUrl { get; set; }
        public string ProfileBase64 { get; set; }
        public bool IsImage => !string.IsNullOrEmpty(ProfileUrl);

        public DateTime? DateOfBirth { get; set; }
        public string ExternalAccountId { get; set; }
        public string ExternalAccount { get; set; }
        public string AccessToken { get; set; }
    }
}
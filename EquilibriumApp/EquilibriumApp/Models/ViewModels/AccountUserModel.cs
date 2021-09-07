using EquilibriumApp.Mobile.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class AccountUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Fullname => $"{FirstName} {LastName}";
        public string Initials => Fullname.ExtractInitialsFromName();
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public int? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string ProfileBase64 { get; set; }

        [JsonProperty("ProfileUrl")]
        public string ProfileUrl { get; set; }

        public bool IsImage => !string.IsNullOrEmpty(ProfileUrl);
        public string UserProperties { get; set; }
        public string UserName { get; set; }
    }
}
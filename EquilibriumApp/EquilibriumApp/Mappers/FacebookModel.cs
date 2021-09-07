using EquilibriumApp.Mobile.Models.ViewModels;
using System;

namespace EquilibriumApp.Mappers
{
    public static class Mappers
    {
        public static RegisterModel MapToRegister(this FacebookModel model)
        {
            return new RegisterModel()
            {
                ExternalAccountId = model.id,
                FirstName = model.first_name,
                LastName = model.last_name,
                Email = model.email,
                ProfileUrl = model?.picture?.data?.url ?? "https://lmsblob.blob.core.windows.net/profile-images/person-male.png",
                ExternalAccount = "Facebook",
                AccessToken = model.AccessToken,
            };
        }
    }
}
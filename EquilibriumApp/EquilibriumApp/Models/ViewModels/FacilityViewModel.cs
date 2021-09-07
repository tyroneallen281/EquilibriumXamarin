using EquilibriumApp.Mobile.Extentions;
using RX.Api.Client;
using System;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class FacilityViewModel : BaseViewModel
    {
        public FacilityViewModel(FacilityModel model)
        {
            Model = model;
        }
        public FacilityModel Model { get; set; }

        public bool HasWebsite
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Model.Website);
            }
        }

        public bool HasContact
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Model.Contact);
            }
        }
    }
}
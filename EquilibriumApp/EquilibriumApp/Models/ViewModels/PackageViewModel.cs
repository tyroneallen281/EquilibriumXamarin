using EquilibriumApp.Mobile.Extentions;
using RX.Api.Client;
using System;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class PackageViewModel : BaseViewModel
    {
        public PackageViewModel(PackageModel model)
        {
            Model = model;
        }
        public PackageModel Model { get; set; }

       
    }
}
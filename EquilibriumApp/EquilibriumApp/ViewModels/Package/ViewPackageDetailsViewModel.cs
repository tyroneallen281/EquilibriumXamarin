using Acr.UserDialogs;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Services;
using RX.Api.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Packages
{
    public class ViewPackageDetailsViewModel : BaseViewModel
    {
        private double _scroll;
        private bool _eventDetails;
        private readonly IFacilityClient _facilityClient;
        private readonly IPackageClient _packageClient;
        private readonly IMemberPackageClient _memberPackageClient;

        public ViewPackageDetailsViewModel(IPackageClient packageClient, IFacilityClient facilityClient, IMemberPackageClient memberPackageClient)
        {
            _facilityClient = facilityClient;
            _packageClient = packageClient;
            _memberPackageClient = memberPackageClient;

        }
       

       
        private PackageViewModel package;

        public PackageViewModel Package
        {
            get { return package; }
            set { SetProperty(ref package, value); }
        }


        public override Task InitializeAsync(object navigationData)
        {
            PageTitle = "Package Details";
            Task.Run(async () =>
            {
                IsBusy = true;
                try
                {
                    Package = new PackageViewModel((PackageModel)navigationData);
                }
                catch (System.Exception)
                {
                    UserDialogs.Instance.Toast("An Error Occured.", TimeSpan.FromSeconds(5));
                }
                IsBusy = false;
            });
         
            return base.InitializeAsync(navigationData);
        }

        private ICommand _callCommand;

        public ICommand CallCommand
        {
            get
            {
                return _callCommand ?? (_callCommand = new Command(async () =>
                {

                    await Launcher.OpenAsync(new Uri($"tel:{Package.Model.FacilityContactNumber}"));
                }));
            }
        }


    }
}
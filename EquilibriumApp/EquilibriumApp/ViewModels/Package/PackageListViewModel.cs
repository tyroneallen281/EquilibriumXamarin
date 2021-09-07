using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Packages;
using EquilibriumApp.Services.Navigation;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Events
{
    public class PackageListViewModel : ViewModelBase
    {
        private readonly IPackageClient _packageClient;
        private readonly IFacilityClient _facilityClient;

        public PackageListViewModel(IPackageClient packageClient, IFacilityClient facilityClient)
        {
            _packageClient = packageClient;
            _facilityClient = facilityClient;
            Packages = new ObservableCollection<PackageModel>();
        }

        public override Task InitializeAsync(object navigationData)
        {
            Facility = (FacilityModel)navigationData;
            PageTitle = $"{Facility.Name} - Packages";

            RefreshCommand.Execute(null);

            return base.InitializeAsync(navigationData);
        }

     
        private FacilityModel _facility;
        public FacilityModel Facility
        {
            get { return _facility; }
            set { SetProperty(ref _facility, value); }
        }


        private bool _showNoPackages;

        public bool ShowNoPackages
        {
            get { return _showNoPackages; }
            set { SetProperty(ref _showNoPackages, value); }
        }

        private ObservableCollection<PackageModel> _packages;

        public ObservableCollection<PackageModel> Packages
        {
            get { return _packages; }
            set { SetProperty(ref _packages, value); }
        }

        private string _seachQuery;

        public string SearchQuery
        {
            get { return _seachQuery; }
            set {
                SetProperty(ref _seachQuery, value);
                RefreshCommand.Execute(null);
            }
        }



        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                    LoadPackagesCommand.Execute(null);
                
                }));
            }
        }

        private ICommand _loadPackagesCommand;

        public ICommand LoadPackagesCommand
        {
            get
            {
                return _loadPackagesCommand ?? (_loadPackagesCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        ShowNoPackages = false;

                        var packagesResult = await _packageClient.GetAllAsync(null, Facility.FacilityId);
                        Packages = new ObservableCollection<PackageModel>(packagesResult.OrderBy(_ => _.Name));
                        ShowNoPackages = !Packages.Any();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        UserDialogs.Instance.Toast("Error loading packages please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }

        private ICommand _packageItemSelectedCommand;

        public ICommand PackageItemSelectedCommand
        {
            get
            {
                return _packageItemSelectedCommand ?? (_packageItemSelectedCommand = new Command(async (selectedItem) =>
                {
                    await _navigationService.NavigateToAsync<ViewPackageDetailsViewModel>(selectedItem);
                }));
            }
        }
    }
}
using Acr.UserDialogs;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Favourite
{
    public class FavoritesViewModel : BaseViewModel
    {
        private readonly IFavouriteClient _favouriteClient;
        private readonly IClassClient _classClient;

        public FavoritesViewModel(IFavouriteClient favouriteClient, IClassClient classClient)
        {
            _favouriteClient = favouriteClient;
            _classClient = classClient;
            SetClassesView();
            RefreshCommand.Execute(null);
            MessagingCenter.Subscribe<Messages>(this, "update_favourites", model =>
            {
                RefreshCommand.Execute(null);
            });
        }
       
        public void SetClassesView()
        {
            ShowClasses = true;
            ShowFacility = false;
        }
        public void SetFacilityView()
        {
            ShowClasses = false;
            ShowFacility = true;
        }

        private bool _loggedIn;

        public bool LoggedIn
        {
            get => _loggedIn;
            set => SetProperty(ref _loggedIn, value);
        }

        private bool _showClasses;
        public bool ShowClasses
        {
            get => _showClasses;
            set => SetProperty(ref _showClasses, value);
        }
        private bool _showFacility;

        public bool ShowFacility
        {
            get => _showFacility;
            set => SetProperty(ref _showFacility, value);
        }
        public ICommand SetClassesViewCommand => new Command(SetClassesView);
        public ICommand SetFacilityViewCommand => new Command(SetFacilityView);


        private ObservableCollection<UserFavouriteModel> _userFavouriteClasses;

        public ObservableCollection<UserFavouriteModel> UserFavouriteClasses
        {
            get { return _userFavouriteClasses; }
            set { SetProperty(ref _userFavouriteClasses, value); }
        }

        private ObservableCollection<UserFavouriteModel> _userFavouriteFacility;

        public ObservableCollection<UserFavouriteModel> UserFavouriteFacility
        {
            get { return _userFavouriteFacility; }
            set { SetProperty(ref _userFavouriteFacility, value); }
        }

        private bool _showNoFacilities;
        public bool ShowNoFacilities
        {
            get { return _showNoFacilities; }
            set { SetProperty(ref _showNoFacilities, value); }
        }

        private bool _showNoClasses;

        public bool ShowNoClasses
        {
            get { return _showNoClasses; }
            set { SetProperty(ref _showNoClasses, value); }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () =>
                {
                    LoggedIn = AuthenticationService.ActiveAccount();
                    if (AuthenticationService.ActiveAccount())
                    {
                        LoadEventsCommand.Execute(null);
                        LoadFacilitiesCommand.Execute(null);
                    }
                }));
            }
        }

        private ICommand _loadEventsCommand;

        public ICommand LoadEventsCommand
        {
            get
            {
                return _loadEventsCommand ?? (_loadEventsCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;

                        var classesResult = await _favouriteClient.GetFavouritesClassAsync(null);
                        UserFavouriteClasses = new ObservableCollection<UserFavouriteModel>(classesResult);
                        ShowNoClasses = !UserFavouriteClasses.Any();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        //UserDialogs.Instance.Toast("Error loading classes please pull to try again.", TimeSpan.FromSeconds(5));
                    }
                    IsBusy = false;
                }));
            }
        }

        private ICommand _loadFacilitiesCommand;

        public ICommand LoadFacilitiesCommand
        {
            get
            {
                return _loadFacilitiesCommand ?? (_loadFacilitiesCommand = new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        var facilitiesResult = await _favouriteClient.GetFavouritesFacilityAsync(null);
                        UserFavouriteFacility = new ObservableCollection<UserFavouriteModel>(facilitiesResult);
                        ShowNoFacilities = !UserFavouriteFacility.Any();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                    }
                    IsBusy = false;


                }));
            }
        }

        private ICommand _classItemSelectedCommand;

        public ICommand ClassItemSelectedCommand
        {
            get
            {
                return _classItemSelectedCommand ?? (_classItemSelectedCommand = new Command(async (selectedItem) =>
                {
                    try
                    {
                        var @classFav = selectedItem as UserFavouriteModel;
                        var @class = await _classClient.Get2Async(@classFav.ClassId.Value);
                        await _navigationService.NavigateToPopupAsync<ClassScheduleViewModel>(@class, true);
                    }
                    catch (Exception)
                    {
                    }
                 
                }));
            }
        }

        private ICommand _facilityItemSelectedCommand;

        public ICommand FacilityItemSelectedCommand
        {
            get
            {
                return _facilityItemSelectedCommand ?? (_facilityItemSelectedCommand = new Command(async (selectedItem) =>
                {
                    var faciltiy = selectedItem as UserFavouriteModel;
                    await _navigationService.NavigateToPopupAsync<ViewFacilityDetailsViewModel>(faciltiy.FacilityId, true);
                }));
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new Command(async (selectedItem) =>
                {
                    await _navigationService.NavigateToPopupAsync<LoginRegisterViewModel>(true);
                }));
            }
        }
    }
}
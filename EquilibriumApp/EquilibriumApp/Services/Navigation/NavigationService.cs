using EquilibriumApp.Controls.XamlControls;
using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.DI;
using EquilibriumApp.Mobile.Authentication;
using EquilibriumApp.Mobile.Controls.XamlControls;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Mobile.ViewModels.Favourite;
using EquilibriumApp.Mobile.ViewModels.Packages;
using EquilibriumApp.Mobile.ViewModels.Payment;
using EquilibriumApp.Mobile.ViewModels.User;
using EquilibriumApp.Mobile.Views.Class;
using EquilibriumApp.Mobile.Views.Facility;
using EquilibriumApp.Mobile.Views.Favourite;
using EquilibriumApp.Mobile.Views.Package;
using EquilibriumApp.Mobile.Views.User;
using EquilibriumApp.ViewModels;
using EquilibriumApp.Views.Home;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.BehaviorsPack;

namespace EquilibriumApp.Services.Navigation
{
    public partial class NavigationService : INavigationService
    {
        private INavigation _navigation;
        protected readonly Dictionary<Type, Type> Mappings;

        protected Application CurrentApplication => Application.Current;

        public NavigationService()
        { 
            Mappings = new Dictionary<Type, Type>();

            CreatePageViewModelMappings();
        }

        public async Task InitializeAsync()
        {
            await NavigateToAsync<MainViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        public async Task NavigateBackAsync()
        {
            if (CurrentApplication.MainPage is MainView)
            {
                var mainPage = CurrentApplication.MainPage as MainView;
                await mainPage.Navigation.PopAsync();
            }
            else if (CurrentApplication.MainPage != null)
            {
                await CurrentApplication.MainPage.Navigation.PopAsync();
            }
        }

       
        public async Task NavigationModalBackAsync()
        {
            if (CurrentApplication.MainPage is MainPage)
            {
                if (CurrentApplication.MainPage is MainView mainPage) await mainPage.Navigation.PopModalAsync();
            }
            else if (CurrentApplication.MainPage != null)
            {
                await CurrentApplication.MainPage.Navigation.PopModalAsync();
            }
        }


        public virtual Task RemoveLastFromBackStackAsync()
        {
            if (CurrentApplication.MainPage is MainView mainPage)
            {
                mainPage.Navigation.RemovePage(
                    mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is MainView)
            {
                CurrentApplication.MainPage = new CustomNavigationPage(page);
            }
            else
            {
                if (CurrentApplication.MainPage.Navigation.ModalStack.Any())
                {
                    await CurrentApplication.MainPage.Navigation.ModalStack.Last().Navigation.PushAsync(page);
                }
                else
                {
                    await CurrentApplication.MainPage.Navigation.PushAsync(page);
                }
            }
           
            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }


        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!Mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }

            return Mappings[viewModelType];
        }

        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            try
            {
                Type pageType = GetPageTypeForViewModel(viewModelType);

                if (pageType == null)
                {
                    throw new Exception($"Mapping type for {viewModelType} is not a page");
                }

                Page page = Activator.CreateInstance(pageType) as Page;
                ViewModelBase viewModel = (ViewModelBase) Locator.Instance.Resolve(viewModelType);
                page.BindingContext = viewModel;

                return page;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private void CreatePageViewModelMappings()
        {
            Mappings.Add(typeof(MainViewModel), typeof(MainView));
            Mappings.Add(typeof(ExtendedSplashViewModel), typeof(ExtendedSplashView));
            Mappings.Add(typeof(SearchClassesViewModel), typeof(SearchClassPage));
            Mappings.Add(typeof(FavoritesViewModel), typeof(FavoritesPage));
            Mappings.Add(typeof(PayViewModel), typeof(PayControl));
            Mappings.Add(typeof(MeViewModel), typeof(MePage)); 
            Mappings.Add(typeof(ViewClassDetailsViewModel), typeof(ViewClassDetailsPage));
            Mappings.Add(typeof(ViewPackageDetailsViewModel), typeof(ViewPackageDetailsPage));
            Mappings.Add(typeof(ViewFacilityDetailsViewModel), typeof(ViewFacilityDetailsPage));
            Mappings.Add(typeof(LoginRegisterViewModel), typeof(LoginRegisterPage));
            Mappings.Add(typeof(ClassScheduleViewModel), typeof(ClassSchedulePage));
            Mappings.Add(typeof(PackageListViewModel), typeof(PackageListPage));
            Mappings.Add(typeof(BookingResultViewModel), typeof(BookingResultPage));
            Mappings.Add(typeof(AddAchievementViewModel), typeof(AddAchievementPage));
            Mappings.Add(typeof(ProfileViewModel), typeof(ProfilePage));
            Mappings.Add(typeof(ResetPasswordViewModel), typeof(ResetPasswordPage));
            Mappings.Add(typeof(OauthRegisterViewModel), typeof(OauthRegisterPage));
        }
    }
}
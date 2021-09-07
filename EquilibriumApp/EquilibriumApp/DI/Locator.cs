using System;
using System.Net.Http;
using Autofac;
using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.ViewModels.Authentication;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Mobile.ViewModels.Favourite;
using EquilibriumApp.Mobile.ViewModels.Packages;
using EquilibriumApp.Mobile.ViewModels.Payment;
using EquilibriumApp.Mobile.ViewModels.User;
using EquilibriumApp.Services.Navigation;
using EquilibriumApp.ViewModels;
using RX.Api.Client;

namespace EquilibriumApp.DI
{
    public class Locator
    {
        private IContainer _container;
        private ContainerBuilder _containerBuilder;
        
        private static readonly Locator _instance = new Locator();

        public static Locator Instance
        {
            get
            {
                return _instance;
            }
        }
        public static HttpClient Client
        {
            get
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                var token = SettingsHelper.GetAuthTokenData?.AccessToken;
                if (token != null)
                {
                    ClientBaseFactory.SetBearerToken(token);
                }
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            }
        }

        public Locator()
        {
           
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<NavigationService>().As<INavigationService>();
            _containerBuilder.RegisterType<CalendarClient>().As<ICalendarClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<FacilityClient>().As<IFacilityClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<PackageClient>().As<IPackageClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<ClassBookingClient>().As<IClassBookingClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<UserAchievementClient>().As<IUserAchievementClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<MemberPackageClient>().As<IMemberPackageClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<MemberPackagePeriodClient>().As<IMemberPackagePeriodClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<UserClient>().As<IUserClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<FileClient>().As<IFileClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<FavouriteClient>().As<IFavouriteClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);
            _containerBuilder.RegisterType<ClassClient>().As<IClassClient>().WithParameter("baseUrl", AppSettings.ApiUrl).WithParameter("httpClient", Client);

            //ViewModels
            _containerBuilder.RegisterType<MainViewModel>();
            _containerBuilder.RegisterType<ExtendedSplashViewModel>();
            _containerBuilder.RegisterType<SearchClassesViewModel>();
            _containerBuilder.RegisterType<PayViewModel>();
            _containerBuilder.RegisterType<FavoritesViewModel>();
            _containerBuilder.RegisterType<MeViewModel>(); 
            _containerBuilder.RegisterType<ViewFacilityDetailsViewModel>();
            _containerBuilder.RegisterType<ViewPackageDetailsViewModel>();
            _containerBuilder.RegisterType<ViewClassDetailsViewModel>();
            _containerBuilder.RegisterType<LoginRegisterViewModel>();
            _containerBuilder.RegisterType<ClassScheduleViewModel>();
            _containerBuilder.RegisterType<PackageListViewModel>();
            _containerBuilder.RegisterType<BookingResultViewModel>();
            _containerBuilder.RegisterType<AddAchievementViewModel>();
            _containerBuilder.RegisterType<ProfileViewModel>();
            _containerBuilder.RegisterType<OauthRegisterViewModel>();
            _containerBuilder.RegisterType<ResetPasswordViewModel>();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _containerBuilder.RegisterType<TImplementation>().As<TInterface>();
        }

        public void Register<T>() where T : class
        {
            _containerBuilder.RegisterType<T>();
        }

        public void Build()
        {
            try
            {
                _container = _containerBuilder.Build();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
           
        }
    }
}
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Services.Navigation;
using System.Threading.Tasks;


namespace EquilibriumApp.ViewModels
{
    public class ExtendedSplashViewModel : ViewModelBase
    {
        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            await _navigationService.InitializeAsync();

            IsBusy = false;
        }
    }
}

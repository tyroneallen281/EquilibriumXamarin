using System;
using System.Threading.Tasks;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Views.Home;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace EquilibriumApp.Services.Navigation
{
    public partial class NavigationService : INavigationService
    {

        public Task NavigateToPopupAsync<TViewModel>(bool animate) where TViewModel : ViewModelBase
        {
            return NavigateToPopupAsync<TViewModel>(null, animate);
        }

        public async Task NavigateToPopupAsync<TViewModel>(object parameter, bool animate) where TViewModel : ViewModelBase
        {
            var page = CreateAndBindPage(typeof(TViewModel), parameter);
            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);

            
            if(page is PopupPage)
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page as PopupPage, animate);
            }
            else if (page is ContentPage)
            {
                page = new CustomNavigationPage(page);
                await CurrentApplication.MainPage.Navigation.PushModalAsync(page, animate);
            }
            else
            {
                throw new ArgumentException($"The type ${typeof(TViewModel)} its not a PopupPage type");
            }
        }

        public async Task NavigateBackModalAsync<TViewModel>( bool animate) where TViewModel : ViewModelBase
        {
            var page = CreateAndBindPage(typeof(TViewModel), null);
            if (page is PopupPage)
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync(animate);
            }
            else
            {
                await CurrentApplication.MainPage.Navigation.PopModalAsync(animate);
            }
        }

    }
}
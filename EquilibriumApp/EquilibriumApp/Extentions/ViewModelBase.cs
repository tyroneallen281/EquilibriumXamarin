using EquilibriumApp.DI;
using EquilibriumApp.Services.Navigation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Extentions
{
    public abstract class ViewModelBase : BindableObject
    {
        private bool _isDeviceConnected;
        private string _pageTitle;
        private bool _isBusy;
        private string _loadingText = "Loading...";
        protected readonly INavigationService _navigationService;

        public ViewModelBase()
        {
            _navigationService = Locator.Instance.Resolve<INavigationService>();
        }


        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }
        public string LoadingText
        {
            get
            {
                return _loadingText;
            }

            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        protected virtual bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
                return false;

            property = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}

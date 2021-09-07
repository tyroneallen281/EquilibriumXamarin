using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Views.CSPages
{
    public class BaseContentPage : MainBaseContentPage
    {
        public BaseContentPage()
        {
        }
    }

    public class MainBaseContentPage : ContentPage
    {
        private bool _hasSubscribed;

        public Color BarTextColor
        {
            get;
            set;
        }

        public Color BarBackgroundColor
        {
            get;
            set;
        }

        public MainBaseContentPage()
        {
            BarTextColor = Color.White;
            BarBackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];

            _hasSubscribed = true;
        }

        ~MainBaseContentPage()
        {
            //Debug.WriteLine("Destructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));
        }

        public bool HasInitialized
        {
            get;
            private set;
        }

        protected virtual void Initialize()
        {
        }

        protected override void OnAppearing()
        {
            if (!_hasSubscribed)
            {
                _hasSubscribed = true;
            }

            if (!HasInitialized)
            {
                HasInitialized = true;
                //OnLoaded();
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _hasSubscribed = false;

            base.OnDisappearing();
            //EvaluateNavigationStack();
        }

        private async Task EvaluateNavigationStack()
        {
            await Task.Delay(100);
            if (Navigation.NavigationStack.Count == 0)
            {
                //UnsubscribeFromMessages();
            }
        }

        /// <summary>
        /// Wraps the ContentPage within a NavigationPage
        /// </summary>
        /// <returns>The navigation page.</returns>
        public Page WithinNavigationPage()
        {
            return this;
        }
    }
}
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Controls.XamlControls
{
    public partial class LoadingControl : ContentView
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            //if (propertyName == TextProperty.PropertyName)
            //{
            //    this.Label.Text = Text;
            //}
            if (propertyName == IsVisibleProperty.PropertyName)
            {
                IsVisible = Visibility;
                // this.HeightRequest = (this.Parent as Grid).HeightRequest;
            }
            if (propertyName == VisibilityProperty.PropertyName)
            {
                IsVisible = Visibility;
                //this.HeightRequest = (this.Parent as Grid).HeightRequest;
            }

        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(LoadingControl), string.Empty);

        public static readonly BindableProperty VisibilityProperty =
           BindableProperty.Create(nameof(Visibility), typeof(bool), typeof(LoadingControl), false);


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool Visibility
        {
            get { return (bool)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
    }
}

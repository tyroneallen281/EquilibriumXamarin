using EquilibriumApp.DI;
using EquilibriumApp.Mobile.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MePage : ContentPage
    {
        public MePage()
        {
            InitializeComponent();
            this.BindingContext = Locator.Instance.Resolve(typeof(MeViewModel));
        }

    }
}
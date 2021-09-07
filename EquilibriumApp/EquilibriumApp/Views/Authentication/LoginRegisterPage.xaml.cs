using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginRegisterPage : ContentPage
    {
        public LoginRegisterPage()
        {
            InitializeComponent();
            if (Navigation.NavigationStack.Count() < 2)
            {
                this.ToolbarItems.Add(new ToolbarItem
                {
                    IconImageSource = "close.png",
                    Command = new Command(async () =>
                    {
                        await this.Navigation.PopModalAsync();
                    })
                });
            }
        }

        protected override void OnAppearing()
        {
          
        }
    }
}
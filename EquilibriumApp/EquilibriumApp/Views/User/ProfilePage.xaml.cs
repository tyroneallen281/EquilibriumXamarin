using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
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
    }
}
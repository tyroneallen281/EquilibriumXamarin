using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Mobile.ViewModels.Events;
using EquilibriumApp.Mobile.Views.CSPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RX.Api.Client;
using EquilibriumApp.DI;

namespace EquilibriumApp.Mobile.Views.Class
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassSchedulePage : ContentPage
    {
        public ClassSchedulePage()
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
            try
            {
                base.OnAppearing();
                
            }
            catch (Exception e)
            {
            }
        }

        private void ItemsView_OnSelectedItemChanged(object sender, EventArgs e)
        {
            var context = BindingContext as ClassScheduleViewModel;
            foreach (DateModel dateModel in ItemsView.ItemsSource)
            {
                dateModel.ShowColor = false;
            }
            var model = ItemsView.SelectedItem as DateModel;
            if (model != null)
                model.ShowColor = true;
            if (context != null)
            {
                context.LoadEventsCommand.Execute(null);
            }
          
        }

  
    }
}
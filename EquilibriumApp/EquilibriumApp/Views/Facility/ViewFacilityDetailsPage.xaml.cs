using EquilibriumApp.Mobile.ViewModels.Events;
using FFImageLoading.Forms;
using RX.Api.Client;
using System;
using System.Linq;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Views.Facility
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewFacilityDetailsPage : ContentPage
    {
        public ViewFacilityDetailsPage()
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
			MessagingCenter.Subscribe<ViewFacilityDetailsViewModel>(this, "MoveToRegion", model =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					FacilityMap.Pins.Clear();
					var pin = new Pin()
					{
						Address = model.Facility.Model.Address,
						Type = PinType.Place,
						Label = model.Facility.Model.Name,
						Position = new Position(model.Facility.Model.Latitude, model.Facility.Model.Longitude)
					};
					FacilityMap.Pins.Add(pin);
					var zoomLevel = 12; // between 1 and 18
					var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
					MapSpan mapSpan = MapSpan.FromCenterAndRadius(pin.Position, Distance.FromKilometers(1));
					FacilityMap.MoveToRegion(mapSpan);

				});

			});
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			
		}
	}
}
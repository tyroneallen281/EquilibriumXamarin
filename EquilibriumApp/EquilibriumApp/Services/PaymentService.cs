using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.AppModels;
using EquilibriumApp.Mobile.ViewModels.Payment;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services.Navigation;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquilibriumApp.Services
{
    public static class PaymentService
    {

        public static async Task<bool> ProcessPaymentBooking(INavigationService _navigation, ClassBookingModel booking )
        {
            

            if (booking.PaymentHistory.Amount == 0)
            {
                MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                await _navigation.NavigateToPopupAsync<BookingResultViewModel>(booking, true);
                return true;
            }
            else
            {
                var paymentViewModel = new PaymentViewModel()
                {
                    PaymentUrl = booking.PaymentUrl,
                    PaymentObject = booking
                };
                await _navigation.NavigateToAsync<PayViewModel>(paymentViewModel);
               
                return true;
            }
        }

    }
}

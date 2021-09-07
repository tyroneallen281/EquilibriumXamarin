using Acr.UserDialogs;
using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Models.ViewModels;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.Payment
{
    public class PayViewModel : BaseViewModel
    {
        private PaymentViewModel paymentViewModel;

        private UrlWebViewSource _startUrl;

        public UrlWebViewSource StartUrl
        {
            get => _startUrl;
            set => SetProperty(ref _startUrl, value);
        }
        
        public PayViewModel()
        {
           
            
        }

        public override Task InitializeAsync(object navigationData)
        {
            PageTitle = "Payment";
            paymentViewModel = navigationData as PaymentViewModel;
            StartUrl = new UrlWebViewSource() { Url = paymentViewModel.PaymentUrl ?? "TODO" };
            return base.InitializeAsync(navigationData);
        }

        private ICommand _paymentResultCommand;

        public ICommand PaymentResultCommand
        {
            get
            {
                return _paymentResultCommand ?? (_paymentResultCommand = new Command(async () =>
                {
                    await _navigationService.NavigateToPopupAsync<BookingResultViewModel>(paymentViewModel, true);
                }));
            }
        }
    }
}
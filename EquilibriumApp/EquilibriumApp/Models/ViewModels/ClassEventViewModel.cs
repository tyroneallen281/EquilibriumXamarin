using EquilibriumApp.Mobile.Extentions;
using RX.Api.Client;
using System;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class ClassEventViewModel : BaseViewModel
    {
        public ClassEventViewModel(ClassEventModel model)
        {
            Model = model;
        }

        private ClassEventModel @class;

        public ClassEventModel Model
        {
            get { return @class; }
            set { SetProperty(ref @class, value); }
        }


        public string BookingString
        {
            get
            {
                if (HasPackage)
                {
                    return "Book - With Package";
                }

                if (ShowCallFacility)
                {
                    return "Book - Call Facility";
                }

                return $"Book - {Model.ClassRateString}";
            }
        }

        public bool HasPackage
        {
            get
            {
                return Model.MemberPackagePeriodId != null;
            }
        }

        public bool ShowBookingButton
        {
            get
            {
                return Model.BookingOpen && !Model.UserBookedClass;
            }
        }

        public bool ShowBookingClosedButton
        {
            get
            {
                return !Model.BookingOpen && !Model.UserBookedClass;
            }
        }

        public bool ShowCancelButton
        {
            get
            {
                return Model.UserBookedClass;
            }
        }

        public bool ShowCallFacility
        {
            get
            {
                return !Model.PaymentEnabled;
            }
        }
    }
}
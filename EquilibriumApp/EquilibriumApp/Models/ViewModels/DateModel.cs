using EquilibriumApp.Mobile.Extentions;
using System;

namespace EquilibriumApp.Mobile.Models.ViewModels
{
    public class DateModel : BaseViewModel
    {
        public DateModel(DateTime date, bool showColor = false)
        {
            Date = date;
            ShowColor = showColor;
        }
        private bool _showColor;

        public bool ShowColor
        {
            get { return _showColor; }
            set { SetProperty(ref _showColor, value); }
        }
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        public string DateString
        {
            get
            {
                return Date.ToString("dd-MM-yyyy");
            }
        }
    }
}
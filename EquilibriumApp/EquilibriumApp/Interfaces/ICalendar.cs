using System.Threading.Tasks;
using RX.Api.Client;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Interfaces
{
    public interface ICalendar
    {
        Task<bool> AddClassToCalender(ClassEventModel _class, double reminder = 30, Color? color = null);
    }
}
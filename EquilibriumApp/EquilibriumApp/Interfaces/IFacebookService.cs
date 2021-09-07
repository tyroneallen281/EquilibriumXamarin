using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Interfaces
{
    public interface IFacebookService
    {
        Task<bool> Login(INavigation navigation = null, bool isModal = false, bool isAttached = false);
    }
}
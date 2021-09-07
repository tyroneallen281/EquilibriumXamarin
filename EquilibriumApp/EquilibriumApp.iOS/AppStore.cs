using Contacts;
using EventKit;

namespace EquilibriumApp.Mobile.iOS
{
    public class AppStore
    {
        public static AppStore Current { get; }

        public EKEventStore EventStore => eventStore;
        public CNContactStore ContactStore => contactStore;
        protected EKEventStore eventStore;
        protected CNContactStore contactStore;
        static AppStore()
        {
            Current = new AppStore();
        }
        protected AppStore()
        {
            eventStore = new EKEventStore();
            contactStore = new CNContactStore();
        }
    }
}
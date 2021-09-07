using EventKitUI;

namespace EquilibriumApp.Mobile.iOS
{
   // [Register("AppDelegate")]
    public class CreateEventEditViewDelegate : EKEventEditViewDelegate
    {
        // we need to keep a reference to the controller so we can dismiss it
        protected EKEventEditViewController EventController;

        public CreateEventEditViewDelegate(EKEventEditViewController eventController)
        {
           
            // save our controller reference
            this.EventController = eventController;
        }

        // completed is called when a user eith
        public override void Completed(EKEventEditViewController controller, EKEventEditViewAction action)
        {
            EventController.DismissViewController(true, null);
        }
    }
}
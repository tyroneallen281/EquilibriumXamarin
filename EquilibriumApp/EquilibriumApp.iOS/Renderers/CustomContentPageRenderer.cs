using System.Collections.Generic;
using EquilibriumApp.Mobile.Controls.CsControls;
using EquilibriumApp.Mobile.iOS.Renderers;
using EquilibriumApp.Mobile.Views.CSPages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomContentPage), typeof(CustomContentPageRenderer))]
namespace EquilibriumApp.Mobile.iOS.Renderers
{
    public class CustomContentPageRenderer:PageRenderer
    {
        //https://timeyoutake.it/2016/01/02/creating-a-left-toolbaritem-in-xamarin-forms/
        public CustomContentPageRenderer()
        {
            
        }
        public new BaseContentPage Element => (BaseContentPage)base.Element;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var leftNavList = new List<UIBarButtonItem>();
            var rightNavList = new List<UIBarButtonItem>();

            var navigationItem = NavigationController.TopViewController.NavigationItem;

            //  this.NavigationItem.LeftBarButtonItem.TintColor = UIColor.White;
         

            for (var i = 0; i < Element.ToolbarItems.Count; i++)
            {
               
                var reorder = (Element.ToolbarItems.Count - 1);
                var itemPriority = Element.ToolbarItems[reorder - i].Priority;

                if (itemPriority == 1)
                {
                    var leftNavItems = navigationItem.RightBarButtonItems[i];
                    leftNavList.Add(leftNavItems);
                }
                else if (itemPriority == 0)
                {
                    var rightNavItems = navigationItem.RightBarButtonItems[i];
                    rightNavList.Add(rightNavItems);
                }
            }

            navigationItem.SetLeftBarButtonItems(leftNavList.ToArray(), false);
            navigationItem.SetRightBarButtonItems(rightNavList.ToArray(), false);

        }

        
    }
}
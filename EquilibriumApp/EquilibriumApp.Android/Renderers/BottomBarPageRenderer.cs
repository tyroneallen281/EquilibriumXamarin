using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using RX.Mobile.Controls.CsControls;
using RX.Mobile.Droid.Renderers;
using RX.Mobile.Droid.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(BottomBarPage), typeof(BottomBarPageRenderer))]

namespace RX.Mobile.Droid.Renderers
{
    public class BottomBarPageRenderer : VisualElementRenderer<BottomBarPage>, IOnTabClickListener
    {
        private bool _disposed;
        private BottomBar _bottomBar;
        private FrameLayout _frameLayout;
        private Utils.IPageController _pageController;
        private IDictionary<Page, BottomBarBadge> _badges;

        public BottomBarPageRenderer()
        {
            AutoPackage = false;
        }

        #region IOnTabClickListener

        public virtual void OnTabSelected(int position)
        {
            //Do we need this call? It's also done in OnElementPropertyChanged
            SwitchContent(Element.Children[position]);
            var bottomBarPage = Element as BottomBarPage;
            bottomBarPage.CurrentPage = Element.Children[position];
            //bottomBarPage.RaiseCurrentPageChanged();
        }

        public virtual void OnTabReSelected(int position)
        {
        }

        #endregion IOnTabClickListener

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                RemoveAllViews();

                foreach (Page pageToRemove in Element.Children)
                {
                    IVisualElementRenderer pageRenderer = Platform.GetRenderer(pageToRemove);

                    if (pageRenderer != null)
                    {
                        pageRenderer.ViewGroup.RemoveFromParent();
                        pageRenderer.Dispose();
                    }

                    pageToRemove.PropertyChanged -= OnPagePropertyChanged;
                    // pageToRemove.ClearValue (Platform.RendererProperty);
                }

                if (_badges != null)
                {
                    _badges.Clear();
                    _badges = null;
                }

                if (_bottomBar != null)
                {
                    _bottomBar.SetOnTabClickListener(null);
                    _bottomBar.Dispose();
                    _bottomBar = null;
                }

                if (_frameLayout != null)
                {
                    _frameLayout.Dispose();
                    _frameLayout = null;
                }

                /*if (Element != null) {
					PageController.InternalChildren.CollectionChanged -= OnChildrenCollectionChanged;
				}*/
            }

            base.Dispose(disposing);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _pageController.SendAppearing();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            _pageController.SendDisappearing();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BottomBarPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                BottomBarPage bottomBarPage = e.NewElement;

                if (_bottomBar == null)
                {
                    _pageController = PageController.Create(bottomBarPage);

                    // create a view which will act as container for Page's
                    _frameLayout = new FrameLayout(Forms.Context);
                    _frameLayout.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, GravityFlags.Fill);
                    AddView(_frameLayout, 0);

                    // create bottomBar control
                    _bottomBar = BottomBar.Attach(_frameLayout, null);
                    _bottomBar.NoTabletGoodness();
                    if (bottomBarPage.FixedMode)
                    {
                        _bottomBar.UseFixedMode();
                    }

                    switch (bottomBarPage.BarTheme)
                    {
                        case BottomBarPage.BarThemeTypes.Light:
                            break;

                        case BottomBarPage.BarThemeTypes.DarkWithAlpha:
                            _bottomBar.UseDarkThemeWithAlpha(true);
                            break;

                        case BottomBarPage.BarThemeTypes.DarkWithoutAlpha:
                            _bottomBar.UseDarkThemeWithAlpha(false);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _bottomBar.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    _bottomBar.SetOnTabClickListener(this);

                    UpdateTabs();
                    UpdateBarBackgroundColor();
                    UpdateBarTextColor();
                }

                if (bottomBarPage.CurrentPage != null)
                {
                    SwitchContent(bottomBarPage.CurrentPage);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                SwitchContent(Element.CurrentPage);
                UpdateSelectedTabIndex(Element.CurrentPage);
            }
            else if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
            {
                UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
            {
                UpdateBarTextColor();
            }
        }

        protected virtual void SwitchContent(Page view)
        {
            Context.HideKeyboard(this);

            _frameLayout.RemoveAllViews();

            if (view == null)
            {
                return;
            }

            if (Platform.GetRenderer(view) == null)
            {
                Platform.SetRenderer(view, Platform.CreateRenderer(view));
            }

            _frameLayout.AddView(Platform.GetRenderer(view).ViewGroup);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int width = r - l;
            int height = b - t;

            var context = Context;

            _bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.AtMost));
            int tabsHeight = Math.Min(height, Math.Max(_bottomBar.MeasuredHeight, _bottomBar.MinimumHeight));

            if (width > 0 && height > 0)
            {
                _pageController.ContainerArea = new Rectangle(0, 0, context.FromPixels(width), context.FromPixels(_frameLayout.MeasuredHeight));
                ObservableCollection<Element> internalChildren = _pageController.InternalChildren;

                foreach (var t1 in internalChildren)
                {
                    var child = t1 as VisualElement;

                    if (child == null)
                    {
                        continue;
                    }

                    IVisualElementRenderer renderer = Platform.GetRenderer(child);
                    var navigationRenderer = renderer as NavigationPageRenderer;
                    if (navigationRenderer != null)
                    {
                        // navigationRenderer.ContainerPadding = tabsHeight;
                    }
                }

                _bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));
                _bottomBar.Layout(0, 0, width, tabsHeight);
            }

            base.OnLayout(changed, l, t, r, b);
        }

        private void UpdateSelectedTabIndex(Page page)
        {
            var index = Element.Children.IndexOf(page);
            _bottomBar.SelectTabAtPosition(index, true);
        }

        private void UpdateBarBackgroundColor()
        {
            if (_disposed || _bottomBar == null)
            {
                return;
            }

            _bottomBar.SetBackgroundColor(Element.BarBackgroundColor.ToAndroid());
        }

        private void UpdateBarTextColor()
        {
            if (_disposed || _bottomBar == null)
            {
                return;
            }

            _bottomBar.SetActiveTabColor(Element.BarTextColor.ToAndroid());
            // The problem SetActiveTabColor does only work in fiexed mode // haven't found yet how to set text color for tab items on_bottomBar, doesn't seem to have a direct way
        }

        private void UpdateTabs()
        {
            // create tab items
            SetTabItems();

            // set tab colors
            SetTabColors();

            SetTabBadges();

            AddPropertyChangedHandlersForPages();
        }

        private void SetTabItems()
        {
            BottomBarTab[] tabs = Element.Children.Select(page =>
            {
                var tabIconId = ResourceManagerEx.IdFromTitle(page.Icon, ResourceManager.DrawableClass);
                return new BottomBarTab(tabIconId, page.Title);
            }).ToArray();

            if (tabs.Length > 0)
            {
                _bottomBar.SetItems(tabs);
            }
        }

        private void SetTabColors()
        {
            for (int i = 0; i < Element.Children.Count; ++i)
            {
                Page page = Element.Children[i];

                Color tabColor = BottomBarPageExtensions.GetTabColor(page);

                if (tabColor != Color.Transparent)
                {
                    _bottomBar.MapColorForTab(i, tabColor.ToAndroid());
                }
            }
        }

        private void SetTabBadges()
        {
            _badges = new Dictionary<Page, BottomBarBadge>(Element.Children.Count);

            foreach (var page in Element.Children)
            {
                CreateOrUpdateBadgeForPage(page);
            }
        }

        private void AddPropertyChangedHandlersForPages()
        {
            foreach (var page in Element.Children)
            {
                page.PropertyChanged += OnPagePropertyChanged;
            }
        }

        private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BottomBarPageExtensions.BadgeCountProperty.PropertyName)
            {
                var page = (Page)sender;

                CreateOrUpdateBadgeForPage(page);
            }
        }

        /// <summary>
        /// Creates or updates a badge for a page
        /// </summary>
        /// <param name="page"></param>
	    private void CreateOrUpdateBadgeForPage(Page page)
        {
            var pageIndex = Element.Children.IndexOf(page);
            var badgeCount = BottomBarPageExtensions.GetBadgeCount(page);

            BottomBarBadge badge;

            // We'll have to keep track of our badges, otherwise we can't update
            // and removing + inserting again gives a crappy user experience
            if (_badges.ContainsKey(page))
            {
                badge = _badges[page];
            }
            else
            {
                // Don't need to create a badge when there's no badge to show
                if (badgeCount == 0) return;

                // BottomBarBadge does not allow us to set color after init. You could, but you'd have to
                // do it manually because the circle background logic is hidden from us
                var badgeColor = BottomBarPageExtensions.GetBadgeColor(page);
                badge = _bottomBar.MakeBadgeForTabAt(pageIndex, badgeColor.ToAndroid(), badgeCount);
                _badges.Add(page, badge);
            }

            // Let's assume that if the new badge count is zero the
            // default behavior will be to hide the badge
            if (badgeCount == 0)
            {
                badge.Hide();
            }
            else
            {
                badge.Count = badgeCount;
                badge.Show();
            }
        }
    }
}
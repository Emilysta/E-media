using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E_media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavPage : Page
    {
        public NavPage()
        {
            this.InitializeComponent();
        }
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("imageTab", typeof(ImageTab)),
            ("metaTab", typeof(ImageTab)),
            ("fourierTab", typeof(ImageTab)),
        };

        private void NavView_ItemInvoked(NavigationView sender,
                                 NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(
            string navItemTag,
            Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type chosenPage = null;
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            chosenPage = item.Page;

            var currentPageType = ContentFrame.CurrentSourcePageType;

            // navigate if the selected page isn't currently loaded
            if (!(chosenPage is null) && !Type.Equals(currentPageType, chosenPage))
            {
                ContentFrame.Navigate(chosenPage, null, transitionInfo);
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            //ContentFrame.Na
            //// Add handler for ContentFrame navigation.
            //ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavViewControl.SelectedItem = NavViewControl.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("imageTab", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
    }
}

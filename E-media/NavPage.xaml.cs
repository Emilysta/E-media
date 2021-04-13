using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace E_media
{
    public sealed partial class NavPage : Page
    {
        public SvgImageSource image;
        public int myID;
        public NavPage()
        {
            this.InitializeComponent();
            myID = ImageListClass.GetID();
            ImageListClass.FileSourceList.Add(myID, null);
            NavViewControl.SelectedItem = NavViewControl.MenuItems[0];
            NavView_Navigate("imageTab", new EntranceNavigationTransitionInfo());
        }
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("imageTab", typeof(ImageTab)),
            ("metaTab", typeof(MetaTab)),
            ("fourierTab", typeof(FourierTab)),
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

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type chosenPage = null;
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            chosenPage = item.Page;

            var currentPageType = ContentFrame.CurrentSourcePageType;

            // navigate if the selected page isn't currently loaded
            if (!(chosenPage is null) && !Type.Equals(currentPageType, chosenPage))
            {
                ContentFrame.Navigate(chosenPage, myID, transitionInfo);
            }
        }

        public NavigationViewItem GetImageTabItem()
        {
            return imageTab;
        }

        public NavigationViewItem GetMetaTabItem()
        {
            return metaTab;
        }

        public NavigationViewItem GetFourierTabItem()
        {
            return fourierTab;
        }

    }
}

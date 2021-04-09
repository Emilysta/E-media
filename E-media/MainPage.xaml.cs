using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace E_media
{
    public class XMLPair
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stream fileStream;
        //public List<XMLPair> Metadata { get; set; }
        public MainPage()
        {
            this.InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(TabFooterControl);
        }

        private void TabViewControl_AddTabButtonClick(muxc.TabView sender, object args)
        {
            var newTab = new muxc.TabViewItem();
            newTab.Header = "Nowe SVG";
            Frame newFrame = new Frame();
            newTab.Content = newFrame;
            newFrame.Navigate(typeof(NavPage));
            sender.TabItems.Add(newTab);

        }

        private void TabViewControl_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        //private void ImageTab_CloseRequested(muxc.TabViewItem sender, muxc.TabViewTabCloseRequestedEventArgs args)
        //{
        //    TabViewControl.SelectedIndex = 0;
        //    ImageTab.Visibility = Visibility.Collapsed;
        //    MetadataTab.Visibility = Visibility.Collapsed;
        //    FourierTab.Visibility = Visibility.Collapsed;
        //}

        //private void MetadataTab_CloseRequested(muxc.TabViewItem sender, muxc.TabViewTabCloseRequestedEventArgs args)
        //{
        //    TabViewControl.SelectedIndex = 1;
        //    MetadataTab.Visibility = Visibility.Collapsed;
        //    Metadata = new List<XMLPair>();
        //}

        //private void FourierTab_CloseRequested(muxc.TabViewItem sender, muxc.TabViewTabCloseRequestedEventArgs args)
        //{
        //    TabViewControl.SelectedIndex = 1;
        //    FourierTab.Visibility = Visibility.Collapsed;
        //}

        //private XMLPair ReadXMLPair(string keyValuePair)
        //{
        //    string[] s = keyValuePair.Split('=');
        //    XMLPair pair = new XMLPair();
        //    pair.PropertyName = s[0];
        //    s[1] = s[1].Trim('"');
        //    pair.Value = s[1];
        //    Debug.WriteLine(pair.PropertyName + "="+pair.Value);
        //    return pair;
        //}
    }
}

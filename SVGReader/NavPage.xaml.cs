using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace SVGReader
{
    public partial class NavPage : Page
    {
        private int myID;
        public ImageTab imageTab;
        public MetaTab metaTab;
        public FourierTab fourierTab;
        public ImageAfterRemove removedTab;
        public RSAPage rsaTab;
        public NavPage()
        {
            InitializeComponent();
            myID = FileManager.GetID();
            imageTab = new ImageTab(myID, this);
            metaTab = new MetaTab(myID, this);
            fourierTab = new FourierTab(myID, this);
            removedTab = new ImageAfterRemove(myID, this);
            rsaTab = new RSAPage();
            imageTab.StartMethod();
            metaTab.StartMethod();
            SetContentToImagetab();
        }
        private void SetContentToMetatab()
        {
            NavContentFrame.Navigate(metaTab);
        }

        private void SetContentToImagetab()
        {
            NavContentFrame.Navigate(imageTab);
        }

        private void SetContentToFouriertab()
        {
            NavContentFrame.Navigate(fourierTab);
            fourierTab.StartMethod();
        }

        private void SetContentToRemovedtab()
        {
            NavContentFrame.Navigate(removedTab);
            removedTab.StartMethod();
        }

        private void SetContentToEncryption()
        {
            NavContentFrame.Navigate(rsaTab);
        }

        private void MetadataButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentToMetatab();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentToImagetab();
        }

        private void FourierButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentToFouriertab();
        }

        private void RemovedButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentToRemovedtab();
        }

        private void CypherButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentToEncryption();
        }
    }
}

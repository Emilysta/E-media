using System.Windows;
using System.Windows.Controls;


namespace SVGReader
{
    /// <summary>
    /// Logika interakcji dla klasy NavPage.xaml
    /// </summary>
    public partial class NavPage : Page
    {
        private int myID;
        public ImageTab imagetab;
        public MetaTab metatab;
        public FourierTab fouriertab;
        public NavPage()
        {
            InitializeComponent();
            myID = FileManager.GetID();
            imagetab = new ImageTab(myID,this);
            metatab = new MetaTab(myID,this);
            fouriertab = new FourierTab(myID,this);
            imagetab.StartMethod();
            metatab.StartMethod();
            fouriertab.StartMethod();
            SetContentToImagetab();
        }
        private void SetContentToMetatab()
        {
            NavContentFrame.Navigate(metatab);
        }

        private void SetContentToImagetab()
        {
            NavContentFrame.Navigate(imagetab);
        }

        private void SetContentToFouriertab()
        {
            NavContentFrame.Navigate(fouriertab);
            fouriertab.StartMethod();
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
    }
}

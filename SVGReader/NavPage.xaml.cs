using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace SVGReader
{
    public partial class NavPage : Page
    {
        private int myID;
        public ImageTab imagetab;
        public MetaTab metatab;
        public FourierTab fouriertab;
        public ImageAfterRemove removedtab;
        public NavPage()
        {
            InitializeComponent();
            myID = FileManager.GetID();
            imagetab = new ImageTab(myID, this);
            metatab = new MetaTab(myID, this);
            fouriertab = new FourierTab(myID, this);
            removedtab = new ImageAfterRemove(myID, this);
            imagetab.StartMethod();
            metatab.StartMethod();
            //fouriertab.StartMethod();
            //removedtab.StartMethod();
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

        private void SetContentToRemovedtab()
        {
            NavContentFrame.Navigate(removedtab);
            removedtab.StartMethod();
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter += "Scalable Vector Graphics | *.svg|All files (*.*)|*.*";
            if (dialog.ShowDialog() ?? false)
            {
                var key = new RSA.RSAKey();
                if (!key.ReadConfig("config.rsa"))
                    key = RSA.RSAKeyGenerator.GenerateKeyPair(512);
                RSA.RSAcbc.EncryptData(dialog.FileName, key);
            }
        }
    }
}

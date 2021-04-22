using System;
using System.Windows.Controls;

namespace SVGReader
{
    /// <summary>
    /// Logika interakcji dla klasy ImageTab.xaml
    /// </summary>
    public partial class ImageTab : Page
    {
        public int myID;
        NavPage navPage;
        public ImageTab(int id, NavPage parentNavPage)
        {
            this.InitializeComponent();
            myID = id;
            navPage = parentNavPage;
        }

        public void StartMethod()
        {
            if (FileManager.FileSourceList.TryGetValue(myID, out string file))
            {
                if (file != null)
                {
                    OpenFile(file);
                }
            }
        }

        private async void OpenFile(string file)
        {
            SVGImage.Source = new Uri(file);
            navPage.fouriertab.NormalImageControl.Source = new Uri(file);
        }
    }
}


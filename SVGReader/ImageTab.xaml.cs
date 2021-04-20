using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //FileOpenPicker fileOpenPicker = new FileOpenPicker();
            //fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            //fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //fileOpenPicker.FileTypeFilter.Add(".svg");
            //StorageFile file = await fileOpenPicker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    OpenFile(file);
            //}
        }


        private async void OpenFile(string file)
        {
            SVGImage.Source = new Uri(file);
            navPage.fouriertab.NormalImageControl.Source = new Uri(file);
        }
    }
}


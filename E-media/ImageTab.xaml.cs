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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E_media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageTab : Page
    {
        public SvgImageSource image;
        public int myID;
        private Stream fileStream;
        public ImageTab()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileOpenPicker.FileTypeFilter.Add(".svg");
            StorageFile file = await fileOpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                OpenFile(file);
                noImageText.Visibility = Visibility.Collapsed;
                MetadataButton.Visibility = Visibility.Visible;
                FourierButton.Visibility = Visibility.Visible;
                ClearMetadataButton.Visibility = Visibility.Visible;
            }

        }

        private async void OpenFile(StorageFile file)
        {
            var stream = await file.OpenAsync(FileAccessMode.Read);
            image = new SvgImageSource();
            await image.SetSourceAsync(stream);
            fileStream = stream.AsStreamForRead();
            ImageControl.Source = image;
        }

        private void MetadataButton_Click(object sender, RoutedEventArgs e)
        {
            //NavPage.navPage.GetMetaTabItem().Visibility = Visibility.Visible;
            //Wywala się na czytaniu pierwszej linijki
            /*
            Metadata = new List<XMLPair>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                if (line.StartsWith("<?xml"))
                {
                    string[] pairs = line.Split();
                    foreach(string s in pairs)
                    {
                        if (s.Contains('=')){
                            Metadata.Add(ReadXMLPair(s));
                        }
                    }
                }
            }
            Lista.ItemsSource = Metadata;
            MetadataTab.Visibility = Visibility.Visible;
            */
        }

        private void FourierButton_Click(object sender, RoutedEventArgs e)
        {
            //NavPage.navPage.GetFourierTabItem().Visibility = Visibility.Visible;
        }
    }
}

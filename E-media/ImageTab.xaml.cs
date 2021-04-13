using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;


namespace E_media
{
    public sealed partial class ImageTab : Page
    {
        public int myID;
        public ImageTab()
        {
            this.InitializeComponent();
            if (ImageListClass.FileSourceList.TryGetValue(myID, out StorageFile file))
            {
                if (file != null)
                {
                    OpenFile(file);
                }
            }
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
            }
        }

        private async void OpenFile(StorageFile file)
        {
            ImageListClass.FileSourceList[myID] = file;
            var stream = await ImageListClass.FileSourceList[myID].OpenAsync(FileAccessMode.Read);
            SvgImageSource image = new SvgImageSource();
            await image.SetSourceAsync(stream);
            ImageControl.Source = image;

            noImageText.Visibility = Visibility.Collapsed;
            MetadataButton.Visibility = Visibility.Visible;
            FourierButton.Visibility = Visibility.Visible;
            ClearMetadataButton.Visibility = Visibility.Visible;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myID = (int)e.Parameter;
        }
    }
}

using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;


namespace E_media
{
    public sealed partial class FourierTab : Page
    {
        public int myID;
        public FourierTab()
        {
            this.InitializeComponent();
            RenderFourierTransform();
        }

        private async void RenderFourierTransform()
        {
            var stream = await ImageListClass.FileSourceList[myID].OpenAsync(FileAccessMode.Read);
            SvgImageSource image = new SvgImageSource();
            await image.SetSourceAsync(stream);
            NormalImageControl.Source = image;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myID = (int)e.Parameter;
        }

        private async void ShowButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(NormalImageControl);
            var pixelBuffer = await bitmap.GetPixelsAsync();
            byte[] pixels = pixelBuffer.ToArray();
            var wb = BitmapFactory.New(bitmap.PixelWidth, bitmap.PixelHeight);
            using (Stream stream = wb.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0, pixels.Length);
            }

            wb = FFT.AddPadding(wb);
            wb = FFT.MakeFFT(wb);
            FourierImageControl.Source = wb;
        }
    }
}

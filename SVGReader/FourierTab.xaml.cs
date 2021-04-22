using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SVGReader
{
    /// <summary>
    /// Logika interakcji dla klasy FourierTab.xaml
    /// </summary>
    public partial class FourierTab : Page
    {
        public int myID;
        NavPage navPage;
        public FourierTab(int id,NavPage parentNavPage)
        {
            this.InitializeComponent();
            myID = id;
            navPage = parentNavPage;
        }

        public void StartMethod()
        {
            new Thread(() =>
            {
                Thread.Sleep(200);
                Dispatcher.Invoke(RenderFourierTransform);
            }).Start();
        }

        private void RenderFourierTransform()
        {
            XMLNode node = navPage.metatab.Nodes.Find(x => x.Name == "svg");
            int width = (int)navPage.imagetab.SVGImage.ActualWidth;
            int height = (int)navPage.imagetab.SVGImage.ActualHeight;
            var renderBitmap = new RenderTargetBitmap(width,height, 96, 96,PixelFormats.Pbgra32);

            var visualBrush = new VisualBrush
            {
                Visual = navPage.imagetab.SVGImage,
                Stretch = Stretch.Uniform
            };
            System.Windows.Size size = new System.Windows.Size(width,height);

            var drawingvisual = new DrawingVisual();

            using (var context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(visualBrush, null, new Rect(size));
            }

            renderBitmap.Render(drawingvisual);
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
            Bitmap bitmap = new Bitmap(stream);

            bitmap = ImageProcessing.AddPadding(bitmap);

            PaddingImageControl.Source = ImageProcessing.ToBitmapImage(bitmap);
            MagnitudeImageControl.Source =ImageProcessing.MakeFFTMagnitude(bitmap);
            PhaseImageControl.Source =ImageProcessing.MakeFFTPhase(bitmap);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using AForge.Math;

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
            RenderFourierTransform();
        }

        private void RenderFourierTransform()
        {
            XMLNode node = navPage.metatab.Nodes.Find(x => x.Name == "svg");
            int width = Convert.ToInt32( node.Attributes.Find(x => x.PropertyName == "width").Value);
            int height = Convert.ToInt32(node.Attributes.Find(x => x.PropertyName == "height").Value);
            Trace.WriteLine("szerokosc: " + width);
            Trace.WriteLine("wysokosc: " + height);
            var renderBitmap = new RenderTargetBitmap(width, height, 96, 96,PixelFormats.Pbgra32);

            var visualBrush = new VisualBrush
            {
                Visual = NormalImageControl,
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
            //Bitmap bitmapWithPadd = ImageProcessing.AddPadding(bitmap);
            //(ImageSource)converter.ConvertFrom(greyscale);
            Bitmap fourier = ImageProcessing.MakeFFT(bitmap);
            FourierImageControl.Source = ToBitmapImage(fourier);
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using System.Drawing.Drawing2D;

namespace SVGReader
{
    public static class ImageProcessing
    {
        public static Bitmap AddPadding(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            int size = 0;
            int n = 0;
            while (size <= Math.Max(width, height))
            {
                size = (int)Math.Pow(2, n);
                if (size == Math.Max(width, height))
                    break;
                n++;
            }
            if ((width == height) && width == size)
                return image;
            Bitmap bmp = new Bitmap(size, size);
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        if (i < width && j < height)
            //            bmp.SetPixel(i, j, image.GetPixel(i, j));
            //        else
            //            bmp.SetPixel(i, j, Color.Black);
            //    }
            //}

            float scale = Math.Min(size / image.Width, size / image.Height);
            var graph = Graphics.FromImage(bmp);
            var brush = new SolidBrush(Color.Black);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, size,size));
            graph.DrawImage(image, ((int)size - scaleWidth) / 2, ((int)size - scaleHeight) / 2, scaleWidth, scaleHeight);
            bmp.Save("Test2.jpg");
            return bmp;
        }

        public static BitmapImage MakeFFTPhase(Bitmap image)
        {
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(image);
            ComplexImage compleximage = ComplexImage.FromBitmap(grayImage);
            compleximage.ForwardFourierTransform();
            Bitmap fi = ToBitmapPhase(compleximage, image.Width);
            return ToBitmapImage(fi);
        }

        public static BitmapImage MakeFFTMagnitude(Bitmap image)
        {
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(image);
            ComplexImage compleximage = ComplexImage.FromBitmap(grayImage);
            compleximage.ForwardFourierTransform();
            Bitmap fi = compleximage.ToBitmap();
            return ToBitmapImage(fi);
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static Bitmap ToBitmapPhase(ComplexImage complexImage, int width)
        {
            // create new image
            Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage(width, width);

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle(0, 0, width, width),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int offset = dstData.Stride - width;
            double scale = Math.Sqrt(width * width);
            // do the job
            unsafe
            {
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {

                        *dst = (byte)Math.Max(0, Math.Min(255, complexImage.Data[y, x].Phase * scale * 255));
                    }
                    dst += offset;
                }
            }
            // unlock destination images
            dstImage.UnlockBits(dstData);

            return dstImage;
        }
    }
}

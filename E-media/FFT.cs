using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using AForge.Math;
using Windows.UI;

namespace E_media
{
    //BGRA8
    public class FFT
    {
        public static WriteableBitmap AddPadding(WriteableBitmap image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int size = 0;
            int n = 0;
            while (size <= Math.Max(width, height))
            {
                size = (int)Math.Pow(2, n);
                if (size == Math.Max(width, height))
                    break;
                n++;
            }
            var img = image.Crop(0, 0, size, size);
            return img;
            WriteableBitmap wb = BitmapFactory.New(size, size);
            //for (int i = 0; i < width; i++)
            //{
            //    for (int j = 0; j < height; j++)
            //    {
            //        wb.SetPixel(i, j, image.GetPixel(i, j));
            //    }
            //}
            //for (int i = width; i < size; i++)
            //{
            //    for (int j = height; j < size; j++)
            //    {
            //        wb.SetPixel(i, j, 0);
            //    }
            //}
            //wb.Invalidate();

            //return wb;
        }

        public static WriteableBitmap MakeFFT(WriteableBitmap image)
        {
            Complex[,] grayImage = ToGrayScale(image);
            FourierTransform.FFT2(grayImage, FourierTransform.Direction.Forward);

            return ComplexToMagnitude(grayImage,image.PixelHeight);
        }

        public static WriteableBitmap ComplexToMagnitude(Complex[,] data, int size)
        {
            WriteableBitmap wb = BitmapFactory.New(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    wb.SetPixel(i, j, (byte)data[i, j].Magnitude);
                }
            }
            return wb;
        }

        public static Complex[,] ToGrayScale(WriteableBitmap image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            Complex[,] grayscale = new Complex[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = image.GetPixel(i, j);
                    grayscale[i, j] = new Complex(color.R / 3 + color.G / 3 + color.B / 3, 0);
                }
            }
            return grayscale;
        }

    }
}

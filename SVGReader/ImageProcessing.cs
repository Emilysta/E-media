using AForge.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Imaging;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;

namespace SVGReader
{
    class ImageProcessing
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
            Bitmap bmp = new Bitmap(size, size);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bmp.SetPixel(i, j, image.GetPixel(i, j));
                }
            }
            for (int i = width; i < size; i++)
            {
                for (int j = height; j < size; j++)
                {
                    bmp.SetPixel(i, j, Color.Black);
                }
            }
            return bmp;
        }

        public static Bitmap MakeFFT(Bitmap image)
        {
            Grayscale filter = new Grayscale(0.3,0.3,0.3);
            Bitmap grayImage = filter.Apply(image);
            //return grayImage;
            ComplexImage complexImage = ComplexImage.FromBitmap(grayImage);
            complexImage.ForwardFourierTransform();
            Bitmap fourierBitmap = complexImage.ToBitmap();
            Bitmap bmp = new Bitmap(complexImage.Width, complexImage.Height);
            for(int i = 0; i < fourierBitmap.Width; i++)
            {
                for(int j = 0; j < fourierBitmap.Height; j++)
                {
                    bmp.SetPixel(i, j, Color.FromArgb(255, (int)complexImage.Data[i, j].Magnitude, (int)complexImage.Data[i, j].Magnitude, (int)complexImage.Data[i, j].Magnitude));
                }
            }

            return bmp;
        }
    }
}

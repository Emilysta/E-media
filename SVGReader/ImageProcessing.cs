using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;
using System.Numerics;
using System.Linq;

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
            Grayscale filter = new Grayscale(0.33, 0.33, 0.33);
            Bitmap grayImage = filter.Apply(image);
            Complex[][] complexImage = Forward(grayImage);
            return ToBitmap(complexImage);
        }
        public static Complex[][] ToComplex(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            BitmapData bitmapData = image.LockBits(
                           new Rectangle(0, 0, width, height),
                           ImageLockMode.ReadOnly,
                           PixelFormat.Format8bppIndexed);

            byte[] buffer = new byte[width * height];
            Complex[][] result = new Complex[width][];

            Marshal.Copy(bitmapData.Scan0, buffer, 0, buffer.Length);
            image.UnlockBits(bitmapData);

            int pixel_position;

            for (int i = 0; i < width; i++)
            {
                result[i] = new Complex[height];
                for (int j = 0; j < height; j++)
                {
                    pixel_position = i * width + j;
                    result[i][j] = new Complex(buffer[pixel_position], 0);
                }
            }
            return result;
        }
        public static Bitmap ToBitmap(Complex[][] complexImage)
        {
            float[] frequencies = new float[complexImage.Length * complexImage.Length];
            for (int i = 0; i < complexImage.Length; i++)
            {
                for (int j = 0; j < complexImage[0].Length; j++)
                {
                    frequencies[i * complexImage.Length + j] = (float)complexImage[i][j].Magnitude;
                }
            }

            float min = frequencies.Min();
            float max = frequencies.Max();
            double log_constant = 255 / Math.Log10(1 + 255);
            for (int i = 0; i < frequencies.Length; i++)
            {
                frequencies[i] = 255 * (frequencies[i] - min) / (max - min);
                frequencies[i] = (int)(Math.Log10(1 + frequencies[i]) * log_constant);
            }

            Bitmap image = new Bitmap(complexImage.Length, complexImage.Length);
            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, complexImage.Length, complexImage.Length),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] result = new byte[bytes];
            for (int y = 0; y < complexImage.Length; y++)
            {
                for (int x = 0; x < complexImage.Length; x++)
                {
                    int pixel_position = y * image_data.Stride + x * 4;
                    for (int i = 0; i < 3; i++)
                    {
                        result[pixel_position + i] = (byte)frequencies[y * complexImage.Length + x];
                    }
                    result[pixel_position + 3] = 255;
                }
            }
            Marshal.Copy(result, 0, image_data.Scan0, bytes);
            image.UnlockBits(image_data);
            return image;
        }

        public static Complex[][] Forward(Bitmap image)
        {
            int size = image.Width;
            var p = new Complex[size][];
            var f = new Complex[size][];
            var t = new Complex[size][];

            var complexImage = ToComplex(image);

            for (int l = 0; l < size; l++)
            {
                p[l] = Forward(complexImage[l]);
            }

            for (int l = 0; l < size; l++)
            {
                t[l] = new Complex[size];
                for (int k = 0; k < size; k++)
                {
                    t[l][k] = p[k][l];
                }
                f[l] = Forward(t[l]);
            }

            return f;
        }
        public static Complex[] Forward(Complex[] input, bool phaseShift = true)
        {
            int size = input.Length;
            var result = new Complex[input.Length];
            var omega = (float)(-2.0 * Math.PI / input.Length);

            if (input.Length == 1)
            {
                result[0] = input[0];

                if (Complex.IsNaN(result[0]))
                {
                    return new[] { new Complex(0, 0) };
                }
                return result;
            }

            var evenInput = new Complex[input.Length / 2];
            var oddInput = new Complex[input.Length / 2];

            for (int i = 0; i < input.Length / 2; i++)
            {
                evenInput[i] = input[2 * i];
                oddInput[i] = input[2 * i + 1];
            }

            var even = Forward(evenInput, phaseShift);
            var odd = Forward(oddInput, phaseShift);

            for (int k = 0; k < input.Length / 2; k++)
            {
                int phase;

                if (phaseShift)
                {
                    phase = k - size / 2;
                }
                else
                {
                    phase = k;
                }
                odd[k] *= Complex.FromPolarCoordinates(1, omega * phase);
            }

            for (int k = 0; k < input.Length / 2; k++)
            {
                result[k] = even[k] + odd[k];
                result[k + input.Length / 2] = even[k] - odd[k];
            }

            return result;
        }
    }
}

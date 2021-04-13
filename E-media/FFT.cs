using System;
using Windows.UI.Xaml.Media.Imaging;

namespace E_media
{
    public class FFT
    {
        public static BitmapImage AddPadding(BitmapImage image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int size = 0;
            while (size < Math.Max(width, height))
            {
                size = (int)Math.Pow(2, size);
                if (size == Math.Max(width, height))
                    break;
                size++;
            }

            int top_padding = size - height;
            int right_padding = size - width;

            BitmapImage imageWithPadding = new BitmapImage();

            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                   // imageWithPadding.SetPixel(i, j, image.GetPixel(i, j));
                }
            }
            for(int i = width; i < size; i++)
            {
                for (int j = height; j < size; j++)
                {
                   // imageWithPadding.SetPixel(i, j, Color.Black);
                }
            }
            return imageWithPadding;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace E_media
{
    public class ImageListElement
    {
        public int number;
        public NavPage image;
        //public SvgImageSource image;
    }

    public static class ImageListClass
    {
        static List<ImageListElement> _imageList;
        static int _counter;
        
        static ImageListClass()
        {
            _imageList = new List<ImageListElement>();
            _counter = 0;
        }
        public static void Add(NavPage imageFromPage)
        {
            _imageList.Add(new ImageListElement
            {
                number = _counter,
                image = imageFromPage
            });
            _counter++;
        }

        public static void Remove(int tabNumber)
        {
            _imageList.Remove(_imageList.Find(x => x.number == tabNumber));
        }

        public static NavPage GetImage(int tabNumber)
        {
            return _imageList.Find(x => x.number == tabNumber).image;
        }

        public static int GetNextID()
        {
            return _counter;
        }
    }
}

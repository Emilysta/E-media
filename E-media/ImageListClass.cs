using System.Collections.Generic;
using Windows.Storage;

namespace E_media
{

    public static class ImageListClass
    {
        public static Dictionary<int, StorageFile> FileSourceList;
        public static int _counter;

        static ImageListClass()
        {
            FileSourceList = new Dictionary<int, StorageFile>();
            _counter = 0;
        }

        public static int GetID()
        {
            return _counter++;
        }
    }
}

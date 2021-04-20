using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGReader
{
    class FileManager
    {
        public static Dictionary<int, string> FileSourceList;
        public static int _counter;

        static FileManager()
        {
            FileSourceList = new Dictionary<int, string>();
            _counter = 0;
        }

        public static int GetID()
        {
            return _counter;
        }
        public static int GetNextID()
        {
            _counter++;
            return _counter;
        }
    }
}

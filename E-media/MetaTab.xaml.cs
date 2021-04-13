using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace E_media
{
    public class XMLPair
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }

    public sealed partial class MetaTab : Page
    {
        public int myID;
        public List<XMLPair> Metadata { get; set; }
        public MetaTab()
        {
            this.InitializeComponent();
        }

        private XMLPair ReadXMLPair(string keyValuePair)
        {
            string[] s = keyValuePair.Split('=');
            XMLPair pair = new XMLPair();
            pair.PropertyName = s[0];
            s[1] = s[1].TrimStart('"');
            s[1] = s[1].TrimEnd('"');
            pair.Value = s[1];
            Debug.WriteLine(pair.PropertyName + "=" + pair.Value);
            return pair;
        }

        private async void ReadMetadata()
        {
            IRandomAccessStream stream = await ImageListClass.FileSourceList[myID].OpenAsync(FileAccessMode.Read);
            Metadata = new List<XMLPair>();
            using (StreamReader reader = new StreamReader(stream.AsStreamForRead()))
            {
                string line = reader.ReadLine();
                if (line.StartsWith("<svg"))
                {
                    string[] pairs = line.Split();
                    foreach(string s in pairs)
                    {
                        if (s.Contains('=')){
                            Metadata.Add(ReadXMLPair(s));
                        }
                    }
                }
            }
            metaList.ItemsSource = Metadata; 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myID = (int)e.Parameter;
            ReadMetadata();
        }
    }
}

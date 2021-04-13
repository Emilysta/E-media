using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E_media
{
    public class XMLPair
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MetaTab : Page
    {
        public int myID;
        public List<XMLPair> Metadata { get; set; }
        public MetaTab()
        {
            this.InitializeComponent();
            ReadMetadata();
        }

        private XMLPair ReadXMLPair(string keyValuePair)
        {
            string[] s = keyValuePair.Split('=');
            XMLPair pair = new XMLPair();
            pair.PropertyName = s[0];
            s[1] = s[1].Trim('"');
            pair.Value = s[1];
            Debug.WriteLine(pair.PropertyName + "=" + pair.Value);
            return pair;
        }

        private void ReadMetadata()
        {
            Metadata = new List<XMLPair>();
            for (int i = 0; i < 200; i++)
            {
                Metadata.Add(new XMLPair
                {
                    PropertyName = "elo",
                    Value = "1"
                });
            }
            metaList.ItemsSource = Metadata;

            //Wywala się na czytaniu pierwszej linijki
            /*
            Metadata = new List<XMLPair>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                if (line.StartsWith("<?xml"))
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
            Lista.ItemsSource = Metadata;
            MetadataTab.Visibility = Visibility.Visible;
            */
        }
    }
}

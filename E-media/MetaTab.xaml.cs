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

        List<string> markerLikeSVG = new List<string>() { "<svg", "<?xml","<!DOCTYPE","<!--" };
        List<string> markerLikeTitle = new List<string>() { "title", "metadata", "desc"};
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
                string tempLine;
                string line = "";
                while ((tempLine = reader.ReadLine()) != null)
                {
                    tempLine = tempLine.Trim();
                    if(tempLine.StartsWith("<"))
                    {
                        line = tempLine;
                    }
                    else
                    {
                        line += " ";
                        line += tempLine;
                    }
                    if(line.EndsWith(">"))
                    {
                        int j = 0;
                        for(int i =0;i<markerLikeSVG.Count;i++)
                        {
                            if(line.StartsWith(markerLikeSVG[i]))
                            {
                                ReadMarkerLikeSVG(line);
                                j = markerLikeTitle.Count;
                                break;
                            }
                        }
                        for (int i = j; i < markerLikeTitle.Count; i++)
                        {
                            if (line.StartsWith("<"+markerLikeTitle[i]+">") && line.EndsWith("</"+markerLikeTitle[i]+">")) 
                            {
                                ReadMarkerLikeTitle(line,markerLikeTitle[i]);
                                break;
                            }
                        }

                    }
                }
            }
            metaList.ItemsSource = Metadata; 
        }

        private void ReadMarkerLikeTitle(string line,string marker)
        {
            line=line.Replace("<"+marker+">","");
            line=line.Replace("</" + marker + ">", "");
            Metadata.Add(new XMLPair() {
            PropertyName = marker,
            Value = line
            });
        }

        private void ReadMarkerLikeSVG(string line)
        {
            string[] pairs = line.Split();
            int lastIndex = pairs.Count() - 1;
            pairs[lastIndex] = pairs[lastIndex].Trim('>');
            if(line.StartsWith("<?xml"))
            {
                pairs[lastIndex] = pairs[lastIndex].Trim('?');
            }

            for (int i = 0;i<lastIndex+1;i++)
            {
                string temp = pairs[i];
                if (temp.Contains('='))
                {
                    if (temp.EndsWith('"'))
                    {
                        Metadata.Add(ReadXMLPair(temp));
                    }
                    else
                    {
                        string temp2 = temp;
                        while(!temp2.EndsWith('"'))
                        {
                            ++i;
                            temp2 += " ";
                            temp2 += pairs[i];
                        }
                        Metadata.Add(ReadXMLPair(temp2));
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myID = (int)e.Parameter;
            ReadMetadata();
        }
    }
}
